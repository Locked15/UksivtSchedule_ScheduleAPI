using System.Data.SqlClient;

namespace ScheduleAPI.Controllers.Other.General
{
    /// <summary>
    /// Класс, содержащий глобальную переменную строки подключения.
    /// </summary>
    public class DataBaseConnector
    {
        #region Область: Свойства.

        /// <summary>
        /// Свойство, содержащее подключение к Базе Данных.
        /// </summary>
        public static SqlConnection Connection { get; private set; }
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Статический конструктор класса.
        /// </summary>
        static DataBaseConnector()
        {
            Connection = new SqlConnection("Server=tcp:uksivtschedule.database.windows.net, 1433; " +
                                           "Initial Catalog=ScheduleDataRu; " +
                                           "Persist Security Info=False; " +
                                           "User ID=Scheduler; " +
                                           "Password=Uksivt_22; " +
                                           "MultipleActiveResultSets=False; " +
                                           "Encrypt=True; " +
                                           "TrustServerCertificate=False; " +
                                           "Connection Timeout=30; " +
                                           "Integrated Security=False;");
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод для проверки корректности подключения.
        /// </summary>
        /// <returns>Значение, отвечающее за корректность подключения.</returns>
        private static bool CheckConnection()
        {
            try
            {
                Connection.Open();
                Connection.Close();

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
