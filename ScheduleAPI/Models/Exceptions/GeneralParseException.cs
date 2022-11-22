namespace ScheduleAPI.Models.Exceptions
{
    /// <summary>
    /// Класс, представляющий исключения парса страницы.
    /// </summary>
    public class GeneralParseException : Exception
    {
        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public GeneralParseException() : base()
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение исключения.</param>
        public GeneralParseException(string message) : base(message)
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение исключения.</param>
        /// <param name="inner">Внутреннее исключение?</param>
        public GeneralParseException(string message, Exception? inner) : base(message, inner)
        {

        }
    }
}
