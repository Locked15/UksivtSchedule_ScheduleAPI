using ScheduleAPI.Models.Elements.Site;

namespace ScheduleAPI.Controllers.Data.Getter.Downloaders.Basic
{
    /// <summary>
    /// Общий интерфейс для всех классов скачивания документов с различных платформ.
    /// Главная платформа (по крайней мере, пока что) — Google Drive.
    /// </summary>
    public interface IDownloader
    {
        #region Область: Свойства.

        /// <summary>
        /// Максимальное количество попыток на скачивание документа. <br />
        /// После истечения попыток, следует создать соответствующее предупреждение и вернуть пустой путь.
        /// </summary>
        int Attempts { get; init; }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Начинает процесс скачивания документа с удаленного ресурса.
        /// </summary>
        /// <param name="element">Элемент замены, полученный с сайта колледжа.</param>
        /// <returns>Путь к скачанному документу.</returns>
        string BeginDocumentDownload(ChangeElement element);
        #endregion
    }
}
