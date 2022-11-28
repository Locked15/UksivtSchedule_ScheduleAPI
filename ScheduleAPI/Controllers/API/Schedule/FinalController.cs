using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.Getter;
using ScheduleAPI.Controllers.Data.Getter.Changes;
using ScheduleAPI.Controllers.Data.Getter.Schedule;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Elements.Schedule;
using ScheduleAPI.Models.Elements.Schedule.Final;

namespace ScheduleAPI.Controllers.API.Schedule
{
    /// <summary>
    /// Класс-контроллер, позволяющий сразу получить итоговое расписание, с учетом замен.
    /// </summary>
    [Route("~/api/[controller]")]
    public class FinalController : Controller
    {
        #region Область: Поля.

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
        }
        #endregion

        #region Область: Обработчики API.

        /// <summary>
        /// Запрос на получение итогового расписания для одного дня.
        /// </summary>
        /// <param name="dayIndex">Индекс нужного дня.</param>
        /// <param name="groupName">Название нужной группы.</param>
        /// <returns>Итоговое расписания с учётом доступных замен.</returns>
        [HttpGet]
        [Route("~/api/[controller]/day")]
        public JsonResult GetFinalDaySchedule(int dayIndex = 0, string groupName = "19П-3")
        {
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            groupName = groupName.RemoveStringChars();

            var baseGetter = new AssetGetter(environment);
            var changesGetter = new TargetChangesGetter(dayIndex, groupName);

            var schedule = baseGetter.GetDaySchedule(dayIndex, groupName);
            var changes = changesGetter.GetDayChanges();

            return new JsonResult(new FinalDaySchedule(schedule, changes), SerializeFormatter.JsonOptions);
        }

        /// <summary>
        /// Запрос на получение итогового расписания на неделю.
        /// </summary>
        /// <param name="groupName">Название нужной группы.</param>
        /// <returns>Итоговое расписание на неделю, с учётом всех доступных замен.</returns>
        [HttpGet]
        [Route("~/api/[controller]/week")]
        public JsonResult GetFinalWeekSchedule(string groupName = "19П-3")
        {
            groupName = groupName.RemoveStringChars();

            var baseGetter = new AssetGetter(environment);
            var changesGetter = new TargetChangesGetter(default, groupName);

            var schedule = baseGetter.GetWeekSchedule(groupName);
            var changes = changesGetter.GetWeekChanges();

            return new JsonResult(new FinalWeekSchedule(groupName, schedule.DaySchedules, changes ?? Enumerable.Empty<ChangesOfDay>().ToList()), 
                                  SerializeFormatter.JsonOptions);
        }
        #endregion
    }
}
