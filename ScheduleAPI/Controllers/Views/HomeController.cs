using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Models.Exceptions.View;
using System.Net;

namespace ScheduleAPI.Controllers.ViewsControllers
{
    /// <summary>
    /// Класс-контроллер для домашней страницы API.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Базовый контроллер для логирования событий, тесно не связанных со специализированными контроллерами.
        /// </summary>
        public static ILogger<HomeController>? BaseLogger { get; private set; } = null;

        /// <summary>
        /// Конструктор класса. Неявно вызывается при первом обращении к API.
        /// </summary>
        /// <param name="logger">Настроенный сервис логирования.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            BaseLogger = logger;
        }

        /// <summary>
        /// Функция получения базовой страницы API.
        /// </summary>
        /// <returns>Запрос на отрисовку страницы "Index.cshtml".</returns>
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Status(int code) =>
                Error(code);

        /// <summary>
        /// Эта функция автоматически срабатывает в случае возникновения НЕ критической ошибки в работе API. <br />
        /// Она перенаправляет пользователя на страницу с информацией.
        /// </summary>
        /// <returns>Страница с информацией об ошибке.</returns>
        public IActionResult Error(int code = 0)
        {
            var model = new ErrorModel()
            {
                RequestID = Guid.NewGuid(),
                ErrorCode = GetElementByCode(code),
                Message = "Happend something bad.\n\nWe'll fix it as soon, as it possible."
            };

            return View("Error", model);
        }

        private static HttpStatusCode GetElementByCode(int code)
        {
            if (Enum.IsDefined(typeof(HttpStatusCode), code))
                return (HttpStatusCode)code;

            return HttpStatusCode.Forbidden;
        }
    }
}
