using Microsoft.AspNetCore.Mvc;

namespace ScheduleAPI.Controllers.API.V1
{
    /// <summary>
    /// Этот контроллер предназначен для перенаправлений запросов к API по старым адресам к новым.
    /// </summary>
    public class LegacyRedirectionController : Controller
    {
        #region Область: Поля и Свойства.

        private readonly ILogger<LegacyRedirectionController> logger;
        #endregion

        #region Область: Инициализаторы.

        public LegacyRedirectionController(ILogger<LegacyRedirectionController> logger)
        {
            this.logger = logger;
        }
        #endregion

        #region Область: Обработчики API.

        [Route("~/api/schedule/{**path}")]
        public IActionResult RedirectLegacyBasicScheduleControllerRequest()
        {
            var newPath = Request.Path.ToString().Replace("/schedule/",
                                                          "/v1/schedule/basic/");
            var queryString = Request.QueryString;

            logger.LogWarning("Request for legacy basic schedule controller was redirected to actual route ('/v1/schedule/basic/').");
            return Redirect(string.Concat(newPath, queryString));
        }

        [Route("~/api/changes/{**path}")]
        public IActionResult RedirectLegacyChangesControllerRequest()
        {
            var newPath = Request.Path.ToString().Replace("/changes/",
                                                          "/v1/schedule/replacements/");
            var queryString = Request.QueryString;

            logger.LogWarning("Request for legacy changes (replacements) controller was redirected to actual route ('/v1/schedule/replacements').");
            return Redirect(string.Concat(newPath, queryString));
        }

        [Route("~/api/final/{**path}")]
        public IActionResult RedirectLegacyFinalScheduleControllerRequest()
        {
            var newPath = Request.Path.ToString().Replace("/final/",
                                                          "/v1/schedule/final/");
            var queryString = Request.QueryString;

            logger.LogWarning("Request for legacy final schedule controller was redirected to actual route ('/v1/schedule/final').");
            return Redirect(string.Concat(newPath, queryString));
        }

        [Route("~/api/search/{**path}")]
        public IActionResult RedirectLegacySearchControllerRequest()
        {
            var newPath = Request.Path.ToString().Replace("/search/",
                                                          "/v1/general/search/");
            var queryString = Request.QueryString;

            logger.LogWarning("Request for legacy search controller was redirected to actual route ('/v1/general/search').");
            return Redirect(string.Concat(newPath, queryString));
        }

        [Route("~/api/structure/{**path}")]
        public IActionResult RedirectLegacyStructureControllerRequest()
        {
            var newPath = Request.Path.ToString().Replace("/structure/",
                                                          "/v1/general/structure/");
            var queryString = Request.QueryString;

            logger.LogWarning("Request for legacy structure controller was redirected to actual route ('/v1/general/structure/').");
            return Redirect(string.Concat(newPath, queryString));
        }
        #endregion
    }
}
