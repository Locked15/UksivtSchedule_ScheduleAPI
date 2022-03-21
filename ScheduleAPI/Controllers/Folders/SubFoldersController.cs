using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ScheduleAPI.Controllers.Folders
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubFoldersController : FolderControllerAbstract
    {
        #region Область: Конструктор.
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="environment">Окружение, в котором развернуто приложение.</param>
        public SubFoldersController(IHostEnvironment environment) : base(environment)
        { }
        #endregion

        #region Область: Метод.
        /// <summary>
        /// Метод, представляющий Get-запрос на получение списка направлений обучения в отделении.
        /// </summary>
        /// <param name="folder">Нужное отделение.</param>
        /// <returns>Строковое представление списка направлений обучения.</returns>
        [HttpGet]
        public String Get(String folder = "Programming")
        {
            List<String> subFolders = Getter.GetSubFolders(folder);

            return JsonConvert.SerializeObject(subFolders);
        }
        #endregion
    }
}
