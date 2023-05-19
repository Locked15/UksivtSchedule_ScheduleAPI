using ScheduleAPI.Controllers.API.V1.Schedule.Replacements;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Document;
using ScheduleAPI.Controllers.Data.Workers.Cache;
using ScheduleAPI.Controllers.Data.Workers.Parsers;
using ScheduleAPI.Models.Elements.Site;
using ScheduleAPI.Models.Exceptions.Data;
using ScheduleAPI.Models.Result.Schedule.Replacements;

namespace ScheduleAPI.Controllers.Data.Getter.Replacements
{
    /// <summary>
    /// Класс, обертывающий функционал получения замен.
    /// Предназначен для получения таргетированных замен — замен для какой-либо группы (на день или неделю).
    /// <br />
    /// Он был обычным, стал статическим, и вот он снова обычный.
    /// Теперь статика применяется для работы с кэшем.
    /// </summary>
    public class TargetReplacementsGetter
    {
        #region Область: Поля.

        /// <summary>
        /// Содержит основные функции и поля, необходимые для работы с кэшем в данном классе. <br />
        /// Ранее был частью. самого класса, но позже логика была вынесена в отдельный файл.
        /// </summary>
        private static ReplacementsGetterCacheWorker cacheWorker;
        #endregion

        #region Область: Свойства.

        /// <summary>
        /// Индекс дня для поиска. <br />
        /// Он игнорируется в случае поиска расписания для недели.
        /// </summary>
        public int DayIndex { get; set; }

        /// <summary>
        /// Название группы, которую предполагается искать. <br />
        /// Игнорируется в случае поиска всех доступных замен.
        /// </summary>
        public string GroupName { get; set; }
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Инициализирует новый объект для получения замен.
        /// </summary>
        /// <param name="dayIndex">Индекс дня, на который необходимы замены (0..6). Игнорируется, если необходимы замены на неделю.</param>
        /// <param name="groupName">Название группы, для которой нужно извлечь замены.</param>
        public TargetReplacementsGetter(int dayIndex, string groupName)
        {
            DayIndex = dayIndex;
            GroupName = groupName.RemoveStringChars();
        }

        /// <summary>
        /// Инициализирует объект для работы с кэшем.
        /// </summary>
        static TargetReplacementsGetter() =>
               cacheWorker = new ReplacementsGetterCacheWorker();
        #endregion

        #region Область: Публичные методы.

        /// <summary>
        /// Метод для получения замен на день.
        /// <br/>
        /// <strong>РЕКОМЕНДУЕТСЯ АСИНХРОННОЕ ВЫПОЛНЕНИЕ.</strong>
        /// </summary>
        /// <returns>Объект, содержащий замены для группы.</returns>
        public ReplacementsOfDay GetDayReplacements()
        {
            if (cacheWorker.TryToFindTargetCachedChangesValue(DayIndex, GroupName) is var restored && restored != null)
                return restored;

            if (GetTargetChangesElement() is ReplacementNodeElement changeElement)
            {
                ReplacementsOfDay toReturn = CompleteWorkWithChangesDocument(changeElement);
                toReturn.NewLessons.ForEach(lesson => lesson.Changed = true);

                cacheWorker.TryToAddValueToCachedVault(toReturn, GroupName);
                return toReturn;
            }
            else
            {
                return new();
            }
        }

        /// <summary>
        /// Метод для получения замен на неделю.
        /// <br/>
        /// <strong>РЕКОМЕНДУЕТСЯ АСИНХРОННОЕ ВЫПОЛНЕНИЕ.</strong>
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Список с объектами, содержащими замены на каждый день.</returns>
        public List<ReplacementsOfDay> GetWeekReplacements()
        {
            var basicDayIndex = DayIndex;
            List<ReplacementsOfDay> list = new(1);

            for (int i = 0; i < 7; i++)
            {
                DayIndex = i;
                list.Add(GetDayReplacements());
            }

            DayIndex = basicDayIndex;
            return list;
        }
        #endregion

        #region Область: Внутренние методы.

        private ReplacementNodeElement? GetTargetChangesElement()
        {
            try
            {
                var changes = new SiteParser().ParseAvailableNodes();
                var element = changes.TryToFindElementByNameOfDayWithoutPreviousWeeks(DayIndex.GetDayByIndex());

                if (element == null)
                {
                    ReplacementsController.Logger?.Log(LogLevel.Information, "При получении замен искомое значение не обнаружено: " +
                                                                        "День: {dayIndex}, Текущая дата — {time}.", DayIndex, DateTime.Now.ToShortDateString());
                }

                return element;
            }

            catch (Exception ex)
            {
                ReplacementsController.Logger?.LogError(3, "При получении замен произошла ошибка парса страницы: {message}.", ex.Message);

                return null;
            }
        }

