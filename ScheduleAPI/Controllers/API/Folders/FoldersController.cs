using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ScheduleAPI.Controllers.API.Folders
{
    /// <summary>
    /// Контроллер для получения данных о директориях с группами.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FoldersController : FolderControllerAbstract
    {
        #region Область: Конструктор.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="env">Окружение, в котором развернуто приложение.</param>
        public FoldersController(IHostEnvironment env) : base(env)
        { }
        #endregion

        #region Область: Метод.

        /// <summary>
        /// Метод, представляющий Get-запрос на получение списка папок-отделений в ассетах.
        /// </summary>
        /// <returns>Строковое представление списка с папками.</returns>
        [HttpGet]
        public string Get()
        {
            List<string> folders = Getter.GetFolders();

            return JsonConvert.SerializeObject(folders);
        }
        #endregion
    }
}
