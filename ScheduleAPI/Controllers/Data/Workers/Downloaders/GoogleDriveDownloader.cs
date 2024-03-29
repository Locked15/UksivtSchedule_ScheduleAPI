﻿using ScheduleAPI.Controllers.API.V1.Schedule.Replacements;
using ScheduleAPI.Controllers.Data.Workers.Downloaders.Basic;
using ScheduleAPI.Models.Elements.Site;
using System.Net;
using System.Text.RegularExpressions;

namespace ScheduleAPI.Controllers.Data.Workers.Downloaders
{
    /// <summary>
    /// Класс-скачиватель, нацеленный на платформу Google Drive.
    /// Позволяет скачивать документы с указанной платформы.
    /// </summary>
    public class GoogleDriveDownloader : AbstractPlatformDownloader
    {
        #region Область: Константы.

        /// <summary>
        /// Константа, содержащая регулярное выражение для получения ID документа с заменами.
        /// </summary>
        private const string DocumentIdExtractionRegExp = @"[-\w]{25,}(?!.*[-\w]{25,})";

        /// <summary>
        /// Константа, содержащая ID "аварийного" документа с заменами, если получить ID из ссылки не удалось.
        /// Это документ с заменами на 01.06.2022!.
        /// </summary>
        private const string EmergencyDocumentId = "1et_FwkiPJ1g18gWspyRswjFpcyXeUK0H";

        /// <summary>
        /// Константа, содержащая шаблон для создания ссылки для скачивания файла с Google Drive.
        /// </summary>
        private const string GoogleDriveDownloadLinkTemplate = "https://drive.google.com/uc?export=download&id=";
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Инициализирует новый объект класса.
        /// </summary>
        /// <param name="attempts">Количество попыток на скачивание документа. Значение по умолчанию: 3.</param>
        public GoogleDriveDownloader(int attempts = 3) : base(attempts)
        {

        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод, пытающийся скачать нужный файл с серверов Google Drive.
        /// <br/>
        /// Так как иногда сервер возвращает ошибку скачивания, это все нужно учитывать.
        /// </summary>
        /// <param name="element">Элемент замен, содержащий ссылку на документ с заменами.</param>
        /// <param name="attempts">Максимальное число попыток скачать документ.</param>
        /// <returns>Путь к скачанному документу.</returns>
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

        /// <summary>
        /// Метод для получения рабочей ссылки для скачивания файла с заменами.
        /// <br/>
        /// Оригинальная (без обработки) ссылка скачивает поврежденный файл, так что её надо обработать.
        /// </summary>
        /// <param name="originalLink">Оригинальная ссылка на файл.</param>
        /// <returns>Обработанная и пригодная для скачивания ссылка.</returns>
        protected override string GetDownloadableLinkForPlatform(string originalLink)
        {
            var idExtractor = new Regex(DocumentIdExtractionRegExp);
            var result = idExtractor.Match(originalLink);

            if (result.Success)
            {
                return string.Concat(GoogleDriveDownloadLinkTemplate, result.Groups[0].Value);
            }

            else
            {
                ReplacementsController.Logger?.Log(LogLevel.Error, "При извлечении ID документа из ссылки произошла ошибка.\nОбращение к аварийному документу...");

                // Чтобы избежать ошибок, мы указываем "безопасную" ссылку. Парс этого документа всегда удачен (если в парсере нет ошибок).
                return string.Concat(GoogleDriveDownloadLinkTemplate, EmergencyDocumentId);
            }
        }

        /// <summary>
        /// Метод для скачивания файла с заменами по обработанной ссылке.
        /// </summary>
        /// <param name="url">Ссылка на скачивание файла.</param>
        /// <returns>Путь к скачанному файлу.</returns>
        /// <exception cref="ArgumentException">Отправленная ссылка была некорректна.</exception>
        protected override string TryToDownloadDocument(string url)
        {
            string fileName = Path.GetRandomFileName();
            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.{DocumentExtension}";

            return ProcessDownload(url, fileName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Отправленная ссылка не соответствует шаблону.</exception>
        protected override string ProcessDownload(string url, string baseDistonation)
        {
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
        #endregion
    }
}
