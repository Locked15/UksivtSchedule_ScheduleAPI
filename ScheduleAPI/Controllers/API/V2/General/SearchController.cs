using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.Getter.Db;
using ScheduleAPI.Models.Entities;

namespace ScheduleAPI.Controllers.API.V2.General
{
    [Route("~/api/v2/[controller]/")]
    [Route("~/api/v2/general/[controller]/")]
    public class SearchController : Controller
    {
        #region Область: Свойства.

        private DataContext DataContext { get; set; }

        public static ILogger<SearchController>? Logger { get; private set; }
        #endregion

        #region Область: Инициализаторы.

        public SearchController(DataContext dataContext, ILogger<SearchController> logger)
        {
            DataContext = dataContext;
            Logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        [HttpGet("{word:regex(teacher[[s]]?)}/all")]
        public IActionResult GetTeachers()
        {
            var dataGetter = new DbDataGetter(DataContext);
            var data = dataGetter.GetAllTeachers();

            return Json(data);
        }

        [HttpGet("{word:regex(teacher[[s]]?)}")]
        public IActionResult SearchTeachers(string? name = default, string? surname = default, string? patronymic = default)
        {
            var dataGetter = new DbDataGetter(DataContext);
            var result = dataGetter.SearchTeachers((name, surname, patronymic));

            return Json(result);
        }
        #endregion
    }
}
