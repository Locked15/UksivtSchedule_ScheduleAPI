using System.Data.SqlClient;
using Bool = System.Boolean;

namespace ScheduleAPI.Other
{
    /// <summary>
    /// Класс, содержащий глобальную переменную строки подключения.
    /// </summary>
    public class DataBaseConnector
    {
        #region Область: Поля.
        /// <summary>
        /// Поле, содержащее подключение к БД.
        /// </summary>
        private static SqlConnection connection;

        /// <summary>
        /// Поле, отвечающее за то, инициализирована ли глобальная конфигурация.
        /// </summary>
        private static Bool initialized = false;
        #endregion

        #region Область: Свойства.
        /// <summary>
        /// Свойство, содержащее конфигурацию приложения.
        /// </summary>
        public static SqlConnection Connection
        {
            get
            {
                if (initialized)
                {
                    return connection;
                }

                throw new Exception("Попытка получить доступ к конфигурации до инициализации.");
            }
        }
        #endregion

        #region Область: Методы.
        /// <summary>
        /// Метод для инициализации строки подключения.
        /// </summary>
        /// <param name="configuration">Экземпляр конфигурации.</param>
        public static void Initialize(IConfiguration configuration)
        {
            if (!initialized)
            {
                connection = new(configuration.GetConnectionString("UksivtScheduleConnect"));

                if (CheckConnection())
                {
                    initialized = true;
                }
            }

            else
            {
                Logger.WriteError(5, "Повторная инициализация конфигурации.");
            }
        }

        /// <summary>
        /// Метод для проверки корректности подключения.
        /// </summary>
        /// <returns>Значение, отвечающее за корректность подключения.</returns>
        private static Bool CheckConnection()
        {
            try
            {
                connection.Open();
                connection.Close();

                return true;
            }

            catch
            {
                return false;
            }
        }
        #endregion
    }
}
