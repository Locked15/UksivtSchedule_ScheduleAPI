using ScheduleAPI.Models.Elements.Site;

namespace ScheduleAPI.Controllers.Data.Workers.Downloaders.Basic
{
    public abstract class AbstractPlatformDownloader : IDownloader
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int Attempts { get; init; } = 3;

        /// <summary>
        /// Свойство, содержащее оптимальное значение переменной 'UserAgent' для выбранной платформы.
        /// </summary>
        protected string UserAgentForPlatform { get; } = "";

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="attempts">Количество попыток на скачивание документа.</param>
        public AbstractPlatformDownloader(int attempts)
        {
            Attempts = attempts;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="element"><inheritdoc /></param>
        /// <returns><inheritdoc /></returns>
        public abstract string BeginDocumentDownload(ChangeElement element);

        /// <summary>
        /// Парсит строку с ссылкой на документ и преобразует её, подготавливая к скачиванию. <br />
        /// Готовая строка может быть использована с 'WebClient' (любой режим) или 'HttpClient' (только в асинхронном режиме).
        /// </summary>
        /// <param name="originalLink">Оригинальная ссылка на документ.</param>
        /// <returns>Готовая к скачиванию ссылка.</returns>
        protected abstract string GetDownloadableLinkForPlatform(string originalLink);

        /// <summary>
        /// Выполняет одну попытку скачивания документа с заменами.
        /// </summary>
        /// <param name="url">Готовая к скачиванию ссылка на документ для скачивания.</param>
        /// <returns>Путь к скачанному документу. 
        /// Если скачивание неудачно — пустая строка.</returns>
        protected abstract string TryToDownloadDocument(string url);

        /// <summary>
        /// Выполняет алгоритм скачивания документа. <br />
        /// Именно здесь создается объект, выполняющий скачивание и производится подключение.
        /// </summary>
        /// <param name="url">Готовая к скачиванию ссылка на документ.</param>
        /// <param name="baseDistonation">Базовое имя файла-документа.</param>
        /// <returns>Итоговое имя файла-документа. Пустая строка, если скачивание неудачно.</returns>
        protected abstract string ProcessDownload(string url, string baseDistonation);
    }
}
