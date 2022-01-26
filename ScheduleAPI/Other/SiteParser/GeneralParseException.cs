/// <summary>
/// Область с классом исключения парса веб-страницы.
/// </summary>
namespace ScheduleAPI.Other.SiteParser
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
        public GeneralParseException(String message) : base(message)
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение исключения.</param>
        /// <param name="inner">Внутреннее исключение?</param>
        public GeneralParseException(String message, Exception? inner) : base(message, inner)
        {

        }
    }
}
