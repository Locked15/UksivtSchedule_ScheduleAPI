using Microsoft.AspNetCore.Mvc;

namespace ScheduleAPI.Controllers.ViewsControllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
