using System.Text;
using NPOI.XWPF.UserModel;
using ScheduleAPI.Controllers.API.Schedule;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Cache.CachedTypes;
using ScheduleAPI.Models.Elements.Schedule;
using ScheduleAPI.Models.Exceptions;

namespace ScheduleAPI.Controllers.Data.Getter.Parsers
{
    /// <summary>
    /// Класс, содержащий логику, нужную для работы с документом с заменами.
    /// </summary>
    public class DocumentParser
    {
        #region Область: Поля.

        /// <summary>
        /// Объект, содержащий документ, который будет прочитан для получения замен.
        /// </summary>
        private readonly XWPFDocument document;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="path">Путь к файлу с заменами.</param>
        /// <exception cref="FileNotFoundException">Файл по указанному адресу не найден.</exception>
        public DocumentParser(string path)
        {
            StreamReader stream = new(path);

            document = new XWPFDocument(stream.BaseStream);
        }

        /// <summary>
        /// Конструктор класса. <br/>
        /// Может быть использован для восстановления документа из кэша.
        /// </summary>
        /// <param name="document">Документ, с которым предстоит работать.</param>
        public DocumentParser(XWPFDocument document)
        {
            this.document = document;
        }
        #endregion

        #region Область: Методы.

        #region Подобласть: Получение только замен.

        /// <summary>
        /// Метод для получения ТОЛЬКО замен на день.
        /// </summary>
        /// <param name="schedule">Расписание на неделю.</param>
        /// <param name="day">Нужный день.</param>
        /// <returns>Замены на выбранный день.</returns>
        public ChangesOfDay GetOnlyChanges(WeekSchedule schedule, string day)
        {
            string groupName = schedule.GroupName;

            return GetOnlyChanges(day, groupName);
        }

