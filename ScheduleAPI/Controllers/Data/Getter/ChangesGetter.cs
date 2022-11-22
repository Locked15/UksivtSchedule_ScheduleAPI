using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Cache;
using ScheduleAPI.Models.Cache.CachedTypes;
using NPOI.XWPF.UserModel;
using ScheduleAPI.Controllers.API.Schedule;
using ScheduleAPI.Controllers.Data.Getter.Parsers;
using ScheduleAPI.Models.Exceptions;
using ScheduleAPI.Models.Elements.Site;
using ScheduleAPI.Models.Elements.Schedule;

namespace ScheduleAPI.Controllers.Data.Getter
{
    /// <summary>
    /// Класс, обертывающий функционал получения замен.
    /// <br />
    /// В связи с переработкой класс был изменен и переделан под статический.
    /// </summary>
    public static class ChangesGetter
    {
        #region Область: Поля.

        /// <summary>
        /// Хранилище кэша, содержащее кэшированные замены для расписаний. <br />
        /// Прямое указание типов (вместо привязки к базовым элементам (как в "Dependency Inversion")), позволяет проще понять код.
        /// <br /><br />
        /// "ChangesOfDayCache" сразу видно, в отличие от "AbstractCacheElement<ChangesOfDay>".
        /// </summary>
        private static readonly CachedVault<ChangesOfDayCache, ChangesOfDay> cachedChanges;

        /// <summary>
        /// Хранилище кэша, содержащее кэшированные документы с заменами для определенных дней.
        /// </summary>
        private static readonly CachedVault<ChangesDocumentCache, XWPFDocument> cachedDocuments;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Статический конструктор класса.
        /// </summary>
        static ChangesGetter()
        {
            cachedChanges = new();
            cachedChanges.TryToRestoreCachedValues();

            cachedDocuments = new(8);
            cachedDocuments.TryToRestoreCachedValues();
        }
        #endregion

        #region Область: Публичные методы.

        /// <summary>
        /// Метод для получения замен на день.
        /// <br/>
        /// <strong>РЕКОМЕНДУЕТСЯ АСИНХРОННОЕ ВЫПОЛНЕНИЕ.</strong>
        /// </summary>
        /// <param name="dayIndex">Индекс дня.</param>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Объект, содержащий замены для группы.</returns>
        public static ChangesOfDay GetDayChanges(int dayIndex, string groupName)
        {
            ChangeElement? element = default;
            List<MonthChanges>? changes = default;
            groupName = groupName.RemoveStringChars();

            #region Подобласть: Проверка кэшированных замен.

            var cachedElement = cachedChanges.Get(el =>
            {
                var basicTargetingDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
                var basicCachedValueDate = DateOnly.FromDateTime(el.CachedElement.ChangesDate.GetValueOrDefault(new DateTime(0)));

                return basicTargetingDate.Equals(basicCachedValueDate) && groupName.Equals(el.GroupName);
            });

            if (cachedElement != null)
            {
                return cachedElement;
            }
            #endregion

            #region Подобласть: Обрабатываем возможные ошибки.

            try
            {
                changes = new SiteParser().ParseAvailableNodes();
                element = changes.TryToFindElementByNameOfDayWithoutPreviousWeeks(dayIndex.GetDayByIndex());
            }

            catch (Exception ex)
            {
                var exceptionReturn = new ChangesOfDay
                {
                    ChangesDate = dayIndex.GetDateTimeInWeek()
                };

                ChangesController.Logger?.LogError(3, "При получении замен произошла ошибка парса страницы: {message}.", ex.Message);

                cachedChanges.Add(new(exceptionReturn, groupName));
                return exceptionReturn;
            }

            if (element == null)
            {
                var exceptionReturn = new ChangesOfDay
                {
                    ChangesDate = dayIndex.GetDateTimeInWeek()
                };

                ChangesController.Logger?.Log(LogLevel.Information, "При получении замен искомое значение не обнаружено: " +
                                              "День: {dayIndex}, Текущая дата — {time}.", dayIndex, DateTime.Now.ToShortDateString());

                cachedChanges.Add(new(exceptionReturn, groupName));
                return exceptionReturn;
            }
            #endregion

            #region Подобласть: Работа с файлом замен.

            ChangesOfDay toReturn;

            try
            {
                DocumentParser reader = InitializeReader(dayIndex, element, out bool deleteDocumentAfterWork, out string pathToDocument);
                toReturn = GetChangesFromReader(reader, (dayIndex, groupName, element.Date));

                if (deleteDocumentAfterWork)
                    DeleteChangesDocumentAsync(pathToDocument);
            }

            catch (HttpRequestException)
            {
                ChangesController.Logger?.Log(LogLevel.Error, "Удалённый сервер хранения документов замен (Google Drive) оказался недоступен.");
                toReturn = new();
            }

            catch (Exception exception)
            {
                ChangesController.Logger?.Log(LogLevel.Error, "Произошла ошибка при работе с документом замен: {message}.", exception.Message);
                toReturn = new();
            }
            #endregion

            cachedChanges.Add(new ChangesOfDayCache(toReturn, groupName));
            return toReturn;
        }

