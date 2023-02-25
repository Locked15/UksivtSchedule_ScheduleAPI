using ScheduleAPI.Controllers.Data.Getter.Schedule;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Result.Search;
using ScheduleAPI.Models.Result.Search.Result;

namespace ScheduleAPI.Controllers.Data.Getter
{
    /// <summary>
    /// Getter-класс для получения данных по поиску.
    /// </summary>
    public class SearchGetter
    {
        #region Область: Поля.

        /// <summary>
        /// Объект, содержащий сведения о локальном окружении.
        /// </summary>
        private IHostEnvironment environment;
        #endregion

        #region Область: Свойства.

        /// <summary>
        /// Объект, содержащий настройки поиска.
        /// </summary>
        public SearchSettings Settings { get; private set; }

        /// <summary>
        /// Свойство-обертка, содержащее объект для сравнения строк. <br />
        /// В зависимости от настроек может возвращать разыные объекты для выполнения сравнения.
        /// </summary>
        private StringComparison StringComparer
        {
            get
            {
                return Settings.CaseSensitive ? StringComparison.CurrentCulture :
                                                StringComparison.CurrentCultureIgnoreCase;
            }
        }
        #endregion

        #region Область: Инициализаторы.

        /// <summary>
        /// Конструктор класса. <br />
        /// Устанавливает настройки по умолчанию.
        /// </summary>
        /// <param name="env">Сведения об окружающей среде приложения.</param>
        public SearchGetter(IHostEnvironment env)
        {
            environment = env;
            Settings = SearchSettings.DefaultSettings;
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="env">Сведения об окружающей среде приложения.</param>
        /// <param name="options">Настройки поиска.</param>
        public SearchGetter(IHostEnvironment env, SearchSettings options) : this(env)
        {
            Settings = options;
        }

        /// <summary>
        /// Обновляет настройки поиска.
        /// </summary>
        /// <param name="options">Новые настройки.</param>
        public void UpdateSettings(SearchSettings options)
        {
            Settings = options;
        }
        #endregion

        #region Область: Функции.

        /// <summary>
        /// Выполняет поиск группы по указанному запросу.
        /// </summary>
        /// <param name="request">Строковое значение, которое должна содержать группа.</param>
        /// <returns>Результат поиска.</returns>
        public SearchForGroupResult SearchTargetGroups(string request)
        {
            var allGroups = GetGroupsByRequest(request);

            return new SearchForGroupResult(allGroups);
        }

        /// <summary>
        /// Создает соответствующие запросы к Getter-классу для получения данных. <br />
        /// Затем фильтрует их согласно указанным настройкам.
        /// </summary>
        /// <param name="request">Значение, которое должно содержаться в названии группы.</param>
        /// <returns>Список подходящих групп.</returns>
        private List<string> GetGroupsByRequest(string request)
        {
            var getter = new AssetGetter(environment);
            var groups = getter.GetAllAvailableGroups();

            request = PrepareRequestBySettings(request);
            return groups.Where(group =>
                                group.Contains(request, StringComparer)).ToList();
        }

        /// <summary>
        /// Выполняет поиск преподавателя по указанному запросу.
        /// </summary>
        /// <param name="request">Строковое значение, которое должно содержаться в ФИО преподавателя.</param>
        /// <returns>Результат поиска.</returns>
        public SearchForTeacherResult SearchTargetTeachers(string request)
        {
            var teachers = GetTeachersByRequest(request);

            return new SearchForTeacherResult(teachers);
        }

        /// <summary>
        /// Создает соответствующие запросы к Getter-классу для получения данных. <br />
        /// Затем фильтрует их согласно указанным настройкам.
        /// </summary>
        /// <param name="request">Значение, которое должно содержаться в ФИО преподавателя.
        /// Стоит учитывать, что зачастую ФИО указывается не полностью: Имя и Отчество сокращаются по первой букве.</param>
        /// <returns>Полный список всех преподавателей, соответствующих запросу.</returns>
        private List<string> GetTeachersByRequest(string request)
        {
            var getter = new AssetGetter(environment);
            var teachers = getter.GetAllTeachers();

            request = PrepareRequestBySettings(request);
            return teachers.Where(teacher =>
                                  teacher.Contains(request, StringComparer)).ToList();
        }

        #region Подобласть: Общие Функции.

        /// <summary>
        /// Выполняет подготовку оригинального запроса по настроенным параметрам.
        /// </summary>
        /// <param name="originalRequest">Оригинальный запрос поиска.</param>
        /// <returns>Подготовленный и соответствующий параметрам запрос.</returns>
        private string PrepareRequestBySettings(string originalRequest)
        {
            var request = originalRequest;
            if (Settings.NormalizeRequest)
                request = request.RemoveStringChars();

            return request;
        }
        #endregion
        #endregion
    }
}
