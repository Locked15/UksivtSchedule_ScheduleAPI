using System.Text;
using ScheduleAPI.Other.General;

/// <summary>
/// Область с классом, представляющим сущность замен за месяц.
/// </summary>
namespace ScheduleAPI.Other.SiteParser
{
    /// <summary>
    /// Класс замен за месяц.
    /// </summary>
    public class MonthChanges
    {
        #region Область: Свойства.
        /// <summary>
        /// Поле, содержащее название месяца.
        /// </summary>
        public String Month { get; set; }

        /// <summary>
        /// Поле, содержащее список замен на данный месяц.
        /// </summary>
        public List<ChangeElement> Changes { get; set; }
        #endregion

        #region Область: Конструкторы класса.
        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public MonthChanges()
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="month">Название месяца.</param>
        /// <param name="changes">Список замен.</param>
        public MonthChanges(String month, List<ChangeElement> changes)
        {
            Month = month;
            Changes = changes;
        }
        #endregion

        #region Область: Методы.
        /// <summary>
        /// Метод, выполняющий поиск по заменам текущего месяца и возвращающем день с указанным названием.
        /// Выполняет поиск только по текущей неделе, не учитывая замены предыдущих недель.
        /// <br/>
        /// Если такой день не найден, будет возвращен <i>"null"</i>.
        /// </summary>
        /// <param name="day">Название дня для поиска.</param>
        /// <returns>Элемент замен с указанным днем.</returns>
        public ChangeElement TryToFindElementByNameOfDayWithoutPreviousWeeks(String day)
        {
            DateTime start = DateTime.Now.GetStartOfWeek().AddDays(-1);
            DateTime end = DateTime.Now.GetEndOfWeek().AddDays(1);

            Changes = Changes.OrderByDescending(change => change.Date).ToList();

            foreach (ChangeElement change in Changes)
            {
                if (change.Date > end)
                {
                    continue;
                }

                else if (change.Date < start)
                {
                    return null;
                }

                if (change.DayOfWeek.Equals(day) && change.CheckHavingChanges())
                {
                    return change;
                }
            }

            return null;
        }

        /// <summary>
        /// Метод для получения строкового представления объекта.
        /// <br/>
        /// Реализация прямо из Java!
        /// </summary>
        /// <returns>Строковое представление объекта.</returns>
        public override String ToString()
        {
            StringBuilder toReturn = new("\nНовый месяц: \n" +
            "CurrentMonth = " + Month + ";\n" +
            "Changes:" +
            "\n{");

            foreach (ChangeElement change in Changes)
            {
                toReturn.Append(change.ToString("\t"));
            }

            toReturn.Append("}");

            return toReturn.ToString();
        }
        #endregion
    }
}
