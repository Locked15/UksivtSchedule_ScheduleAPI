using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Other;
using ScheduleAPI.Models;
using ScheduleAPI.Models.Getter;
using Bool = System.Boolean;

namespace ScheduleAPI.Controllers
{
    /// <summary>
    /// Класс-контроллер получения расписания через базу данных.
    /// </summary>
    [Route("api/day/[controller]")]
    [ApiController]
    public class ScheduleDataBaseDayController : Controller
    {
        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="config">Конфигурация машины.</param>
        public ScheduleDataBaseDayController(IConfiguration config)
        {
            DataBaseConnector.Initialize(config);
        }

        /// <summary>
        /// Метод, реализующий Get-запрос на получение расписания.
        /// </summary>
        /// <param name="dayIndex">Индекс дня для получения расписания.</param>
        /// <param name="groupName">Название группы.</param>
        /// <param name="selectUnsecure">Выбирать "небезопасные" значения из БД?</param>
        /// <returns>Строковое представление расписания на указанный день для указанной группы.</returns>
        [HttpGet]
        public String Get(Int32 dayIndex = 0, String groupName = "19П-3", Bool selectUnsecure = false)
        {
            DaySchedule schedule = DbGetter.GetDaySchedule(dayIndex, groupName, selectUnsecure);

            return SerializeFormatter.ConvertToUnformattedJsonForm(schedule);
        }
    }

    /// <summary>
    /// Класс-контроллер получения расписания через базу данных.
    /// <br/>
    /// Позволяет получать расписание на неделю.
    /// </summary>
    [Route("api/week/[controller]")]
    [ApiController]
    public class ScheduleDataBaseWeekController : Controller
    {
        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="config">Конфигурация машины.</param>
        public ScheduleDataBaseWeekController(IConfiguration config)
        {
            DataBaseConnector.Initialize(config);
        }

        /// <summary>
        /// Метод, реализующий Get-запрос на получение расписания.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <param name="selectUnsecure">Выбирать "небезопасные" значения из БД?</param>
        /// <returns>Строковое представление расписания на неделю для указанной группы.</returns>
        [HttpGet]
        public String Get(String groupName = "19П-3", Bool selectUnsecure = false)
        {
            WeekSchedule schedule = DbGetter.GetWeekSchedule(groupName, selectUnsecure);

            return SerializeFormatter.ConvertToUnformattedJsonForm(schedule);
        }
    }
}
