using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Controllers.Data.Getter;

namespace ScheduleAPI.Controllers.API.Folders
{
    /// <summary>
    /// Базовый абстрактный класс для контроллеров иерархии файлов и папок.
    /// </summary>
    public abstract class FolderControllerAbstract : Controller
    {
        #region Область: Поле.

        /// <summary>
        /// Защищенное поле, содержащее объект, нужный для работы с иерархией ассетов.
        /// </summary>
        protected AssetGetter Getter;
        #endregion

        #region Область: Конструктор.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="environment">Окружение, в котором развернуто приложение.</param>
        public FolderControllerAbstract(IHostEnvironment environment)
        {
            Getter = new(environment);
        }
        #endregion
    }
}
