using System.Data.SqlClient;

namespace ScheduleAPI.Controllers.Other.General
{
    /// <summary>
    /// Класс-логгер, нужный для записи ошибок API в БД.
    /// <br/><br/>
    /// <i>
    /// Приоритет основных ошибок:
    /// <br/>
    /// 1 — Ошибка, связанная с получением расписания;
    /// <br/>
    /// 2 — Ошибка, связанная с получением замен;
    /// <br/>
    /// 3 — Ошибка, связанная с подключением к БД.
    /// <br/>
    /// 4 — Ошибка, связанная с парсом страницы.
    /// </i>
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Метод для записи ошибки в БД.
        /// </summary>
        /// <param name="priority">Приоритет ошибки.</param>
        public static void WriteError(int priority)
        {
            string message;

            if (priority == 1)
            {
                message = "Critical Error Occurred! Fix It ASAP.";
            }

            else if (priority <= 3)
            {
                message = "Important Error Occurred. Need Attention.";
            }

            else
            {
                message = "Common Error Occurred. Fix It When Get Time.";
            }

            WriteError(priority, message);
        }

        /// <summary>
        /// Метод для записи ошибки в БД.
        /// </summary>
        /// <param name="priority">Приоритет ошибки.</param>
        /// <param name="message">Текст ошибки.</param>
        public static void WriteError(int priority, string message)
        {
            WriteError(priority, message, DateTime.Now);
        }

        /// <summary>
        /// Метод для записи ошибки в БД.
        /// </summary>
        /// <param name="priority">Приоритет ошибки.</param>
        /// <param name="message">Текст ошибки.</param>
        /// <param name="time">Дата возникновения ошибки.</param>
        public static void WriteError(int priority, string message, DateTime time)
        {
            SqlConnection connect = DataBaseConnector.Connection;
            SqlCommand command = new("INSERT INTO Error_Logs(Error_Priority, Error_Data, Error_DateTime)" +
                                     $"VALUES(@priority, @message, @time)", connect);
            command.Parameters.AddWithValue("@priority", priority);
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@time", time);

            try
            {
                connect.Open();

                command.ExecuteNonQuery();

                connect.Close();
            }

            catch
            {
                connect.Close();
            }
        }
    }
}
