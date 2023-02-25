using ScheduleAPI.Models.Cache.CachedTypes.Basic;
using ScheduleAPI.Models.Elements.Documents;

namespace ScheduleAPI.Models.Cache.CachedTypes
{
    /// <summary>
    /// Класс кэша для документа с заменами. <br />
    /// Так как Google Drive часто отказывает в скачивании документа с заменами, его кэширование может быть полезной мерой.
    /// </summary>
    public class ChangesDocumentCache : AbstractCacheElement<ChangesDocument>
    {
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="cachingValue">Документ с заменами, который нужно кэшировать.</param>
        public ChangesDocumentCache(ChangesDocument cachingValue) : base(cachingValue)
        {

        }

        /// <summary>
        /// Получает хэш-код кэшированного элемента.
        /// </summary>
        /// <returns>Хэш-код.</returns>
        public override int GetCachedValueHashCode()
        {
            return CachedElement.GetHashCode();
        }

        /// <summary>
        /// Получает безопасное значение кэша.
        /// </summary>
        /// <returns>Если кэш ещё актуален — его значение;
        /// В ином случае — "null".</returns>
        public override ChangesDocument? GetCacheSafely()
        {
            return base.GetCacheSafely();
        }
    }
}
