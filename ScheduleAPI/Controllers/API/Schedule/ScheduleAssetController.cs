using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models.Getter;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.ScheduleElements;

namespace ScheduleAPI.Controllers.API.Schedule
{
    /// <summary>
    /// Класс-контроллер получения расписания через ассеты.
    /// </summary>
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
        /// <param name="environment">Данные о окружении приложения.</param>
        public ScheduleDayAssetController(IHostEnvironment environment)
        {
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
        public JsonResult Get(int dayIndex = 0, string groupName = "19П-3")
        {
            groupName = groupName.RemoveStringChars();
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            DaySchedule schedule = getter.GetDaySchedule(dayIndex, groupName);

            return new JsonResult(schedule, SerializeFormatter.JsonOptions);
        }
        #endregion
    }

    /// <summary>
    /// Класс-контроллер получения расписания через ассеты.
    /// <br/>
    /// Позволяет получить расписание на неделю.
    /// </summary>
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
        /// <param name="environment">Данные о окружении приложения.</param>
        public ScheduleWeekAssetController(IHostEnvironment environment)
        {
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
        public JsonResult Get(string groupName = "19П-3")
        {
            WeekSchedule schedule = getter.GetWeekSchedule(groupName);

            return new JsonResult(schedule, SerializeFormatter.JsonOptions);
        }
        #endregion
    }
}