        private ReplacementsOfDay CompleteWorkWithChangesDocument(ReplacementNodeElement? element)
        {
            ReplacementsOfDay toReturn;

            try
            {
                DocumentParser reader = InitializeReader(DayIndex, element, out bool deleteDocumentAfterWork, out string pathToDocument);
                toReturn = GetChangesFromReader(reader, element.Date);

                if (deleteDocumentAfterWork)
                    DeleteChangesDocumentAsync(pathToDocument);
            }

            catch (HttpRequestException)
            {
                ReplacementsController.Logger?.Log(LogLevel.Error, "Удалённый сервер хранения документов замен (Google Drive) оказался недоступен.");
                toReturn = new();
            }

            catch (Exception exception)
            {
                ReplacementsController.Logger?.Log(LogLevel.Error, "Произошла ошибка при работе с документом замен: {message}.", exception.Message);
                toReturn = new();
            }

            return toReturn;
        }


        #region Подобласть: Работа с 'ридером'.

        /// <summary>
        /// Инициализирует экземпляр класса "DocumentParser", используемый для получения замен из документа. <br />
        /// Если в процессе будет скачан новый документ, то он будет автоматически кэширован.
        /// </summary>
        /// <param name="dayIndex">Индекс дня, на который нужно получить замены (для сверки кэшированных документов).</param>
        /// <param name="element">Элемент с заменой с сайта колледжа, который содержит ссылку на документ с заменами.</param>
        /// <param name="deleteDocumentAfterWork">Возвращаемая переменная, отвечающая за то, будут ли проводиться попытки удалить документ с заменами после работы.</param>
        /// <param name="pathToDocument">
        /// Путь к документу с заменами. <br />
        /// Если документ был найден в кэше — пустая строка. <br />
        /// Иначе — полноценный путь.
        /// </param>
        /// <returns>Инициализированный экземпляр класса "DocumentParser", готовый к работе.</returns>
        /// <exception cref="HttpRequestException"/>
        private DocumentParser InitializeReader(int dayIndex, ReplacementNodeElement element, out bool deleteDocumentAfterWork, out string pathToDocument)
        {
            DateOnly targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
            if (cacheWorker.TryToFindTargetCachedDocumentValue(targetDate) is var document && document != null)
            {
                deleteDocumentAfterWork = false;
                pathToDocument = string.Empty;

                return new(document);
            }

            #region Подобласть: Получение нового документа с заменами.

            pathToDocument = DocumentGetter.DownloadChangesDocument(element);
            deleteDocumentAfterWork = true;

            // Если мы берем документ из кэша, то его путь будет пустым. Аналогичное происходит при неудачном скачивании документа.
            if (!string.IsNullOrEmpty(pathToDocument))
            {
                DocumentParser reader = ProcessNewReaderCreation(pathToDocument);

                return reader;
            }

            else
            {
                throw new HttpRequestException("При попытке скачать документ произошла ошибка.");
            }
            #endregion
        }

        /// <summary>
        /// Выполняет необходимые операции по генерации нового объекта чтения документа с заменами.
        /// Создает объект, пытается кэшировать его.
        /// </summary>
        /// <param name="pathToDocument">Путь к скачанному документу.</param>
        /// <returns>Объект для чтения документа.</returns>
        private static DocumentParser ProcessNewReaderCreation(string pathToDocument)
        {
            var reader = new DocumentParser(pathToDocument);
            cacheWorker.TryToAddValueToCachedVault(reader.ChangesDocument);

            return reader;
        }

        /// <summary>
        /// Выполняет работу по получению замен из отправленного экземпляра "DocumentParser".
        /// </summary>
        /// <param name="reader">Экземпляр класса "ChangedReader", с указанным документов для чтения замен.</param>
        /// <param name="date"></param>
        /// <returns>Замены, соответствующие указанной дате и группе.</returns>
        private ReplacementsOfDay GetChangesFromReader(DocumentParser reader, DateTime? date)
        {
            ReplacementsOfDay toReturn;
            try
            {
                toReturn = reader.GetOnlyChanges(DayIndex.GetDayByIndex(), GroupName);
                toReturn.ChangesDate = date;
                toReturn.ChangesFound = true;
            }

            catch (WrongDayInDocumentException exception)
            {
                ReplacementsController.Logger?.Log(LogLevel.Information, "При обработке документа обнаружилось несоответствие дат. Очередная ошибка составителей замен?\n" +
                                                                    "{message}.", exception.Message);
                toReturn = new();
            }

            catch (Exception exception)
            {
                ReplacementsController.Logger?.Log(LogLevel.Error, "Произшла какая-то ошибка, при работе с заменами:\n{message}.", exception.Message);
                toReturn = new();
            }

            return toReturn;
        }

        /// <summary>
        /// Асинхронная функция. Пытается удалить файл по указанному пути. <br />
        /// Предназначен для удаления документа с заменами, после завершения работы с ним.
        /// </summary>
        /// <param name="path">Путь, по которому находится документ с заменами.</param>
        private static async void DeleteChangesDocumentAsync(string path)
        {
            await Task.Run(() =>
            {
                int delAttempts = 0;
                bool deleted = false;

                while (!deleted && delAttempts < 5)
                {
                    try
                    {
                        File.Delete(path);

                        deleted = true;
                    }

                    catch (IOException)
                    {
                        delAttempts++;

                        Thread.Sleep(100);
                    }
                }

                if (!deleted)
                {
                    ReplacementsController.Logger?.Log(LogLevel.Warning, "Удалить файл с заменами не удалось.");
                }
            });
        }
        #endregion
        #endregion
    }
}
