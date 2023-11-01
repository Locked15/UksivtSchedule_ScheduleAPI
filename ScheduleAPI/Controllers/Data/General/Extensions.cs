using NPOI.XWPF.UserModel;
using ScheduleAPI.Models.Elements;
using ScheduleAPI.Models.Elements.Site;
using System.Text;
using System.Text.RegularExpressions;

namespace ScheduleAPI.Controllers.Data.General
{
    /// <summary>
    /// Класс расширений.
    /// </summary>
    public static partial class Extensions
    {
        #region Область: Методы расширения, связанные с датами.

        #region Подобласть: Расширения 'Int32'.

        /// <summary>
        /// Метод расширения, позволяющий получить день по указанному индексу.
        /// </summary>
        /// <param name="index">Индекс дня (0 : 6).</param>
        /// <returns>День, соответствующий данному индексу.</returns>
        /// <exception cref="IndexOutOfRangeException">Введен некорректный индекс.</exception>
        public static string GetDayByIndex(this int index)
        {
            return index switch
            {
                0 => "Понедельник",
                1 => "Вторник",
                2 => "Среда",
                3 => "Четверг",
                4 => "Пятница",
                5 => "Суббота",
                6 => "Воскресенье",

                _ => throw new IndexOutOfRangeException($"Введен некорректный индекс ({index})."),
            };
        }

        /// <summary>
        /// Метод расширения, позволяющий получить полную вариацию даты по указанному индексу дня в пределах текущей недели.
        /// </summary>
        /// <param name="dayIndex">Индекс нужного дня.</param>
        /// <returns>Полная дата.</returns>
        public static DateTime GetDateTimeInWeek(this int dayIndex)
        {
            int currentDayIndex = DateTime.Now.DayOfWeek.GetIndexFromDayOfWeek();
            DateTime current = DateTime.Now;

            while (dayIndex > currentDayIndex)
            {
                current = current.AddDays(1);

                ++currentDayIndex;
            }

            while (dayIndex < currentDayIndex)
            {
                current = current.AddDays(-1);

                --currentDayIndex;
            }

            return current;
        }
        #endregion

        #region Подобласть: Расширения 'String'.

        /// <summary>
        /// Метод расширения, позволяющий получить индекс по названию дня.
        /// </summary>
        /// <param name="day">Название дня.</param>
        /// <returns>День, соответствующий данному индексу.</returns>
        /// <exception cref="ArgumentException">Введен некорректный день.</exception>
        public static int GetIndexByDay(this string day)
        {
            day = day.GetTranslatedDay();
            day = day.ToLower();

            return day switch
            {
                "понедельник" => 0,
                "вторник" => 1,
                "среда" => 2,
                "четверг" => 3,
                "пятница" => 4,
                "суббота" => 5,
                "воскресенье" => 6,

                _ => throw new ArgumentException($"Введен некорректный день ({day})."),
            };
        }

        /// <summary>
        /// Метод расширения, позволяющий перевести название дня на русский язык.
        /// </summary>
        /// <param name="day">Название дня на английском.</param>
        /// <returns>Название дня на русском.</returns>
        public static string GetTranslatedDay(this string day)
        {
            day = day.ToLower();

            return day switch
            {
                "monday" => "Понедельник",
                "tuesday" => "Вторник",
                "wednesday" => "Среда",
                "thursday" => "Четверг",
                "friday" => "Пятница",
                "saturday" => "Суббота",
                "sunday" => "Воскресенье",

                _ => day,
            };
        }

        /// <summary>
        /// Метод расширения, позволяющий получить номер месяца по его названию.
        /// </summary>
        /// <param name="monthName">Название месяца для получения его номера.</param>
        /// <returns>Номер месяца.</returns>
        public static int GetMonthNumber(this string monthName)
        {
            monthName = monthName.ToLower();

            return monthName switch
            {
                "январь" => 1,
                "февраль" => 2,
                "март" => 3,
                "апрель" => 4,
                "май" => 5,
                "июнь" => 6,
                "июль" => 7,
                "август" => 8,
                "сентябрь" => 9,
                "октябрь" => 10,
                "ноябрь" => 11,
                "декабрь" => 12,

                _ => throw new ArgumentException("Отправленное значение некорректно."),
            };
        }
        #endregion

