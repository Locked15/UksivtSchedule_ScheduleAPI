using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Other;
using ScheduleAPI.Models;
using ScheduleAPI.Other.General;
using ScheduleAPI.Models.Getter;

namespace ScheduleAPI.Controllers
{
    /// <summary>
    /// Класс-контроллер получения расписания через ассеты.
    /// </summary>
    [Obsolete("Аварийный контроллер. Получает данные из ассетов, не задействуя БД.")]
    [Route("api/day/[controller]")]
    [ApiController]
    public class ScheduleDayAssetController : Controller
    {
        #region Область: Поля.
        /// <summary>
        /// Поле, содержащее объект, нужный для получения расписания из ассетов.
        /// </summary>
        private readonly AssetGetter getter;
        #endregion

        #region Область: Конструктор.
        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <param name="environment">Данные о окружении приложения.</param>
        public ScheduleDayAssetController(IConfiguration configuration, IHostEnvironment environment)
        {
            DataBaseConnector.Initialize(configuration);

            getter = new(environment);
        }
        #endregion

        #region Область: Методы.
        /// <summary>
        /// Метод, реализующий Get-запрос на получение расписания.
        /// </summary>
        /// <param name="dayIndex">Индекс дня для получения расписания.
        /// <br/>
        /// Значение по умолчанию: 0.</param>
        /// <param name="groupName">Название группы для получения расписания.
        /// <br/>
        /// Значение по умолчанию: 19П-3.</param>
        /// <returns>Json-объект, содержащий расписание для указанной группы в указанный день.</returns>
        [HttpGet]
        [Obsolete("Аварийный контроллер. Позволяет получать данные из ассетов, вместо БД.")]
        public String Get(Int32 dayIndex = 0, String groupName = "19П-3")
        {
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            DaySchedule schedule = getter.GetDaySchedule(dayIndex, groupName);

            return SerializeFormatter.ConvertToUnformattedJsonForm(schedule);
        }
        #endregion
    }

    /// <summary>
    /// Класс-контроллер получения расписания через ассеты.
    /// <br/>
    /// Позволяет получить расписание на неделю.
    /// </summary>
    [Obsolete("Аварийный контроллер. Получает данные из ассетов, не задействуя БД.")]
    [Route("api/week/[controller]")]
    [ApiController]
    public class ScheduleWeekAssetController : Controller
    {
        #region Область: Поля.
        /// <summary>
        /// Поле, содержащее объект, нужный для получения расписания из ассетов.
        /// </summary>
        private readonly AssetGetter getter;
        #endregion

        #region Область: Конструктор.
        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <param name="environment">Данные о окружении приложения.</param>
        public ScheduleWeekAssetController(IConfiguration configuration, IHostEnvironment environment)
        {
            DataBaseConnector.Initialize(configuration);

            getter = new(environment);
        }
        #endregion

        #region Область: Методы.
        /// <summary>
        /// Метод, реализующий Get-запрос на получение расписания.
        /// </summary>
        /// <param name="groupName">Название группы для получения расписания.
        /// <br/>
        /// Значение по умолчанию: 19П-3.</param>
        /// <returns>Json-объект, содержащий расписание для указанной группы в указанный день.</returns>
        [HttpGet]
        [Obsolete("Аварийный контроллер. Позволяет получать данные из ассетов, вместо БД.")]
        public String Get(String groupName = "19П-3")
        {
            WeekSchedule schedule = getter.GetWeekSchedule(groupName);

            return SerializeFormatter.ConvertToUnformattedJsonForm(schedule);
        }
        #endregion
    }
}
