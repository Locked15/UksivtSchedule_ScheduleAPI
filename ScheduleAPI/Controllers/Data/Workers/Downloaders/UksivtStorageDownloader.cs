using ScheduleAPI.Controllers.API.Replacements;
using ScheduleAPI.Controllers.Data.Workers.Downloaders.Basic;
using ScheduleAPI.Models.Elements.Site;
using System.Net;

namespace ScheduleAPI.Controllers.Data.Workers.Downloaders
{
    public class UksivtStorageDownloader : AbstractPlatformDownloader
    {
        private const string SiteRootName = "https://uksivt.ru";

        public UksivtStorageDownloader(int attempts = 3) : base(attempts)
        {
        }

        public override string BeginDocumentDownload(ReplacementNodeElement element)
        {
            int currentAttempt = 0;
            string path = string.Empty;

            while (currentAttempt < Attempts && string.IsNullOrEmpty(path))
            {
                try
                {
                    path = TryToDownloadDocument(GetDownloadableLinkForPlatform(element?.LinkToDocument ?? string.Empty));
                    if (string.IsNullOrEmpty(path))
                    {
                        Thread.Sleep(100);
                    }
                }

                catch (ArgumentException e)
                {
                    ReplacementsController.Logger?.Log(LogLevel.Warning, "Преобразование ссылки прошло неудачно, точная информация: {message}.", e.Message);
                }

                finally
                {
                    currentAttempt++;
                }
            }

            return path;
        }

        protected override string GetDownloadableLinkForPlatform(string originalLink)
        {
            if (originalLink.Contains("../"))
            {
                ReplacementsController.Logger?.LogWarning("Обнаружена относительная ссылка в объявлении замен.\nНормализация ссылки...");
                while (originalLink.Contains("../"))
                {
                    originalLink = originalLink.Replace("../", string.Empty);
                }

                originalLink = string.Concat(SiteRootName, '/', originalLink);
            }

            return originalLink;
        }

        protected override string TryToDownloadDocument(string url)
        {
            string fileName = Path.GetRandomFileName();
            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.{DocumentExtension}";

            return ProcessDownload(url, fileName);
        }

        protected override string ProcessDownload(string url, string baseDistonation)
        {
            using (WebClient client = new())
            {
                try
                {
                    client.UseDefaultCredentials = true;
                    client.Headers.Add("user-agent", UserAgentForPlatform);
                    while (File.Exists(baseDistonation))
                    {
                        baseDistonation = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.{DocumentExtension}";
                    }

                    client.DownloadFile(new Uri(url), baseDistonation);
                }
                catch (Exception e)
                {
                    ReplacementsController.Logger?.Log(LogLevel.Error, "Обнаружена ошибка при скачивании файла с заменами: {message}.", e.Message);

                    return string.Empty;
                }
            }

            return baseDistonation;
        }
    }
}