        #region Подобласть: Расширения 'DayOfWeek', 'DateOnly', 'DateTime'.

        /// <summary>
        /// Метод для получения индекса дня из "DayOfWeek".
        /// <br/>
        /// <b>НЕ НУЖНО ИСПОЛЬЗОВАТЬ ПРИВЕДЕНИЕ К INT32</b>, в 'DayOfWeek' другая последовательность дней. 
        /// <br/>
        /// Индекс будет неправильным!
        /// </summary>
        /// <param name="day">Экземпляр 'DayOfWeek' из которого нужно получить индекс.</param>
        /// <returns>Индекс дня недели.</returns>
        public static int GetIndexFromDayOfWeek(this DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => 0,
                DayOfWeek.Tuesday => 1,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday => 3,
                DayOfWeek.Friday => 4,
                DayOfWeek.Saturday => 5,

                _ => 6,
            };
        }

        /// <summary>
        /// Возвращает номер недели для текущей даты. <br />
        /// Это может быть полезно для проверки кэша со следующей недели.
        /// </summary>
        /// <param name="date">Текущая дата.</param>
        /// <returns>Номер недели (начиная с 1).</returns>
        public static int GetWeekNumber(this DateOnly date)
        {
            int result = date.Day / 7;
            if (result == 0)
                result++;

            return result;
        }

