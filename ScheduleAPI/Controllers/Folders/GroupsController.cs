using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ScheduleAPI.Controllers.Folders
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : FolderControllerAbstract
    {
        #region Область: Конструктор.
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="environment">Окружение, в котором развернуто приложение.</param>
        public GroupsController(IHostEnvironment environment) : base(environment)
        { }
        #endregion

        #region Область: Метод.
        /// <summary>
        /// Метод, представляющий Get-запрос на получение списка групп.
        /// </summary>
        /// <param name="folder">Первая папка в ассетах, отвечающая за отделение.</param>
        /// <param name="subFolder">Вторая папка в ассетах, отвечающая за направление обучения.</param>
        /// <returns>Строковое представление списка групп.</returns>
        [HttpGet]
        public String GetByData(String folder = "Programming", String subFolder = "П")
        {
            List<String> groups = Getter.GetGroupNames(folder, subFolder);

            return JsonConvert.SerializeObject(groups);
        }
        #endregion
    }
}