        /// <summary>
        /// Метод для получения замен на неделю.
        /// <br/>
        /// <strong>РЕКОМЕНДУЕТСЯ АСИНХРОННОЕ ВЫПОЛНЕНИЕ.</strong>
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Список с объектами, содержащими замены на каждый день.</returns>
        public static List<ChangesOfDay> GetWeekChanges(string groupName)
        {
            List<ChangesOfDay> list = new(1);

            for (int i = 0; i < 7; i++)
            {
                list.Add(GetDayChanges(i, groupName));
            }

            return list;
        }
        #endregion

        #region Область: Внутренние методы.

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
        private static DocumentParser InitializeReader(int dayIndex, ChangeElement element, out bool deleteDocumentAfterWork, out string pathToDocument)
        {
            DateOnly targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());

            #region Подобласть: Проверка кэшированных документов с заменами.

            var cachedDocument = cachedDocuments.Get(doc => doc.DocumentDate.Equals(targetDate));

            if (cachedDocument != null)
            {
                deleteDocumentAfterWork = false;
                pathToDocument = string.Empty;

                return new(cachedDocument);
            }
            #endregion

            #region Подобласть: Получение нового документа с заменами.

            pathToDocument = Helper.TryToDownloadFileFromGoogleDrive(element);
            deleteDocumentAfterWork = true;

            if (!string.IsNullOrEmpty(pathToDocument))
            {
                DocumentParser reader = new(pathToDocument);

                cachedDocuments.Add(reader.CreateCachedValue(targetDate));
                return reader;
            }

            else
            {
                throw new HttpRequestException("При попытке скачать документ произошла ошибка.");
            }
            #endregion
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
                    ChangesController.Logger?.Log(LogLevel.Warning, "Удалить файл с заменами не удалось.");
                }
            });
        }

        /// <summary>
        /// Выполняет работу по получению замен из отправленного экземпляра "DocumentParser".
        /// </summary>
        /// <param name="reader">Экземпляр класса "ChangedReader", с указанным документов для чтения замен.</param>
        /// <param name="requiredData">
        /// Необходимые данные для получения замен:
        /// <list type="number">
        /// <item>dayIndex — Индекс дня для получения замен;</item>
        /// <item>groupName — Название группы для получения замен;</item>
        /// <item>date — Дата, на которую предназначены замены.</item>
        /// </list>
        /// </param>
        /// <returns>Замены, соответствующие указанной дате и группе.</returns>
        private static ChangesOfDay GetChangesFromReader(DocumentParser reader, (int dayIndex, string groupName, DateTime? date) requiredData)
        {
            ChangesOfDay toReturn;

            try
            {
                toReturn = reader.GetOnlyChanges(requiredData.dayIndex.GetDayByIndex(), requiredData.groupName);
                toReturn.ChangesDate = requiredData.date;
                toReturn.ChangesFound = true;
            }

            catch (WrongDayInDocumentException exception)
            {
                ChangesController.Logger?.Log(LogLevel.Information, "При обработке документа обнаружилось несоответствие дат. Очередная ошибка составителей замен?\n" +
                                                                    "{message}.", exception.Message);
                toReturn = new();
            }

            catch (Exception exception)
            {
                ChangesController.Logger?.Log(LogLevel.Error, "Произшла какая-то ошибка, при работе с заменами:\n{message}.", exception.Message);
                toReturn = new();
            }

            return toReturn;
        }
        #endregion
    }
}
