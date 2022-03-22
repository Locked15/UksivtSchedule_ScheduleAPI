using System.Net;

/// <summary>
/// Область кода с классом-помощником.
/// </summary>
namespace ScheduleAPI.Other.General
{
    /// <summary>
    /// Класс-помощник, нужный для различных задач.
    /// </summary>
    public static class Helper
    {
        #region Область: Константы.
        /// <summary>
        /// Константа, содержащая шаблон для создания ссылки для скачивания файла с Google Drive.
        /// </summary>
        private const String GoogleDriveDownloadLinkTemplate =
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
        public static String GetDownloadableFileLink(String originalLink)
        {
            String id = originalLink[..originalLink.LastIndexOf('/')];
            id = id[(id.LastIndexOf('/') + 1)..];

            return GoogleDriveDownloadLinkTemplate + id;
        }

        /// <summary>
        /// Метод для скачивания файла с заменами по обработанной ссылке.
        /// </summary>
        /// <param name="url">Ссылка на скачивание файла.</param>
        /// <param name="destination">Место, куда будет скачан файл.</param>
        /// <returns>Путь к скачанному файлу.</returns>
        /// <exception cref="ArgumentException">Отправленная ссылка была некорректна.</exception>
        public static String DownloadFileFromURL(String url)
        {
            String fileName = Path.GetRandomFileName();
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
                    client.UseDefaultCredentials = true;
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.31 Safari/537.36");

                    while (File.Exists(fileName))
                    {
                        fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".docx";
                    }

                    client.DownloadFile(new Uri(url), fileName);
                }

                catch (Exception e)
                {
                    Logger.WriteError(3, $"Обнаружена ошибка при скачивании файла с заменами: {e.Message}.");

                    return String.Empty;
                }
            }

            return fileName;
        }
        #endregion
    }
}
