namespace ScheduleAPI.Models.Exceptions.Data
{
    public class GroupNotFoundException : Exception
    {
        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public GroupNotFoundException() : base()
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение исключения.</param>
        public GroupNotFoundException(string message) : base(message)
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="message">Сообщение исключения.</param>
        /// <param name="inner">Внутреннее исключение.</param>
        public GroupNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