        /// <summary>
        /// Метод для получения ТОЛЬКО замен.
        /// </summary>
        /// <param name="day">Название нужного дня.</param>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Объект с заменами на выбранный день.</returns>
        /// <exception cref="WrongDayInDocumentException"></exception>
        public ChangesOfDay GetOnlyChanges(string day, string groupName)
        {
            #region Подобласть: Переменные для проверки групп на "Практику".

            string onPractiseString = "";
            StringBuilder technicalString = new();
            #endregion

            #region Подобласть: Переменные для составления измененного расписания.

            int cellNumber;
            string possibleNumbs;
            Lesson currentLesson;

            bool cycleStopper = false;
            bool changesListen = false;
            bool absoluteChanges = false;

            List<Lesson> newLessons = new List<Lesson>(1);
            #endregion

            #region Подобласть: Список с параграфами.

            List<XWPFParagraph> paragraphs = document.GetParagraphsEnumerator().GetParagraphs();
            #endregion

            // Самым последним параграфом идет имя исполнителя, поэтому его игнорируем:
            for (int i = 0; i < paragraphs.Count - 1; i++)
            {
                string text;

                // Пятым параграфом идет название дня и недели. Проверяем корректность:
                if (i == 5)
                {
                    text = paragraphs[i].Text.ToLower();

                    if (!text.Contains(day.ToLower()))
                    {
                        throw new WrongDayInDocumentException($"Указанный в документе день: {text}.");
                    }
                }

                // Первыми идут параграфы с инициалами администрации, игнорируем:
                else if (i > 5)
                {
                    text = paragraphs[i].Text;

                    technicalString.Append(text);
                }
            }

            /* Порой бывает так, что замен нет, а вот перераспределение кабинетов — да, ...
               ... (оно идет в виде списка кабинетов и групп) поэтому такой случай надо обработать. */
            if (technicalString.ToString().Contains("на практике"))
            {
                // Замены "на практику" всегда идут сверху, так что их индекс всегда 0.
                onPractiseString = technicalString.ToString().Split("на практике")[0].ToLower();
            }

            // Проверяем участие проверяемой группы на "практику":
            if (onPractiseString.Contains(groupName.ToLower()))
            {
                DaySchedule temp = DaySchedule.GetOnPractiseSchedule(day);

                return new ChangesOfDay(true, temp.Lessons);
            }

            // Если группа НЕ на практике, то начинаем проверять таблицу с заменами:
            else
            {
                IEnumerator<XWPFTable> tables = document.GetTablesEnumerator();

                while (tables.MoveNext())
                {
                    XWPFTable currentTable = tables.Current;
                    List<XWPFTableRow> rows = currentTable.Rows;

                    /* Порой в документе бывает несколько таблиц (пример: замены на 17.11.2020), ...
                       ... и тогда таблица с заменами идет второй.                                   */
                    if (!CheckTableIsCorrect(currentTable))
                    {
                        continue;
                    }

                    foreach (XWPFTableRow row in rows)
                    {
                        cellNumber = 0;
                        possibleNumbs = string.Empty;
                        currentLesson = new Lesson();
                        List<XWPFTableCell> cells = row.GetTableCells();

                        foreach (XWPFTableCell cell in cells)
                        {
                            /* Нам нужно 2 варианта строки, один чтобы выполнять сравнения, ...
                               ... а второй — для добавления данных в результат.                */
                            string text = cell.GetText();
                            string lowerText = text.ToLower();

                            // Иногда вместо тире стоит нижнее подчеркивание, обрабатываем случай:
                            text = text.Replace('_', '-');
                            lowerText = lowerText.Replace('_', '-');

                            /* Перед выполнением всех остальных проверок, необходимо ...
                               ... выполнить проверку на пустое содержимое ячейки:       */
                            if (text.Equals(string.Empty))
                            {
                                ++cellNumber;

                                continue;
                            }

                            /* Если мы встретили ячейку, содержащую название ...
                               ... нужной группы, начинаем считывание замен:     */
                            if (lowerText.Equals(groupName.ToLower()))
                            {
                                changesListen = true;

                                //Если замены по центру, то они на весь день.
                                absoluteChanges = cellNumber != 0;
                            }

                            /* Если мы встречаем название другой группы во время чтения замен, ...
                               ... то прерываем цикл:                                              */
                            else if (changesListen && !lowerText.Equals(groupName.ToLower()) &&
                            (cellNumber == 0 || cellNumber == 3))
                            {
                                cycleStopper = true;

                                break;
                            }

                            /* В ином случае мы продолжаем считывать замены, ...
                               ... ориентируясь на текущий номер ячейки:         */
                            else if (changesListen)
                            {
                                switch (cellNumber)
                                {
                                    //Во второй ячейке находится номер пары.
                                    case 1:
                                        possibleNumbs = text;

                                        break;

                                    //В пятой ячейке название новой пары.
                                    case 4:
                                        currentLesson.Name = text;

                                        break;

                                    //В шестой — имя преподавателя.
                                    case 5:
                                        currentLesson.Teacher = text;

                                        break;

                                    //В седьмой — место проведения пары.
                                    case 6:
                                        currentLesson.Place = text;

                                        break;
                                }
                            }

                            // Проверяем участие проверяемой группы на "ликвидацию задолженностей":
                            if (changesListen && lowerText.Contains("ликвидация"))
                            {
                                DaySchedule temp = DaySchedule.GetDebtLiquidationSchedule(day);

                                return new ChangesOfDay(true, temp.Lessons);
                            }

                            ++cellNumber;
                        }

                        /* После завершения чтения замен и удачного их получения, ...
                           ... мы раскрываем их и добавляем в список с заменами:      */
                        if (changesListen && !possibleNumbs.Equals(string.Empty))
                        {
                            newLessons.AddRange(ExpandPossibleLessons(possibleNumbs, currentLesson));
                        }

                        // После прерывания первого цикла, прерываем и второй:
                        if (cycleStopper)
                        {
                            break;
                        }
                    }
                }
            }

            /* После обработки документа необходимо проверить полученные результаты, ...
               ... ведь у группы, возможно, вообще нет замен.                            */
            if (!newLessons.Any())
            {
                // Вызываем конструктор и создаем новый объект, чтобы не затрагивать значения "ChangesOfDay.DefaultChanges".
                return new();
            }

            return new(absoluteChanges, newLessons);
        }
        #endregion

        #region Подобласть: Получение объединенного с заменами расписания.

