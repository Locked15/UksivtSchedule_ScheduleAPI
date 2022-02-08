using System.Text;
using NPOI.XWPF.UserModel;
using ScheduleAPI.Models;
using ScheduleAPI.Other.General;
using Bool = System.Boolean;

/// <summary>
/// Область кода с парсером документа с заменами.
/// </summary>
namespace ScheduleAPI.Other.DocumentParser
{
    /// <summary>
    /// Класс, содержащий логику, нужную для работы с документом с заменами.
    /// </summary>
    public class ChangesReader
    {
        #region Область: Поля.
        private XWPFDocument document;
        #endregion

        #region Область: Конструктор.
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="path">Путь к файлу с заменами.</param>
        /// <exception cref="FileNotFoundException">Файл по указанному адресу не найден.</exception>
        public ChangesReader(String path)
        {
            StreamReader stream = new(path);

            document = new XWPFDocument(stream.BaseStream);
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
        public ChangesOfDay GetOnlyChanges(WeekSchedule schedule, String day)
        {
            String groupName = schedule.GroupName;

            return GetOnlyChanges(day, groupName);
        }

        /// <summary>
        /// Метод для получения ТОЛЬКО замен.
        /// </summary>
        /// <param name="day">Название нужного дня.</param>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Объект с заменами на выбранный день.</returns>
        /// <exception cref="WrongDayInDocumentException"></exception>
        public ChangesOfDay GetOnlyChanges(String day, String groupName)
        {
            #region Подобласть: Переменные для проверки групп на "Практику".
            String onPractiseString = "";
            StringBuilder technicalString = new StringBuilder();
            #endregion

            #region Подобласть: Переменные для составления измененного расписания.
            Int32 cellNumber;
            String possibleNumbs;
            Lesson currentLesson;
            Bool cycleStopper = false;
            Bool changesListen = false;
            Bool absoluteChanges = false;
            List<Lesson> newLessons = new List<Lesson>(1);
            #endregion

            #region Подобласть: Список с параграфами.
            List<XWPFParagraph> paragraphs = document.GetParagraphsEnumerator().GetParagraphs();
            #endregion

            //Самым последним параграфом идет имя исполнителя, поэтому его игнорируем:
            for (int i = 0; i < paragraphs.Count - 1; i++)
            {
                String text;

                //Пятым параграфом идет название дня и недели. Проверяем корректность:
                if (i == 5)
                {
                    text = paragraphs[i].Text.ToLower();

                    if (!text.Contains(day.ToLower()))
                    {
                        throw new WrongDayInDocumentException("День отправленного расписания и документа с заменами не совпадают.");
                    }
                }

                //Первыми идут параграфы с инициалами администрации, игнорируем:
                else if (i > 5)
                {
                    text = paragraphs[i].Text;

                    technicalString.Append(text);
                }
            }

            /* Порой бывает так, что замен нет, а вот перераспределение кабинетов — да, ...
               ... (оно идет в виде списка кабинетов и групп) поэтому такой случай надо обработать. */
            if (technicalString.ToString().Contains("– на практике"))
            {
                //Замены "на практику" всегда идут сверху, так что их индекс всегда 0.
                onPractiseString = technicalString.ToString().Split("– на практике")[0].ToLower();
            }

            //Проверяем участие проверяемой группы на "практику":
            if (onPractiseString.Contains(groupName.ToLower()))
            {
                DaySchedule temp = DaySchedule.GetOnPractiseSchedule(day);

                return new ChangesOfDay(true, temp.Lessons);
            }

            //Если группа НЕ на практике, то начинаем проверять таблицу с заменами:
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
                        possibleNumbs = String.Empty;
                        currentLesson = new Lesson();
                        List<XWPFTableCell> cells = row.GetTableCells();

                        foreach (XWPFTableCell cell in cells)
                        {
                            /* Нам нужно 2 варианта строки, один чтобы выполнять сравнения, ...
                               ... а второй — для добавления данных в результат.                */
                            String text = cell.GetText();
                            String lowerText = text.ToLower();

                            //Иногда вместо тире стоит нижнее подчеркивание, обрабатываем случай:
                            text = text.Replace('_', '-');
                            lowerText = lowerText.Replace('_', '-');

                            /* Перед выполнением всех остальных проверок, необходимо ...
                               ... выполнить проверку на пустое содержимое ячейки:       */
                            if (text.Equals(String.Empty))
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

                            ++cellNumber;
                        }

                        /* После завершения чтения замен и удачного их получения, ...
                           ... мы раскрываем их и добавляем в список с заменами:      */
                        if (changesListen && !possibleNumbs.Equals(String.Empty))
                        {
                            newLessons.AddRange(ExpandPossibleLessons(possibleNumbs, currentLesson));
                        }

                        //После прерывания первого цикла, прерываем и второй:
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
                // Вызываем конструтор и создаем новый объект, чтобы не затрагивать значения "ChangesOfDay.DefaultChanges".
                return new();
            }

            return new(absoluteChanges, newLessons);
        }
        #endregion

