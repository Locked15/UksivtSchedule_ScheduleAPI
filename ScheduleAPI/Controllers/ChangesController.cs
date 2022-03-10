using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScheduleAPI.Other;
using ScheduleAPI.Models;
using ScheduleAPI.Other.General;
using ScheduleAPI.Models.Getter;

namespace ScheduleAPI.Controllers
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

        /// <summary>
        /// Поле, содержащее значение, отвечающее за активность потока обновления данных.
        /// </summary>
        private Boolean subThreadIsActive = false;
        #endregion

        #region Область: Конструкторы.
        /// <summary>
        /// Конструктор класса. Вызывается неявно при запуске API.
        /// </summary>
        /// <param name="env">Информация об окружении API.</param>
        /// <param name="configuration">Информация о конфигурации приложения.</param>
        public ChangesDayController(IHostEnvironment env, IConfiguration configuration)
        {
            environment = env;

            DataBaseConnector.Initialize(configuration);
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
        public String Get(Int32 dayIndex = 0, String groupName = "19П-3")
        {
            dayIndex = dayIndex.CheckDayIndexFromOverflow();
            ChangesOfDay changes = new ChangesGetter().GetDayChanges(dayIndex, groupName);

            changes.ChangesDate = changes.ChangesDate.HasValue ? changes.ChangesDate : dayIndex.GetDateTimeInWeek();
            String value = JsonConvert.SerializeObject(changes);

            if (!subThreadIsActive)
            {
                AutoUploadSiteAsync();
            }
            
            return value;
        }

        /// <summary>
        /// Метод для автоматического обновления данных о изменении расписания.
        /// <br/>
        /// Нужен для обеспечения быстрой работы API.
        /// </summary>
        [NonAction]
        public async void AutoUploadSiteAsync()
        {
            subThreadIsActive = true;

            //Субпоток автоматического обновления данных с сайта. Вызов GET производится каждые 10 минут.
            await Task.Run(() =>
            {
                System.Diagnostics.Stopwatch sw1 = new();

                while (true)
                {
                    Thread.Sleep(600000);

                    sw1.Restart();
                    Get();
                    sw1.Stop();

                    Logger.WriteError(7, $"Завершен вызов автоматического обновления данных о сайте, время вызова: {sw1.ElapsedMilliseconds} миллисекунд.");
                }
            });
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
        /// <param name="configuration">Информация о конфигурации приложения.</param>
        public ChangesWeekController(IHostEnvironment env, IConfiguration configuration)
        {
            environment = env;

            DataBaseConnector.Initialize(configuration);
        }
        #endregion

        #region Область: Методы.
        /// <summary>
        /// Метод, представляющий Get-запрос на получение замен на неделю.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Строковое представление списка замен.</returns>
        [HttpGet]
        public String Get(String groupName = "19П-3")
        {
            List<ChangesOfDay> changes = new ChangesGetter().GetWeekChanges(groupName);

            return JsonConvert.SerializeObject(changes);
        }
        #endregion
    }
}
