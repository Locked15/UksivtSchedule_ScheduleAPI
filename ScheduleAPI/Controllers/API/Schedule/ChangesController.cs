using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.Getter;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Elements.Schedule;

namespace ScheduleAPI.Controllers.API.Schedule
{
    /// <summary>
    /// Класс-контроллер для получения замен.
    /// <br/>
    /// Ранее он был разбит на 2 класса: для одного дня и для недели. Теперь они объединены в один класс.
    /// </summary>
    [Route("~/api/[controller]")]
    public class ChangesController
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
        public static ILogger<ChangesController>? Logger { get; private set; } = null;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="env">Информация об окружении API.</param>
        /// <param name="logger">Автоматически инициализированный сервис логгера для работы.</param>
        public ChangesController(IHostEnvironment env, ILogger<ChangesController> logger)
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
        [HttpGet]
        [Route("~/api/[controller]/day")]
        public JsonResult DayChanges(int dayIndex = 0, string groupName = "19П-3")
        {
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            ChangesOfDay changes = new ChangesGetter(dayIndex, groupName).GetDayChanges();

            changes.ChangesDate = changes.ChangesDate.HasValue ? changes.ChangesDate : dayIndex.GetDateTimeInWeek();

            return new JsonResult(changes, SerializeFormatter.JsonOptions);
        }

        /// <summary>
        /// Метод, представляющий Get-запрос на получение замен на неделю.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Строковое представление списка замен.</returns>
        [HttpGet]
        [Route("~/api/[controller]/week")]
        public JsonResult WeekChanges(string groupName = "19П-3")
        {
            List<ChangesOfDay> changes = new ChangesGetter(default, groupName).GetWeekChanges();

            return new JsonResult(changes, SerializeFormatter.JsonOptions);
        }
        #endregion
    }
}
