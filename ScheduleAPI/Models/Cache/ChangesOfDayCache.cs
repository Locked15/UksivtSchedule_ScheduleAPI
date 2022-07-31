using ScheduleAPI.Models.ScheduleElements;

namespace ScheduleAPI.Models.Cache
{
    public class ChangesOfDayCache : AbstractCacheElement<ChangesOfDay>
    {
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="cachingValue">Замены, которые нужно кэшировать.</param>
        public ChangesOfDayCache(ChangesOfDay cachingValue) : base(cachingValue)
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
        public override ChangesOfDay? GetCacheSafety()
        {
            return base.GetCacheSafety();
        }
    }
}
