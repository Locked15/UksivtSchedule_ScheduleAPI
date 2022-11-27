namespace ScheduleAPI.Controllers.Other.General
{
    /// <summary>
    /// Класс-помощник, нужный для различных задач.
    /// </summary>
    public static class Helper
    {
        #region Область: Методы.

        /// <summary>
        /// Вычисляет путь к директории проекта (директория, отображаемая "Обозревателем решений"). <br />
        /// </summary>
        /// <returns>Путь к директории проекта.</returns>
        public static string GetSiteRootFolderPath()
        {
            string basicAppPath;

            // При разработке приложения используется опция сборки "Debug", так что будет исполняться этот код.
#if DEBUG
            basicAppPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? string.Empty;
#endif

            // На сервере приложение работает под опцией сборки "Release", так что будет выполняться данный код.
#if RELEASE
            basicAppPath = AppDomain.CurrentDomain.BaseDirectory;
#endif

            return basicAppPath;
        }

        /// <summary>
        /// Проверяет две даты на соответствие дней недели, с учетом следующих недель (и начала следующего месяца).
        /// </summary>
        /// <param name="first">Первая дата для проверки (потенциально прошлая/текущая).</param>
        /// <param name="second">Вторая дата для проверки (потенциально текущая/будущая).</param>
        /// <returns>Результат проверки на идентичность.</returns>
        public static bool CheckDaysToEqualityIncludingFutureDates(DateOnly first, DateOnly second)
        {
            bool isFuture = first.GetWeekNumber() < second.GetWeekNumber() || 
                            first.Month < second.Month;
            bool daysAreEqual = first.DayOfWeek == second.DayOfWeek;

            return first == second || (isFuture && daysAreEqual);
        }
        #endregion
    }
}
