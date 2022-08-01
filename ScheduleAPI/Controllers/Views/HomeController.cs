using Microsoft.AspNetCore.Mvc;

namespace ScheduleAPI.Controllers.ViewsControllers
{
    /// <summary>
    /// Класс-контроллер для домашней страницы API.
    /// </summary>
    public class HomeController : Controller
    {
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
