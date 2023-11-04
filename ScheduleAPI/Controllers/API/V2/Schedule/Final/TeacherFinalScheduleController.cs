using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Db;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Entities.Wrappers;

namespace ScheduleAPI.Controllers.API.V2.Schedule.Final
{
    [Route("~/api/v2/[controller]/")]
    [Route("~/api/v2/schedule/{word:regex(final[[s]]?)}/teacher/")]
    public class TeacherFinalScheduleController : Controller, IScheduleController
    {
        #region Область: Поля.

        private readonly DbDataGetter dataGetter;
        #endregion

        #region Область: Свойства.

        private DataContext DataContext { get; init; }

        public static ILogger<TeacherFinalScheduleController>? Logger { get; private set; }
        #endregion

        #region Область: Инициализаторы.

        public TeacherFinalScheduleController(DataContext dataContext, ILogger<TeacherFinalScheduleController> logger)
        {
            DataContext = dataContext;
            dataGetter = new(DataContext);

            Logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        #region Подоблать: Дневные Расписания.

        [HttpGet("day/index-id")]
        public IActionResult GetTeacherFinalDayScheduleByDayIndexAndTeacherId(int dayIndex = IScheduleController.DefaultDayIndex, int teacherId = IScheduleController.DefaultTeacherIndex)
        {
            var targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
            var result = dataGetter.GetFinalScheduleForTeacher(teacherId, targetDate);

            return Json(result);
        }

        [HttpGet("day/index-bio")]
        public IActionResult GetTeacherFinalDayScheduleByDayIndexAndBio(string? name, string? surname, string? patronymic, int dayIndex = IScheduleController.DefaultDayIndex)
        {
            var targetDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
            var result = dataGetter.GetFinalScheduleForTeacher(name, surname, patronymic, targetDate);

            return Json(result);
        }

        [HttpGet("day/date-id")]
        public IActionResult GetTeacherFinalDayScheduleByDateAndTeacherId(DateOnly targetDate, int teacherId = IScheduleController.DefaultTeacherIndex)
        {
            var result = dataGetter.GetFinalScheduleForTeacher(teacherId, targetDate);
            return Json(result);
        }

        [HttpGet("day/date-bio")]
        public IActionResult GetTeacherFinalDayScheduleByDateAndBio(string? name, string? surname, string? patronymic, DateOnly targetDate)
        {
            var result = dataGetter.GetFinalScheduleForTeacher(name, surname, patronymic, targetDate);
            return Json(result);
        }
        #endregion

        #region Подоблать: Недельные Расписания.

        [HttpGet("week/id")]
        [HttpGet("week/index-id")]
        public IActionResult GetTeacherFinalWeekScheduleByTeacherId(int teacherId = IScheduleController.DefaultTeacherIndex)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            for (int i = 0; i < 7; i++)
            {
                var targetDate = DateOnly.FromDateTime(i.GetDateTimeInWeek());
                results.Add(dataGetter.GetFinalScheduleForTeacher(teacherId, targetDate));
            }

            return Json(results);
        }

        [HttpGet("week/bio")]
        [HttpGet("week/index-bio")]
        public IActionResult GetTeacherFinalWeekScheduleByBio(string? name, string? surname, string? patronymic)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            for (int i = 0; i < 7; i++)
            {
                var targetDate = DateOnly.FromDateTime(i.GetDateTimeInWeek());
                results.Add(dataGetter.GetFinalScheduleForTeacher(name, surname, patronymic, targetDate));
            }

            return Json(results);
        }

        [HttpGet("week/date-id")]
        public IActionResult GetTeacherFinalWeekScheduleByDateAndTeacherId(DateOnly targetDate, int teacherId = IScheduleController.DefaultTeacherIndex)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            (DateOnly currentDate, DateOnly targetWeekEndingDate) dates = (targetDate.GetStartOfWeek(),
                                                                           targetDate.GetEndOfWeek());
            while (dates.currentDate <= dates.targetWeekEndingDate)
            {
                results.Add(dataGetter.GetFinalScheduleForTeacher(teacherId, dates.currentDate));
                dates.currentDate = dates.currentDate.AddDays(1);
            }

            return Json(results);
        }

        [HttpGet("week/date-bio")]
        public IActionResult GetTeacherFinalWeekScheduleByDateAndBio(string? name, string? surname, string? patronymic, DateOnly targetDate)
        {
            var results = new List<TeacherScheduleDataWrapper>(7);
            (DateOnly currentDate, DateOnly targetWeekEndingDate) dates = (targetDate.GetStartOfWeek(),
                                                                           targetDate.GetEndOfWeek());
            while (dates.currentDate <= dates.targetWeekEndingDate)
            {
                results.Add(dataGetter.GetFinalScheduleForTeacher(name, surname, patronymic, dates.currentDate));
                dates.currentDate = dates.currentDate.AddDays(1);
            }

            return Json(results);
        }
        #endregion
        #endregion
    }
}
