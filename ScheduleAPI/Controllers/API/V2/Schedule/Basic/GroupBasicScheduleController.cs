using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Db;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Entities.Wrappers.Group;

namespace ScheduleAPI.Controllers.API.V2.Schedule.Basic
{
    [Route("~/api/v2/[controller]/")]
    [Route("~/api/v2/schedule/{word:regex(basic[[s]]?)}/group/")]
    public class GroupBasicScheduleController : Controller
    {
        #region Область: Поля.

        private readonly DbDataGetter dataGetter;
        #endregion

        #region Область: Свойства.

        private DataContext DataContext { get; init; }

        public static ILogger<GroupBasicScheduleController>? Logger { get; private set; }
        #endregion

        #region Область: Инициализаторы.

        public GroupBasicScheduleController(DataContext dataContext, ILogger<GroupBasicScheduleController> logger)
        {
            DataContext = dataContext;
            dataGetter = new(DataContext);

            Logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        [HttpGet("day")]
        public IActionResult GetGroupBasicDayScheduleByDayIndex(int dayIndex = 0, string targetGroup = "19П-3")
        {
            var targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
            var result = dataGetter.GetBasicScheduleForGroup(targetGroup, targetDate);

            return Json(result);
        }

        [HttpGet("day/date")]
        public IActionResult GetGroupBasicDayScheduleByDate(DateOnly targetDate, string targetGroup = "19П-3")
        {
            var result = dataGetter.GetBasicScheduleForGroup(targetGroup, targetDate);
            return Json(result);
        }

        [HttpGet("week")]
        public IActionResult GetGroupBasicWeekSchedule(string targetGroup = "19П-3")
        {
            var results = new List<GroupBasicScheduleWrapper>(7);
            for (int i = 0; i < 7; i++)
            {
                var targetDate = DateOnly.FromDateTime(i.GetDateTimeInWeek());
                results.Add(dataGetter.GetBasicScheduleForGroup(targetGroup, targetDate));
            }

            return Json(results);
        }

        [HttpGet("week/date")]
        public IActionResult GetGroupBasicWeekScheduleByDate(DateOnly targetDate, string targetGroup = "19П-3")
        {
            var results = new List<GroupBasicScheduleWrapper>(7);
            (DateOnly currentDate, DateOnly targetWeekEndingDate) dates = (targetDate.GetStartOfWeek(),
                                                                           targetDate.GetEndOfWeek());
            while (dates.currentDate <= dates.targetWeekEndingDate)
            {
                results.Add(dataGetter.GetBasicScheduleForGroup(targetGroup, dates.currentDate));
                dates.currentDate = dates.currentDate.AddDays(1);
            }

            return Json(results);
        }
        #endregion
    }
}
