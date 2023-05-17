using Bool = System.Boolean;

namespace ScheduleAPI.Models.Elements.Site
{
    /// <summary>
    /// Класс, представляющий один элемент с заменами.
    /// </summary>
    public class ReplacementNodeElement
    {
        #region Область: Свойства.

        /// <summary>
        /// Свойство, содержащее число месяца, на которое идут замены.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Свойство, содержащее название дня недели, на который идут замены.
        /// </summary>
        public string? DayOfWeek { get; set; }

        /// <summary>
        /// Свойство, содержащее ссылку на документ с заменами.
        /// </summary>
        public string? LinkToDocument { get; set; }
        #endregion

        #region Область: Конструкторы класса.

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public ReplacementNodeElement()
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="dayOfMonth">День месяца (число).</param>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="linkToDocument">Ссылка на документ с заменами.</param>
        public ReplacementNodeElement(DateTime date, string? dayOfWeek, string? linkToDocument)
        {
            Date = date;
            DayOfWeek = dayOfWeek;
            LinkToDocument = linkToDocument;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод, позволяющий проверить определенный день на наличие замен.
        /// </summary>
        /// <returns>Логическое значение, отвечающее за наличие/отсутствие замен.</returns>
        public Bool CheckContainingChanges()
        {
            return LinkToDocument != null;
        }

        /// <summary>
        /// Собственный метод для получения строкового представления объекта.
        /// </summary>
        /// <param name="append">Строка для дополнительной вставки.</param>
        /// <returns>Строковое представление объекта.</returns>
        public string ToString(string append)
        {
            //Реализация прямиком из Java:
            return "\n" + append + "ReplacementNodeElement: \n" +
            append + "DayOfMonth = " + Date.Day + ";\n" +
            append + "LinkToDocument = " + LinkToDocument + ";\n" +
            append + "CurrentDay = " + DayOfWeek + ".\n";
        }
        #endregion
    }
}