        #region Подобласть: Получение слитого с заменами расписания.
        /// <summary>
        /// Метод для получения расписания на день с учетом замен.
        /// </summary>
        /// <param name="schedule">Оригинальное расписание на неделю.</param>
        /// <param name="day">День недели для получения расписания с заменами.</param>
        /// <returns>Расписание на день с учетом замен.</returns>
        /// <exception cref="WrongDayInDocumentException">Отправленный день несоответствует дню в документе.</exception>
        public DaySchedule GetDayScheduleWithChanges(WeekSchedule schedule, String day)
        {
            String groupName = schedule.GroupName;
            DaySchedule scheduleOfDay = schedule.Days[day.GetIndexByDay()];

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
        /// <exception cref="WrongDayInDocumentException">Отправленный день несоответствует дню в документе.</exception>
        public DaySchedule GetDayScheduleWithChanges(String day, String groupName, DaySchedule schedule)
        {
            #region Подобласть: Переменные для проверки групп на "Практику".
            String onPractiseString = "";
            StringBuilder technicalString = new StringBuilder();
            #endregion

            #region Подобласть: Переменные для составления измененного расписания.
            Int32 cellNumber;
            String possibleNumbs;
            Lesson currentLesson;
            Bool cycleStopper = false;
            Bool changesListen = false;
            Bool absoluteChanges = false;
            List<Lesson> newLessons = new List<Lesson>(1);
            #endregion

            #region Подобласть: Список с параграфами.
            List<XWPFParagraph> paragraphs = document.GetParagraphsEnumerator().GetParagraphs();
            #endregion

            //Самым последним параграфом идет имя исполнителя, поэтому его игнорируем:
            for (int i = 0; i < paragraphs.Count - 1; i++)
            {
                String text;

                //Пятым параграфом идет название дня и недели. Проверяем корректность:
                if (i == 5)
                {
                    text = paragraphs[i].Text.ToLower();

                    if (!text.Contains(day.ToLower()))
                    {
                        throw new WrongDayInDocumentException("День отправленного расписания и документа с заменами не совпадают.");
                    }
                }

                //Первыми идут параграфы с инициалами администрации, игнорируем:
                else if (i > 5)
                {
                    text = paragraphs[i].Text;

                    technicalString.Append(text);
                }
            }

            /* Порой бывает так, что замен нет, а вот перераспределение кабинетов — да, ...
               ... (оно идет в виде списка кабинетов и групп) поэтому такой случай надо обработать. */
            if (technicalString.ToString().Contains("– на практике"))
            {
                //Замены "на практику" всегда идут сверху, так что их индекс всегда 0.
                onPractiseString = technicalString.ToString().Split("– на практике")[0].ToLower();
            }

            //Проверяем участие проверяемой группы на "практику":
            if (onPractiseString.Contains(groupName.ToLower()))
            {
                return DaySchedule.GetOnPractiseSchedule(day);
            }

            //Если группа НЕ на практике, то начинаем проверять таблицу с заменами:
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
                        possibleNumbs = String.Empty;
                        currentLesson = new Lesson();
                        List<XWPFTableCell> cells = row.GetTableCells();

                        foreach (XWPFTableCell cell in cells)
                        {
                            /* Нам нужно 2 варианта строки, один чтобы выполнять сравнения, ...
                               ... а второй — для добавления данных в результат.                */
                            String text = cell.GetText();
                            String lowerText = text.ToLower();

                            //Иногда вместо тире стоит нижнее подчеркивание, обрабатываем случай:
                            text = text.Replace('_', '-');
                            lowerText = lowerText.Replace('_', '-');
                           
                            /* Перед выполнением всех остальных проверок, необходимо ...
                               ... выполнить проверку на пустое содержимое ячейки:       */
                            if (text.Equals(String.Empty))
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

                            ++cellNumber;
                        }

                        /* После завершения чтения замен и удачного их получения, ...
                           ... мы раскрываем их и добавляем в список с заменами:      */
                        if (changesListen && !possibleNumbs.Equals(String.Empty))
                        {
                            newLessons.AddRange(ExpandPossibleLessons(possibleNumbs, currentLesson));
                        }

                        //После прерывания первого цикла, прерываем и второй:
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
                return schedule;
            }

            //Если же замены есть, выполняем слияние и возвращаем результат:
            return schedule.MergeChanges(newLessons, absoluteChanges);
        }
        #endregion

        #region Подобласть: Прочие методы.
        /// <summary>
        /// Внутренний метод, позволяющий раскрыть сокращенную запись номеров пар в полный вид.
        /// </summary>
        /// <param name="value">Сокращенный (возможно) вид записи номеров пар.</param>
        /// <param name="lesson">Пара, которая должна быть проведена.</param>
        /// <returns>Полный вид пар.</returns>
        private List<Lesson> ExpandPossibleLessons(String value, Lesson lesson)
        {
            String[] splatted = value.Split(",");
            List<Lesson> toReturn = new List<Lesson>(1);

            /* В отличие от Java, C# способен преобразовать строки с пробелами в целые числа, ...
               ... так что можно сразу переходить в развертке значений.                            */
            foreach (String s in splatted)
            {
                toReturn.Add(new Lesson(Int32.Parse(s), lesson.Name, lesson.Teacher, lesson.Place));
            }

            return toReturn;
        }

        /// <summary>
        /// Метод, нужный для проверки таблицы на то, является ли таблица таблицей с заменами.
        /// </summary>
        /// <param name="table">Логическое значение, отвечающее за корректность таблицы.</param>
        /// <returns></returns>
        private Bool CheckTableIsCorrect(XWPFTable table)
        {
            String temp = table.Text.ToLower();

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