        /// <summary>
        /// Метод для получения расписания на день с учетом замен.
        /// </summary>
        /// <param name="schedule">Оригинальное расписание на неделю.</param>
        /// <param name="day">День недели для получения расписания с заменами.</param>
        /// <returns>Расписание на день с учетом замен.</returns>
        /// <exception cref="WrongDayInDocumentException">Отправленный день не соответствует дню в документе.</exception>
        public DaySchedule GetDayScheduleWithChanges(WeekSchedule schedule, string day)
        {
            string groupName = schedule.GroupName;
            DaySchedule scheduleOfDay = schedule.DaySchedules[day.GetIndexByDay()];

            return GetDayScheduleWithChanges(day, groupName, scheduleOfDay);
        }

        /// <summary>
        /// Метод для получения расписания на день с учетом замен.
        /// <br/>
        /// <strong>Выполнение метода может занять некоторое время, <i>рекомендуется асинхронное выполнение</i>.</strong>
        /// </summary>
        /// <param name="day">День, на который нужно получить замены.</param>
        /// <param name="groupName">Название группы для получения замен.</param>
        /// <param name="schedule">Оригинальное расписание на день.</param>
        /// <returns>Расписание на день с учетом замен.</returns>
        /// <exception cref="WrongDayInDocumentException">Отправленный день не соответствует дню в документе.</exception>
        public DaySchedule GetDayScheduleWithChanges(string day, string groupName, DaySchedule schedule)
        {
            var changes = GetOnlyChanges(day, groupName);

            return schedule.MergeChanges(changes.NewLessons, changes.AbsoluteChanges);
        }
        #endregion

        #region Подобласть: Кэширование.

        /// <summary>
        /// Создает кэш для документа с заменами.
        /// </summary>
        /// <param name="targetDate">Для упрощения работы дата в документе вынесена в параметр.</param>
        /// <returns>Кэшированное значение.</returns>
        public ChangesDocumentCache CreateCachedValue(DateOnly targetDate)
        {
            var cache = new ChangesDocumentCache(document, targetDate);

            return cache;
        }
        #endregion

        #region Подобласть: Прочие методы.

        /// <summary>
        /// Внутренний метод, позволяющий раскрыть сокращенную запись номеров пар в полный вид.
        /// </summary>
        /// <param name="value">Сокращенный (возможно) вид записи номеров пар.</param>
        /// <param name="lesson">Пара, которая должна быть проведена.</param>
        /// <returns>Полный вид пар.</returns>
        private static List<Lesson> ExpandPossibleLessons(string value, Lesson lesson)
        {
            /* Как показала практика, иногда в конце номеров пар могут оставить лишнюю запятую.
               Например: '1,2,3,' и тогда будет вызвана ошибка. Нужно обработать этот случай и удалить лишние символы. */
            string[] splatted = value.Trim(',').Split(new char[] { ',', '.' });
            List<Lesson> toReturn = new(1);

            /* В отличие от Java, C# способен преобразовать строки с пробелами в целые числа, ...
               ... так что можно сразу переходить в развертке значений.                            */
            foreach (string splattedOne in splatted)
            {
                if (int.TryParse(splattedOne, out int parsed))
                {
                    toReturn.Add(new Lesson(parsed, lesson.Name, lesson.Teacher, lesson.Place));
                }

                else
                {
                    ChangesController.Logger?.Log(LogLevel.Warning, "Не удалось преобразовать значение в числовой тип при раскрытии номеров пар.");
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Метод, нужный для проверки таблицы на то, является ли таблица таблицей с заменами.
        /// </summary>
        /// <param name="table">Логическое значение, отвечающее за корректность таблицы.</param>
        /// <returns></returns>
        private static bool CheckTableIsCorrect(XWPFTable table)
        {
            string temp = table.Text.ToLower();

            /* В таблице, содержащей сами замены всегда есть подобные значения в её оглавлении, ...
               ... но на всякий случай здесь есть возможность замены некоторых оглавлений.          */
            return temp.Contains("группа") && (temp.Contains("заменяемая дисциплина") ||
            temp.Contains("заменяемый преподаватель")) && (temp.Contains("заменяющая дисциплина") ||
            temp.Contains("заменяющий преподаватель")) && temp.Contains("ауд");
        }
        #endregion
        #endregion
    }
}
