using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Getter;
using ScheduleAPI.Models.ScheduleElements;

namespace ScheduleAPI.Controllers.API.Schedule
{
    /// <summary>
    /// Класс-контроллер получения замен для расписания.
    /// </summary>
    [Route("api/day/[controller]")]
    [ApiController]
    public class ChangesDayController : Controller
    {
        #region Область: Поля.

        /// <summary>
        /// Поле, содержащее объект, содержащий данные о окружении приложения.
        /// </summary>
        private IHostEnvironment environment;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="env">Информация об окружении API.</param>
        public ChangesDayController(IHostEnvironment env)
        {
            environment = env;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод, представляющий Get-запрос на получение замен.
        /// </summary>
        /// <param name="dayIndex">Индекс дня.</param>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Строковое представление списка замен.</returns>
        [HttpGet]
        public JsonResult Get(int dayIndex = 0, string groupName = "19П-3")
        {
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            ChangesOfDay changes = ChangesGetter.GetDayChanges(dayIndex, groupName);

            changes.ChangesDate = changes.ChangesDate.HasValue ? changes.ChangesDate : dayIndex.GetDateTimeInWeek();

            return new JsonResult(changes, SerializeFormatter.JsonOptions);
        }
        #endregion
    }

    /// <summary>
    /// Класс-контроллер получения замен для расписания.
    /// <br/>
    /// Позволяет получить замены на неделю.
    /// </summary>
    [Route("api/week/[controller]")]
    [ApiController]
    public class ChangesWeekController : Controller
    {
        #region Область: Поля.

        /// <summary>
        /// Поле, содержащее объект, содержащий данные о окружении приложения.
        /// </summary>
        private IHostEnvironment environment;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="env">Информация об окружении API.</param>
        public ChangesWeekController(IHostEnvironment env)
        {
            environment = env;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод, представляющий Get-запрос на получение замен на неделю.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Строковое представление списка замен.</returns>
        [HttpGet]
        public JsonResult Get(string groupName = "19П-3")
        {
            List<ChangesOfDay> changes = ChangesGetter.GetWeekChanges(groupName);

            return new JsonResult(changes, SerializeFormatter.JsonOptions);
        }
        #endregion
    }
}
