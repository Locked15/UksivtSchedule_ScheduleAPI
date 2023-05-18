using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ScheduleAPI.Controllers.Data.Getter;
using ScheduleAPI.Models.Elements;
using ScheduleAPI.Models.Result.Search;

namespace ScheduleAPI.Controllers.API.V1.General
{
    /// <summary>
    /// Контроллер для выполнения различных операций поиска.
    /// </summary>
    [Route("~/api/[controller]")]
    [Route("~/api/v1/[controller]")]
    public class SearchController : Controller
    {
        #region Область: Поля.

        /// <summary>
        /// Содержит в себе объект, необходимый для получения соответствующих данных.
        /// </summary>
        private SearchGetter getter;

        /// <summary>
        /// Содержит объект, необходимый для работы с кэшем сессии. <br />
        /// Конкретнее, для сохранения состояния настроек поиска.
        /// </summary>
        private IMemoryCache cacheWorker;

        /// <summary>
        /// Содержит в себе сведения об окружающей среде.
        /// </summary>
        private IHostEnvironment environment;
        #endregion

        #region Область: Свойства.

        /// <summary>
        /// Содержит в себе объект, необходимый для логгирования.
        /// </summary>
        public static ILogger<SearchController>? Logger { get; private set; }
        #endregion

        #region Область: Инициализаторы.

        /// <summary>
        /// Конструктор класса. <br />
        /// Неявно вызывается при каждом обращении к контроллеру.
        /// </summary>
        /// <param name="cache">Хранилище кэша.</param>
        /// <param name="env">Сведения о локальной среде выполнения.</param>
        /// <param name="logger">Объект, необходимый для выполнения логгирования.</param>
        public SearchController(IMemoryCache cache, IHostEnvironment env, ILogger<SearchController> logger)
        {
            cacheWorker = cache;
            environment = env;

            Logger = logger;
            getter = CreateGetterObject();
        }

        /// <summary>
        /// Создает (или восстанавливает) объект, необходимый для получения данных. <br />
        /// После завершения работы размещает его в локальном хранилище кэша.
        /// </summary>
        /// <returns>Объект, необходимый для работы с данными.</returns>
        private SearchGetter CreateGetterObject()
        {
            if (!cacheWorker.TryGetValue("getter", out SearchGetter? searcher) || searcher == null)
                searcher = new(environment);

            cacheWorker.Set("getter", getter);
            return searcher;
        }
        #endregion

        #region Область: Обработчики API.

        /// <summary>
        /// Обновляет настройки поиска. <br />
        /// Отправленные параметры автоматически будут обернуты в полноценный объект. <br />
        /// Вызов пустого объекта приведет к сборсу параметров до значений по умолчанию.
        /// <br /> <br />
        /// TODO: Обновить под [HttpPatch] или [HttpPut].
        /// </summary>
        /// <param name="options">Объект содержащий настройки поиска.</param>
        /// <returns>Результат установки настроек.</returns>
        [HttpGet]
        [Route("~/api/[controller]/settings")]
        public JsonResult SetSettings(SearchSettings options)
        {
            UpdateGetterSettings(options);
            cacheWorker.Set("getter", getter);

            return new JsonResult("Settings succefully applied.", JsonSettingsModel.JsonOptions);
        }

        /// <summary>
        /// Выполняет запрос на получение списка групп, соответствующих введенному запросу.
        /// </summary>
        /// <param name="request">Строка, которая должна содержаться в названии группы.</param>
        /// <returns>Строковое представление объекта.</returns>
        [HttpGet]
        [Route("~/api/[controller]/groups")]
        public JsonResult GetGroups(string request = "П")
        {
            var result = getter.SearchTargetGroups(request);

            return new JsonResult(result, JsonSettingsModel.JsonOptions);
        }

        /// <summary>
        /// Выполняет запрос на получение списка преподавателей, соответствующих запросу.
        /// </summary>
        /// <param name="request">Строка запроса. ФИО преподавателя должно содержать это значение.</param>
        /// <returns>Строковое представление результата поиска.</returns>
        [HttpGet]
        [Route("~/api/[controller]/teachers")]
        public JsonResult GetTeachers(string request = "Карим")
        {
            var result = getter.SearchTargetTeachers(request);

            return new JsonResult(result, JsonSettingsModel.JsonOptions);
        }
        #endregion

        #region Область: Прочие Функции.

        /// <summary>
        /// Выполняет обновление настроек объекта получения данных.
        /// </summary>
        /// <param name="settings">Новые настройки для поиска.</param>
        [NonAction]
        private void UpdateGetterSettings(SearchSettings settings) =>
                getter.UpdateSettings(settings);

        /// <summary>
        /// Сбрасывает настройки поиска до значений по умолчанию.
        /// </summary>
        [NonAction]
        private void ResetGetterSettings() =>
                getter.UpdateSettings(SearchSettings.DefaultSettings);
        #endregion
    }
}
