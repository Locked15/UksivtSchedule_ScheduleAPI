using System.Net;
using System.Text.RegularExpressions;
using ScheduleAPI.Controllers.Other.SiteParser;

namespace ScheduleAPI.Controllers.Other.General
{
    /// <summary>
    /// Класс-помощник, нужный для различных задач.
    /// </summary>
    public static class Helper
    {
        #region Область: Константы.

        /// <summary>
        /// Константа, содержащая регулярное выражение для получения ID документа с заменами.
        /// </summary>
        private const string DocumentIdExtractionRegExp =
                             @"[-\w]{25,}(?!.*[-\w]{25,})";

        /// <summary>
        /// Константа, содержащая ID "аварийного" документа с заменами, если получить ID из ссылки не удалось.
        /// </summary>
        private const string EmergencyDocumentId = 
                             "1et_FwkiPJ1g18gWspyRswjFpcyXeUK0H";

        /// <summary>
        /// Константа, содержащая шаблон для создания ссылки для скачивания файла с Google Drive.
        /// </summary>
        private const string GoogleDriveDownloadLinkTemplate =
                             "https://drive.google.com/uc?export=download&id=";
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод для получения рабочей ссылки для скачивания файла с заменами.
        /// <br/>
        /// Оригинальная (без обработки) ссылка скачивает поврежденный файл, так что её надо обработать.
        /// </summary>
        /// <param name="originalLink">Оригинальная ссылка на файл.</param>
        /// <returns>Обработанная и пригодная для скачивания ссылка.</returns>
        public static string GetDownloadableFileLink(string originalLink)
        {
            var idExtractor = new Regex(DocumentIdExtractionRegExp);
            var result = idExtractor.Match(originalLink);

            if (result.Success)
            {
                return string.Concat(GoogleDriveDownloadLinkTemplate, result.Groups[0].Value);
            }

            else
            {
                Logger.WriteError(2, "При извлечении ID документа из ссылки произошла ошибка.\nОбращение к аварийному документу...");

                // Чтобы избежать ошибок, мы указываем "безопасную" ссылку, к документу с заменами на 
                return string.Concat(GoogleDriveDownloadLinkTemplate, EmergencyDocumentId);
            }
        }

        /// <summary>
        /// Метод, пытающийся скачать нужный файл с серверов Google Drive.
        /// <br/>
        /// Так как иногда сервер возвращает ошибку скачивания, это все нужно учитывать.
        /// </summary>
        /// <param name="element">Элемент замен, содержащий ссылку на документ с заменами.</param>
        /// <param name="attempts">Максимальное число попыток скачать документ.</param>
        /// <returns>Путь к скачанному документу.</returns>
        public static string TryToDownloadFileFromGoogleDrive(ChangeElement element, int attempts = 3)
        {
            int currentAttempt = 0;
            string path = string.Empty;

            while (currentAttempt < attempts && string.IsNullOrEmpty(path))
            {
                try
                {
                    path = Helper.MakeAttemptToDownloadFileFromURL(Helper.GetDownloadableFileLink(element.LinkToDocument));

                    if (string.IsNullOrEmpty(path))
                    {
                        Thread.Sleep(100);
                    }
                }

                catch (ArgumentException e)
                {
                    Logger.WriteError(2, $"Преобразование ссылки прошло неудачно, точная информация: {e.Message}.", DateTime.Now);
                }

                finally
                {
                    currentAttempt++;
                }
            }

            return path;
        }

        /// <summary>
        /// Метод для скачивания файла с заменами по обработанной ссылке.
        /// </summary>
        /// <param name="url">Ссылка на скачивание файла.</param>
        /// <param name="destination">Место, куда будет скачан файл.</param>
        /// <returns>Путь к скачанному файлу.</returns>
        /// <exception cref="ArgumentException">Отправленная ссылка была некорректна.</exception>
        public static string MakeAttemptToDownloadFileFromURL(string url)
        {
            string fileName = Path.GetRandomFileName();
            fileName = Path.GetFileNameWithoutExtension(fileName) + ".docx";

            // Чтобы предотвратить попытки скачать файл по оригинальной ссылке, делаем проверку:
            if (!url.Contains(GoogleDriveDownloadLinkTemplate))
            {
                throw new ArgumentException("Отправленная ссылка некорректна.");
            }

            using (WebClient client = new())
            {
                try
                {
                    // Актуальность "User-Agent": 31.07.2022!
                    client.UseDefaultCredentials = true;
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36 Edg/103.0.1264.77");

                    while (File.Exists(fileName))
                    {
                        fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".docx";
                    }

                    client.DownloadFile(new Uri(url), fileName);
                }

                catch (Exception e)
                {
                    Logger.WriteError(3, $"Обнаружена ошибка при скачивании файла с заменами: {e.Message}.");

                    return string.Empty;
                }
            }

            return fileName;
        }
        #endregion
    }
}
