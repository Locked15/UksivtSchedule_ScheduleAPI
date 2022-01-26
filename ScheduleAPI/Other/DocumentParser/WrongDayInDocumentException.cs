/// <summary>
/// Область кода с исключением несовпадения дней.
/// </summary>
namespace ScheduleAPI.Other.DocumentParser
{
    /// <summary>
    /// Исключение, возникающее при несовпадении дней в сигнатуре метода и документе.
    /// </summary>
    public class WrongDayInDocumentException : Exception
    {
        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public WrongDayInDocumentException() : base()
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение исключения.</param>
        public WrongDayInDocumentException(String message) : base(message)
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение исключения.</param>
        /// <param name="inner">Внутреннее исключение.</param>
        public WrongDayInDocumentException(String message, Exception inner) : base(message, inner)
        {

        }
    }
}
