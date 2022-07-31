using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ScheduleAPI.Controllers.Folders
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoldersController : FolderControllerAbstract
    {
        #region Область: Конструктор.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="env">Окружение, в котором развенуто приложение.</param>
        public FoldersController(IHostEnvironment env) : base(env)
        { }
        #endregion

        #region Область: Метод.

        /// <summary>
        /// Метод, представляющий Get-запрос на получение списка папок-отделений в ассетах.
        /// </summary>
        /// <returns>Строковое представление списка с папками.</returns>
        [HttpGet]
        public String Get()
        {
            List<String> folders = Getter.GetFolders();

            return JsonConvert.SerializeObject(folders);
        }
        #endregion
    }
}
