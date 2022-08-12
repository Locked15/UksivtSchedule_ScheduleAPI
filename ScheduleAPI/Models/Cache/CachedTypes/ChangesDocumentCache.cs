using NPOI.XWPF.UserModel;
using ScheduleAPI.Models.Cache.CachedTypes.Basic;

namespace ScheduleAPI.Models.Cache.CachedTypes
{
    /// <summary>
    /// Класс кэша для документа с заменами. <br />
    /// Так как Google Drive часто отказывает в скачивании документа с заменами, его кэширование может быть полезной мерой.
    /// </summary>
    public class ChangesDocumentCache : AbstractCacheElement<XWPFDocument>
    {
        /// <summary>
        /// Дата, для которой предназначен данный документ.
        /// </summary>
        public DateOnly DocumentDate { get; private set; }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="cachingValue">Документ с заменами, который нужно кэшировать.</param>
        /// <param name="documentDate">Дата, на которую предназначен данный документ.</param>
        public ChangesDocumentCache(XWPFDocument cachingValue, DateOnly documentDate) : base(cachingValue)
        {
            DocumentDate = documentDate;
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
        public override XWPFDocument? GetCacheSafety()
        {
            return base.GetCacheSafety();
        }
    }
}
