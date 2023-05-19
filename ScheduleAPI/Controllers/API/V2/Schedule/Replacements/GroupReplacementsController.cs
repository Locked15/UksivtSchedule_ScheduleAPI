using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Db;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Entities.Wrappers.Group;

namespace ScheduleAPI.Controllers.API.V2.Schedule.Replacements
{
    [Route("~/api/v2/[controller]/")]
    [Route("~/api/v2/schedule/{word:regex(replacement[[s]]?)}/group/")]
    public class GroupReplacementsController : Controller
    {
        #region Область: Поля.

        private readonly DbDataGetter dataGetter;
        #endregion

        #region Область: Свойства.

        private DataContext DataContext { get; init; }

        public static ILogger<GroupReplacementsController>? Logger { get; private set; }
        #endregion

        #region Область: Инициализаторы.

        public GroupReplacementsController(DataContext dataContext, ILogger<GroupReplacementsController> logger)
        {
            DataContext = dataContext;
            dataGetter = new(DataContext);

            Logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        [HttpGet("day")]
        public IActionResult GetGroupDayReplacementsByDayIndex(int dayIndex = 0, string targetGroup = "19П-3")
        {
            var targetDate = DateOnly.FromDateTime(dayIndex.CheckDayIndexFromOverflow().GetDateTimeInWeek());
            var result = dataGetter.GetReplacementsForGroup(targetGroup, targetDate);

            return Json(result);
        }

        [HttpGet("day/date")]
        public IActionResult GetGroupDayReplacementsByDate(DateOnly targetDate, string targetGroup = "19П-3")
        {
            var result = dataGetter.GetReplacementsForGroup(targetGroup, targetDate);
            return Json(result);
        }

        [HttpGet("week")]
        public IActionResult GetGroupWeekReplacements(string targetGroup = "19П-3")
        {
            var results = new List<GroupReplacementsWrapper>(1);
            for (int i = 0; i < 7; i++)
            {
                var targetDate = DateOnly.FromDateTime(i.CheckDayIndexFromOverflow().GetDateTimeInWeek());
                results.Add(dataGetter.GetReplacementsForGroup(targetGroup, targetDate));
            }

            return Json(results);
        }

        [HttpGet("week/date")]
        public IActionResult GetGroupWeekReplacementsByDate(DateOnly targetDate, string targetGroup = "19П-3")
        {
            var results = new List<GroupReplacementsWrapper>(7);
            (DateOnly currentDate, DateOnly targetWeekEndingDate) dates = (targetDate.GetStartOfWeek(),
                                                                           targetDate.GetEndOfWeek());
            while (dates.currentDate <= dates.targetWeekEndingDate)
            {
                results.Add(dataGetter.GetReplacementsForGroup(targetGroup, dates.currentDate));
                dates.currentDate = dates.currentDate.AddDays(1);
            }

            return Json(results);
        }
        #endregion
    }
}
