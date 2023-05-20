using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Schedule;
using ScheduleAPI.Models.Elements;

namespace ScheduleAPI.Controllers.API.V1.Schedule
{
    /// <summary>
    /// Класс-контроллер, позволяющий сразу получить итоговое расписание, с учетом замен.
    /// </summary>
    [Route("~/api/v1/[controller]/")]
    [Route("~/api/v1/schedule/[controller]/")]
    public class FinalController : Controller
    {
        #region Область: Поля.

        /// <summary>
        /// Содержит объект, необходимый для получения данных о финальном расписании.
        /// </summary>
        private FinalScheduleGetter getter;

        /// <summary>
        /// Содержит сведения о локальной среде.
        /// </summary>
        private IHostEnvironment environment;
        #endregion

        #region Область: Свойства.

        /// <summary>
        /// Логгер, связанный с текущим контроллером.
        /// </summary>
        public static ILogger<FinalController>? Logger { get; private set; }
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса. <br />
        /// Неявно вызывается, при первом обращении к контроллеру.
        /// </summary>
        /// <param name="env">Сведения о среде выполнения.</param>
        /// <param name="logger">Логгер для этого контроллера.</param>
        public FinalController(IHostEnvironment env, ILogger<FinalController> logger)
        {
            environment = env;
            Logger = logger;

            getter = new(environment);
        }
        #endregion

        #region Область: Обработчики API.

        /// <summary>
        /// Запрос на получение итогового расписания для одного дня.
        /// </summary>
        /// <param name="dayIndex">Индекс нужного дня.</param>
        /// <param name="groupName">Название нужной группы.</param>
        /// <returns>Итоговое расписания с учётом доступных замен.</returns>
        [HttpGet("day")]
        public IActionResult GetFinalDaySchedule(int dayIndex = 0, string groupName = "19П-3")
        {
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            groupName = groupName.RemoveStringChars();

            var result = getter.GetDaySchedule(dayIndex, groupName);
            result.ScheduleDate ??= dayIndex.GetDateTimeInWeek();

            return new JsonResult(getter.GetDaySchedule(dayIndex, groupName), JsonSettingsModel.JsonOptions);
        }

        /// <summary>
        /// Запрос на получение итогового расписания на неделю.
        /// </summary>
        /// <param name="groupName">Название нужной группы.</param>
        /// <returns>Итоговое расписание на неделю, с учётом всех доступных замен.</returns>
        [HttpGet("week")]
        public IActionResult GetFinalWeekSchedule(string groupName = "19П-3")
        {
            groupName = groupName.RemoveStringChars();

            var schedule = getter.GetWeekSchedule(default, groupName);
            return new JsonResult(schedule, JsonSettingsModel.JsonOptions);
        }
        #endregion
    }
}
