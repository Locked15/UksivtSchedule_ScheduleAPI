using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models;
using ScheduleAPI.Other.General;
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
        public ScheduleDataBaseDayController()
        { }

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
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
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
        public ScheduleDataBaseWeekController()
        { }

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
