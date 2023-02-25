using ScheduleAPI.Models.Elements.Site;
using ScheduleAPI.Controllers.API.Changes;
using ScheduleAPI.Controllers.Data.Workers.Downloaders;
using ScheduleAPI.Controllers.Data.Workers.Downloaders.Basic;

namespace ScheduleAPI.Controllers.Data.Getter
{
    /// <summary>
    /// Класс, предназначенный для выкачки документов.
    /// </summary>
    public static class DocumentGetter
    {
        /// <summary>
        /// Константа, содержащая ключевые элементы ссылки, определяющие платформу как Google Drive.
        /// </summary>
        private const string GoogleTemplate = "google";

        /// <summary>
        /// Выполняет цепочку вызовов, что приводит к скачиванию документа с указанной платформы.
        /// </summary>
        /// <param name="element">Элемент замены, полученный во время парса сайта колледжа.</param>
        /// <returns>Путь к скачанному документу.</returns>
        public static string DownloadChangesDocument(ChangeElement element)
        {
            try
            {
                IDownloader downloader = GetDownloaderInstanceByDocumentLink(element.LinkToDocument ?? string.Empty);

                return downloader.BeginDocumentDownload(element);
            }
            catch (InvalidOperationException)
            {
                ChangesController.Logger?.LogError("Не найдено определение класса для скачивания документа с текущей платформы.\nСсылка: {link}.", element.LinkToDocument);
            }
            catch (Exception ex)
            {
                ChangesController.Logger?.LogError("Произошла ошибка при скачивании документа: {error}.", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Создает экземпляр класса-скачивателя для платформы, на которой размещен документ с заменами.
        /// </summary>
        /// <param name="originalLink">Оригинальная ссылка на документ.</param>
        /// <returns>Экземпляр соответствующего класса.</returns>
        /// <exception cref="InvalidOperationException">Платформа с документом не поддерживается.</exception>
        private static IDownloader GetDownloaderInstanceByDocumentLink(string originalLink)
        {
            if (originalLink.Contains(GoogleTemplate))
                return new GoogleDriveDownloader();
            else
                throw new InvalidOperationException();
        }
    }
}
