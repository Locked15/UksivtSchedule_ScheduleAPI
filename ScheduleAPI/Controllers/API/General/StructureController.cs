using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.Getter;

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
        [Route("~/api/[controller]/folders")]
        public JsonResult GetFolders()
        {
            List<string> folders = getter.GetFolders();

            return Json(folders);
        }

        [HttpGet]
        [Route("~/api/[controller]/subfolders")]
        public JsonResult GetSubFolders(string folder = "Programming")
        {
            List<string> subFolders = getter.GetSubFolders(folder);

            return Json(subFolders);
        }

        [HttpGet]
        [Route("~/api/[controller]/groups")]
        public JsonResult GetGroups(string folder = "Programming", string subFolder = "П")
        {
            List<string> groups = getter.GetGroupNames(folder, subFolder);

            return Json(groups);
        }

        [HttpGet]
        [Route("~/api/[controller]/summary")]
        public JsonResult GetSummary()
        {
            List<string> groups = new List<string>(1);
            foreach (var folder in getter.GetFolders())
            {
                foreach (var subFolder in getter.GetSubFolders(folder))
                {
                    groups.AddRange(getter.GetGroupNames(folder, subFolder));
                }
            }

            return Json(groups);
        }
    }
}
