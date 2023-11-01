using ScheduleAPI.Controllers.API.V1.Schedule.Replacements;
using ScheduleAPI.Controllers.Data.Workers.Downloaders;
using ScheduleAPI.Controllers.Data.Workers.Downloaders.Basic;
using ScheduleAPI.Models.Elements.Site;

namespace ScheduleAPI.Controllers.Data.Getter.Document
{
    /// <summary>
    /// Класс, предназначенный для выкачки документов.
    /// </summary>
    public static class DocumentGetter
    {
        /// <summary>
        /// Выполняет цепочку вызовов, что приводит к скачиванию документа с указанной платформы.
        /// </summary>
        /// <param name="element">Элемент замены, полученный во время парса сайта колледжа.</param>
        /// <returns>Путь к скачанному документу.</returns>
        public static string DownloadChangesDocument(ReplacementNodeElement element)
        {
            try
            {
                IDownloader downloader = GetDownloaderInstanceByDocumentLink(element.LinkToDocument ?? string.Empty);

                return downloader.BeginDocumentDownload(element);
            }
            catch (InvalidOperationException)
            {
                ReplacementsController.Logger?.LogError("Не найдено определение класса для скачивания документа с текущей платформы.\nСсылка: {link}.", element.LinkToDocument);
            }
            catch (Exception ex)
            {
                ReplacementsController.Logger?.LogError("Произошла ошибка при скачивании документа: {error}.", ex.Message);
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
            if (LinkTemplates.CheckLinkToPresent(originalLink, LinkTemplates.GoogleTemplates))
                return new GoogleDriveDownloader();
            else if (LinkTemplates.CheckLinkToPresent(originalLink, LinkTemplates.UksivtStorageTemplate))
                return new UksivtStorageDownloader();
            else
                throw new InvalidOperationException();
        }
    }
}
