using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.Getter.Schedule;

namespace ScheduleAPI.Controllers.API.General
{
    [Route("~/api/[controller]")]
    public class StructureController : Controller
    {
        private AssetGetter getter;

        private IHostEnvironment environment;

        public static ILogger<StructureController>? Logger { get; private set; }

        public StructureController(IHostEnvironment env, ILogger<StructureController> logger)
        {
            environment = env;
            Logger = logger;

            getter = new(environment);
        }

        [HttpGet]
        [Route("~/api/[controller]/branches")]
        public JsonResult GetBranches()
        {
            List<string> folders = getter.GetBranches();

            return Json(folders);
        }

        [HttpGet]
        [Route("~/api/[controller]/affiliates")]
        public JsonResult GetAffiliates(string branch = "Programming")
        {
            List<string> affiliates = getter.GetAffiliates(branch);

            return Json(affiliates);
        }

        [HttpGet]
        [Route("~/api/[controller]/groups")]
        public JsonResult GetGroups(string branch = "Programming", string affiliate = "П")
        {
            List<string> groups = getter.GetGroupNames(branch, affiliate);

            return Json(groups);
        }

        [HttpGet]
        [Route("~/api/[controller]/summary")]
        public JsonResult GetSummary()
        {
            List<string> groups = getter.GetAllAvailableGroups();

            return Json(groups);
        }
    }
}
