using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Controllers.Data.Getter.Replacements;
using ScheduleAPI.Models.Result.Schedule.Replacements;

namespace ScheduleAPI.Controllers.API.V1.Schedule.Replacements
{
    /// <summary>
    /// Класс-контроллер для получения замен.
    /// Для сохранения обратной совместимости после переименования контроллера старый маршрут был сохранён (вместе с новым).
    /// <br/>
    /// Ранее он был разбит на 2 класса: для одного дня и для недели. Теперь они объединены в один класс.
    /// </summary>
    [Route("~/api/v1/[controller]/")]
    [Route("~/api/v1/schedule/[controller]/")]
    public class ReplacementsController : Controller, IScheduleController
    {
        #region Область: Поля.

        /// <summary>
        /// Поле, содержащее объект, содержащий данные о окружении приложения.
        /// </summary>
        private IHostEnvironment environment;
        #endregion

        #region Область: Свойства.

        /// <summary>
        /// Свойство с автоматически инициализированным логгером.
        /// </summary>
        public static ILogger<ReplacementsController>? Logger { get; private set; } = null;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="env">Информация об окружении API.</param>
        /// <param name="logger">Автоматически инициализированный сервис логгера для работы.</param>
        public ReplacementsController(IHostEnvironment env, ILogger<ReplacementsController> logger)
        {
            environment = env;
            Logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        /// <summary>
        /// Метод, представляющий Get-запрос на получение замен.
        /// </summary>
        /// <param name="dayIndex">Индекс дня.</param>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Строковое представление списка замен.</returns>
        [HttpGet("day")]
        public IActionResult DayReplacements(int dayIndex = IScheduleController.DefaultDayIndex, string groupName = IScheduleController.DefaultGroupName)
        {
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            ReplacementsOfDay replacements = new TargetReplacementsGetter(dayIndex, groupName).GetDayReplacements();

            replacements.ChangesDate ??= dayIndex.GetDateTimeInWeek();

            return Json(replacements);
        }

        /// <summary>
        /// Метод, представляющий Get-запрос на получение замен на неделю.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Строковое представление списка замен.</returns>
        [HttpGet("week")]
        public IActionResult WeekReplacements(string groupName = IScheduleController.DefaultGroupName)
        {
            List<ReplacementsOfDay> replacements = new TargetReplacementsGetter(default, groupName).GetWeekReplacements();

            return Json(replacements);
        }
        #endregion
    }
}
