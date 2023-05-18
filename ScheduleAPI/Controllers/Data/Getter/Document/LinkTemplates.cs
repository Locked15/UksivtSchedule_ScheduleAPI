namespace ScheduleAPI.Controllers.Data.Getter.Document;

internal static class LinkTemplates
{
    /// <summary>
    /// Константа, содержащая ключевые элементы ссылки, определяющие платформу как Google Drive.
    /// </summary>
    public static List<string> GoogleTemplates { get; } = new(1) { "google" };

    /// <summary>
    /// Константа, содержащая ключевые элементы ссылки, определяющие платформу как локальное хранилище сайта.
    /// </summary>
    public static List<string> UksivtStorageTemplate { get; } = new(2) {
        "uksivt",
        "storage/files"
    };

    /// <summary>
    /// Проверяет отправленную ссылку на присутствие в отправленной коллекции шаблонов.
    /// Проверка НЕ зависит от регистра.
    /// </summary>
    /// <param name="originalLink">Оригинальная ссылка на сайт. Может быть пустой.</param>
    /// <param name="templates">Список с шаблонами ссылок для проверки.</param>
    /// <returns>Представлена ли отправленная ссылка в отправленном списке шаблонов.</returns>
    public static bool CheckLinkToPresent(string? originalLink, List<string> templates)
    {
        originalLink = originalLink?.ToLower() ?? string.Empty;
        return templates.Any(template =>
                             originalLink.Contains(template, StringComparison.CurrentCultureIgnoreCase));
    }
}
