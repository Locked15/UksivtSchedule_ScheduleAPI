using Microsoft.AspNetCore.Mvc;

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
    }
}
