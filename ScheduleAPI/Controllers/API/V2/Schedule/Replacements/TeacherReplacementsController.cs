using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Db;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Entities.Wrappers;

namespace ScheduleAPI.Controllers.API.V2.Schedule.Replacements
{
    [Route("~/api/v2/[controller]/")]
    [Route("~/api/v2/schedule/{word:regex(replacement[[s]]?)}/teacher/")]
    public class TeacherReplacementsController : Controller
    {
        #region Область: Поля.

        private readonly DbDataGetter dataGetter;
        #endregion

        #region Область: Свойства.

        private DataContext DataContext { get; init; }

        public static ILogger<TeacherReplacementsController>? Logger { get; private set; }
        #endregion

        #region Область: Инициализаторы.

        public TeacherReplacementsController(DataContext dataContext, ILogger<TeacherReplacementsController> logger)
        {
            DataContext = dataContext;
            dataGetter = new(DataContext);

            Logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        #region Подоблать: Дневные Замены.

        [HttpGet("day/index-id")]
        public IActionResult GetTeacherDayReplacementsByDayIndexAndTeacherId(int dayIndex = 0, int teacherId = 0)
        {
            var targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
            var result = dataGetter.GetReplacementsForTeacher(teacherId, targetDate);

            return Json(result);
        }

        [HttpGet("day/index-bio")]
        public IActionResult GetTeacherDayReplacementByDayIndexAndBio(string? name, string? surname, string? patronymic, int dayIndex = 0)
        {
            var targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
            var result = dataGetter.GetReplacementsForTeacher(name, surname, patronymic, targetDate);

            return Json(result);
        }

        [HttpGet("day/date-id")]
        public IActionResult GetTeacherDayReplacementsByDateAndTeacherId(DateOnly targetDate, int teacherId = 0)
        {
            var result = dataGetter.GetReplacementsForTeacher(teacherId, targetDate);
            return Json(result);
        }

        [HttpGet("day/date-bio")]
        public IActionResult GetTeacherDateReplacementsByDateAndBio(string? name, string? surname, string? patronymic, DateOnly targetDate)
        {
            var result = dataGetter.GetReplacementsForTeacher(name, surname, patronymic, targetDate);
            return Json(result);
        }
        #endregion

        #region Подоблать: Недельные Замены.

        [HttpGet("week/id")]
        [HttpGet("week/index-id")]
        public IActionResult GetTeacherWeekReplacementsByTeacherId(int teacherId = 0)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            for (int i = 0; i < 7; i++)
            {
                var targetDate = DateOnly.FromDateTime(i.GetDateTimeInWeek());
                results.Add(dataGetter.GetReplacementsForTeacher(teacherId, targetDate));
            }

            return Json(results);
        }

        [HttpGet("week/bio")]
        [HttpGet("week/index-bio")]
        public IActionResult GetTeacherWeekReplacementsByBio(string? name, string? surname, string? patronymic)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            for (int i = 0; i < 7; i++)
            {
                var targetDate = DateOnly.FromDateTime(i.GetDateTimeInWeek());
                results.Add(dataGetter.GetReplacementsForTeacher(name, surname, patronymic, targetDate));
            }

            return Json(results);
        }

        [HttpGet("week/date-id")]
        public IActionResult GetTeacherWeekReplacementsByDateAndTeacherId(DateOnly targetDate, int teacherId = 0)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            (DateOnly currentDate, DateOnly targetWeekEndingDate) dates = (targetDate.GetStartOfWeek(),
                                                                           targetDate.GetEndOfWeek());
            while (dates.currentDate <= dates.targetWeekEndingDate)
            {
                results.Add(dataGetter.GetReplacementsForTeacher(teacherId, targetDate));
                dates.currentDate = dates.currentDate.AddDays(1);
            }

            return Json(results);
        }

        [HttpGet("week/date-bio")]
        public IActionResult GetTeacherWeekReplacementsByDateAndBio(string? name, string? surname, string? patronymic, DateOnly targetDate)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            (DateOnly currentDate, DateOnly targetWeekEndingDate) dates = (targetDate.GetStartOfWeek(),
                                                                           targetDate.GetEndOfWeek());
            while (dates.currentDate <= dates.targetWeekEndingDate)
            {
                results.Add(dataGetter.GetReplacementsForTeacher(name, surname, patronymic, dates.currentDate));
                dates.currentDate = dates.currentDate.AddDays(1);
            }

            return Json(results);
        }
        #endregion
        #endregion
    }
}
