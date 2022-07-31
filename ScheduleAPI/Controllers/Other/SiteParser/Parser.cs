using AngleSharp;
using AngleSharp.Dom;
using ScheduleAPI.Controllers.Other.General;

namespace ScheduleAPI.Controllers.Other.SiteParser
{
    /// <summary>
    /// Класс, представляющий сущность парсера и его логику.
    /// </summary>
    public class Parser
    {
        #region Область: Поля.

        /// <summary>
        /// Внутреннее поле, содержащее всю Web-страницу с заменами.
        /// </summary>
        private Document webPage;
        #endregion

        #region Область: Константы.

        /// <summary>
        /// Внутренняя константа, содержащая CSS-селектор для парса страницы.
        /// </summary>
        private const string selector = "section > div > div > div > div > div > div" +
        " > div > div > div > div > div > div > div > div > div > div > div";

        /// <summary>
        /// Внутренняя константа, содержащая путь к странице с заменами.
        /// </summary>
        private const string webPagePath = "https://www.uksivt.ru/zameny";

        /// <summary>
        /// Внутренняя константа, содержащая неразрывный пробел.
        /// </summary>
        private const string nonBreakSpace = "\u00A0";
        #endregion

        #region Область: Конструктор класса.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <exception cref="Exception">Общее исключение (отсутствие подключения к сети).</exception>
        public Parser()
        {
            webPage = (Document)BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(webPagePath).Result;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод для получения списка возможных дней для просмотра замен.
        /// <br/>
        /// <strong>Выполнение занимает много времени, вызов стоит выполнять асинхронно.</strong>
        /// </summary>
        /// <param name="limit">Количество месяцев, которые нужно получить.</param>
        /// <returns>Список с доступными заменами по месяцам.</returns>
        /// <exception cref="GeneralParseException">Общее исключение парса страницы.</exception>
        public List<MonthChanges> ParseAvailableNodes(int limit = 2)
        {
            #region Подобласть: Переменные считывания доступных замен.

            int i = 0;
            int monthCounter = 0;
            int currentYear = DateTime.Now.Year;
            string currentMonth = "Январь";

            List<ChangeElement> changes = new(30);
            List<MonthChanges> monthChanges = new(2);
            #endregion

            #region Подобласть: Переменные парса веб-страницы.

            IElement? generalChange = webPage.QuerySelector(selector);
            List<IElement>? listOfChanges = generalChange?.Children.ToList();
            #endregion

            //Совершаем проверку на возможные ошибки:
            if (generalChange == null || listOfChanges == null)
            {
                if (generalChange == null)
                {
                    Logger.WriteError(4, $"GeneralChange был \'null\', а селектор: {selector}.");
                }

                if (listOfChanges == null)
                {
                    Logger.WriteError(4, $"ListOfChanges был \'null\', а селектор: {selector}.");
                }

                throw new GeneralParseException("В процессе обработки страницы произошла ошибка.");
            }

            //Если же ошибок получения данных нет, начинаем парсить страницу:
            foreach (IElement? element in listOfChanges)
            {
                string text = element.Text();
                string nodeName = element.NodeName.ToLower();

                //Обработка отступов с неразрывными пробелами:
                if (nodeName.Equals("p") && !text.Equals(nonBreakSpace))
                {
                    //В первой итерации программа также зайдет сюда, обрабатываем этот случай:
                    if (changes.Any())
                    {
                        monthChanges.Add(new MonthChanges(currentMonth, changes));
                    }

                    changes = new(30);
                    currentMonth = text.Substring(0, text.LastIndexOf(' ')).Replace(nonBreakSpace, "");
                    currentYear = int.Parse(text.Substring(text.LastIndexOf(' ')).Replace(nonBreakSpace, ""));

                    i = 1;
                }

                //Если мы встречаем тег "table", то мы дошли до таблицы с заменами на какой-либо месяц.
                else if (nodeName.Equals("table") && monthCounter < limit)
                {
                    /* Первым тегом таблицы всегда идет "<thead>", определяющий заголовок, ...
                       ... а следом за ним — "<tbody>", определяющий тело таблицы. Он нам и нужен. */
                    IElement tableBody = element.Children[1];
                    List<IElement> tableRows = tableBody.Children.ToList();

                    for (int j = 0; j < tableRows.Count; j++)
                    {
                        int dayCounter = 0;
                        bool firstIteration = true;
                        IElement currentRow = tableRows[j];

                        //В первой строке содержатся ненужные значения, пропускаем:
                        if (j == 0)
                        {
                            continue;
                        }

                        //В иных случаях начинаем итерировать ячейки таблицы:
                        foreach (IElement tableCell in currentRow.Children)
                        {
                            string cellText = tableCell.Text();

                            /* Первая ячейка, видимо для отступа, содержит неразрывный пробел, так что ...
                               ... пропускаем такую итерацию.
                               Кроме того, если 1 число месяца выпадает не на понедельник, то ...
                               ... некоторое количество ячеек также будет пустым.                          */
                            if (cellText.Equals(nonBreakSpace))
                            {
                                if (!firstIteration)
                                {
                                    ++dayCounter;
                                }

                                firstIteration = false;
                                continue;
                            }

                            //В некоторых ячейках (дни без замен) нет содержимого, так что учитываем это.
                            if (tableCell.Children.Length < 1)
                            {
                                changes.Add(new ChangeElement(new DateTime(currentYear, currentMonth.GetMonthNumber(), i),
                                dayCounter.GetDayByIndex(), null));
                            }

                            //В ином случае замены есть и нам нужно получить дочерний элемент.
                            else
                            {
                                IElement? link = tableCell.FirstElementChild;

                                //На всякий случай обрабатываем возможную ошибку с получением атрибута:
                                changes.Add(new ChangeElement(new DateTime(currentYear, currentMonth.GetMonthNumber(), i),
                                dayCounter.GetDayByIndex(), link?.GetAttribute("href") ?? String.Empty));
                            }

                            ++i;
                            ++dayCounter;
                        }
                    }

                    ++monthCounter;
                }
            }

            return monthChanges;
        }
        #endregion
    }
}
