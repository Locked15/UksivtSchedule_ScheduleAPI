﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetBranches()
        {
            List<string> folders = getter.GetBranches();

            return Ok(folders);
        }

        [HttpGet]
        [Route("~/api/[controller]/affiliates")]
        public IActionResult GetAffiliates(string branch = "Programming")
        {
            List<string> affiliates = getter.GetAffiliates(branch);

            return Ok(affiliates);
        }

        [HttpGet]
        [Route("~/api/[controller]/groups")]
        public IActionResult GetGroups(string branch = "Programming", string affiliate = "П")
        {
            List<string> groups = getter.GetGroupNames(branch, affiliate);

            return Ok(groups);
        }

        [HttpGet]
        [Route("~/api/[controller]/summary")]
        public IActionResult GetSummary()
        {
            List<string> groups = getter.GetAllAvailableGroups();

            return Ok(groups);
        }
    }
}
