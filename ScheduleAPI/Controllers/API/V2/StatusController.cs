using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Entities.Utilities;

namespace ScheduleAPI.Controllers.API.V2
{
    [Route("~/api/v2/")]
    [Route("~/api/v2/[controller]")]
    public class StatusController : Controller
    {
        #region Область: Свойства.

        private DataContext DataContext { get; init; }
        #endregion

        #region Область: Инициализаторы.

        public StatusController(DataContext dataContext)
        {
            DataContext = dataContext;
        }
        #endregion

        #region Область: Обработчики API.

        [HttpGet]
        public IActionResult GetDbDataRelevance()
        {
            var lastAvailableReplacementDate = DataContext.ScheduleReplacements.Max(replacement =>
                                                                                    replacement.ReplacementDate);
            var lastAvailableFinalScheduleDate = DataContext.FinalSchedules.Max(schedule =>
                                                                                schedule.ScheduleDate);

            var relevanceStatus = new EntitiesDatesStatus(lastAvailableReplacementDate, lastAvailableFinalScheduleDate);

            return Json(relevanceStatus);
        }
        #endregion
    }
}
