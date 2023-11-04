using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Db;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Entities.Wrappers.Group;

namespace ScheduleAPI.Controllers.API.V2.Schedule.Final
{
    [Route("~/api/v2/[controller]/")]
    [Route("~/api/v2/schedule/{word:regex(final[[s]]?)}/group/")]
    public class GroupFinalScheduleController : Controller, IScheduleController
    {
        #region Область: Поля.

        private readonly DbDataGetter dataGetter;
        #endregion

        #region Область: Свойства.

        private DataContext DataContext { get; init; }

        public static ILogger<GroupFinalScheduleController>? Logger { get; private set; }
        #endregion

        #region Область: Инициализаторы.

        public GroupFinalScheduleController(DataContext dataContext, ILogger<GroupFinalScheduleController> logger)
        {
            DataContext = dataContext;
            dataGetter = new(DataContext);

            Logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        [HttpGet("day")]
        public IActionResult GetGroupFinalDayScheduleByDayIndex(int dayIndex = IScheduleController.DefaultDayIndex, string targetGroup = IScheduleController.DefaultGroupName)
        {
            var targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
            var result = dataGetter.GetFinalScheduleForGroup(targetGroup, targetDate);

            return Json(result);
        }

        [HttpGet("day/date")]
        public IActionResult GetGroupFinalDayScheduleByDate(DateOnly targetDate, string targetGroup = IScheduleController.DefaultGroupName)
        {
            var result = dataGetter.GetFinalScheduleForGroup(targetGroup, targetDate);
            return Json(result);
        }

        [HttpGet("week")]
        public IActionResult GetGroupFinalWeekSchedule(string targetGroup = IScheduleController.DefaultGroupName)
        {
            var results = new List<GroupFinalScheduleWrapper>(7);
            for (int i = 0; i < 7; i++)
            {
                var targetDate = DateOnly.FromDateTime(i.GetDateTimeInWeek());
                results.Add(dataGetter.GetFinalScheduleForGroup(targetGroup, targetDate));
            }

            return Json(results);
        }

        [HttpGet("week/date")]
        public IActionResult GetGroupFinalWeekScheduleByDate(DateOnly targetDate, string targetGroup = IScheduleController.DefaultGroupName)
        {
            var results = new List<GroupFinalScheduleWrapper>(7);
            (DateOnly currentDate, DateOnly targetWeekEndingDate) dates = (targetDate.GetStartOfWeek(),
                                                                           targetDate.GetEndOfWeek());
            while (dates.currentDate <= dates.targetWeekEndingDate)
            {
                results.Add(dataGetter.GetFinalScheduleForGroup(targetGroup, dates.currentDate));
                dates.currentDate = dates.currentDate.AddDays(1);
            }

            return Json(results);
        }
        #endregion
    }
}