        public static DateOnly GetStartOfWeek(this DateOnly date)
        {
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);
            }

            return date;
        }

        public static DateOnly GetEndOfWeek(this DateOnly date)
        {
            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        /// <summary>
        /// Метод расширения, позволяющий получить дату начала недели.
        /// </summary>
        /// <param name="time">Дата, для которой нужно получить дату начала недели.</param>
        /// <returns>Дата начала недели.</returns>
        public static DateTime GetStartOfWeek(this DateTime time)
        {
            while (time.DayOfWeek != DayOfWeek.Monday)
            {
                time = time.AddDays(-1);
            }

            return time;
        }

        /// <summary>
        /// Метод расширения, позволяющий получить дату конца недели.
        /// </summary>
        /// <param name="time">Дата, для которой нужно получить дату конца недели.</param>
        /// <returns>Дата конца недели.</returns>
        public static DateTime GetEndOfWeek(this DateTime time)
        {
            while (time.DayOfWeek != DayOfWeek.Sunday)
            {
                time = time.AddDays(1);
            }

            return time;
        }
        #endregion
        #endregion

        #region Область: Методы расширений, связанные с расписанием.

        /// <summary>
        /// Метод расширения для получения названия подпапки ассетов из названия группы.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Название подпапки (префикс).</returns>
        public static string GetBranchName(this string groupName, List<AffiliationsInfo> affiliateInfo)
        {
            string educationYearEnding = GetEducationYearEnding().ToString()[2..];

            if (groupName.Contains(educationYearEnding) && !groupName.Contains("уКСК", StringComparison.InvariantCultureIgnoreCase))
            {
                return "General";
            }
            else
            {
                return affiliateInfo.First(info =>
                                           info.Affiliations.Any(affiliate =>
                                                                 groupName.Contains(affiliate, StringComparison.InvariantCultureIgnoreCase))
                                          ).BranchName;
            }
        }

        /// <summary>
        /// Возвращает номер года с началом учебного года.
        /// 
        /// Учебный год начинается в сентябре и идёт до конца июня.
        /// Таким образом в случае второго семестра текущий год необходимо уменьшить на 1.
        /// </summary>
        /// <returns></returns>
        private static int GetEducationYearEnding()
        {
            //Если сейчас второй семестр, то первый курс поступал в прошлом году.
            if (DateTime.Now.Month <= 6)
                return DateTime.Now.AddYears(-1).Year;

            //В ином случае, они поступили в этом году.
            else
                return DateTime.Now.Year;
        }

        /// <summary>
        /// Метод расширения для получения подпапки ассетов из названия группы.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Подпапка ассетов.</returns>
        public static string GetAffiliationName(this string groupName)
        {
            StringBuilder subFolderBuilder = new();
            foreach (Match match in AffiliationsInfo.BranchExtractionRegEx().Matches(groupName).Cast<Match>())
            {
                subFolderBuilder.Append(match.Value);
            }

            return subFolderBuilder.ToString().ToUpper();
        }

        /// <summary>
        /// Метод расширения, дублирующий функционал такого же метода у MonthChange, но применяется к списку.
        /// <br/>
        /// Если такой день не найден, будет возвращен <i>"null"</i>.
        /// </summary>
        /// <param name="day">Название дня для поиска.</param>
        /// <returns>Элемент замен с указанным днем.</returns>
        public static ReplacementNodeElement? TryToFindElementByNameOfDayWithoutPreviousWeeks(this List<MonthReplacementsNode> allChanges, string day)
        {
            /* Так как в целях совместимости была использована структура "DateTime", а не "DateOnly", ...
               ... то в дело сравнения вмешивается ещё и время, что может нарушить работу.
               Для исправления потенциальных проблем берутся сдвинутые на 1 день границы.                 */
            DateTime start = DateTime.Now.GetStartOfWeek().AddDays(-1);
            DateTime end = DateTime.Now.GetEndOfWeek().AddDays(1);

            //Месяцы идут в порядке убывания (Декабрь -> Ноябрь -> ...).
            foreach (MonthReplacementsNode monthChanges in allChanges)
            {
                //А вот дни - наоборот, в порядке возрастания, так что их надо инвертировать.
                List<ReplacementNodeElement> changes = monthChanges.Changes.OrderByDescending(change => change.Date).ToList();

                foreach (ReplacementNodeElement change in changes)
                {
                    //Если текущая дата больше конца таргетированной недели, мы ещё не добрались до нужной даты.
                    if (change.Date > end)
                    {
                        continue;
                    }
                    //А если текущая дата МЕНЬШЕ, чем начало таргетированной недели, мы уже её прошли.
                    else if (change.Date < start)
                    {
                        return null;
                    }

                    if (change.DayOfWeek != null && change.DayOfWeek.Equals(day) &&
                        change.CheckContainingChanges())
                    {
                        return change;
                    }
                }
            }

            return null;
        }
        #endregion

        #region Область: Методы расширений, связанные с парсом документа.

        /// <summary>
        /// Внутренний метод, нужный для конвертации нумератора в список.
        /// </summary>
        /// <param name="enumerator">Нумератор параграфов.</param>
        /// <returns>Список, содержащий параграфы.</returns>
        public static List<XWPFParagraph> GetParagraphs(this IEnumerator<XWPFParagraph> enumerator)
        {
            List<XWPFParagraph> paragraphs = new(1);

            while (enumerator.MoveNext())
            {
                paragraphs.Add(enumerator.Current);
            }

            return paragraphs;
        }
        #endregion

        #region Область: Методы расширения, связанные с обработкой входных данных.

        /// <summary>
        /// Метод расширения, позволяющий 'скосить' лишнее значение индекса дня, если оно слишком большое.
        /// </summary>
        /// <param name="dayIndex">Индекс дня.</param>
        /// <returns>Проверенный индекс дня.</returns>
        public static int CheckDayIndexFromOverflow(this int dayIndex)
        {
            // Подготовка индекса, если он некорректен (больше 6 (это воскресенье)):
            while (dayIndex > 6)
            {
                dayIndex -= 7;
            }

            return dayIndex;
        }

        /// <summary>
        /// Метод расширения, удаляющий символы строки из отправленного значения. <br />
        /// В первую очередь предназначен для нормализации названий групп, отправленных в API.
        /// </summary>
        /// <param name="groupName">Строка, в которой будут удалены символы ' и ".</param>
        /// <returns>Новая строка без вхождений указанных элементов.</returns>
        public static string RemoveStringChars(this string groupName)
        {
            groupName = groupName.Replace("\'", string.Empty);
            groupName = groupName.Replace("\"", string.Empty);

            return groupName;
        }
        #endregion
    }
}
