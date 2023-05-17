using ScheduleAPI.Models.Cache.CachedTypes.Basic;
using ScheduleAPI.Models.Result.Schedule.Replacements;

namespace ScheduleAPI.Models.Cache.CachedTypes
{
    /// <summary>
    /// Класс, наследный от универсального "AbstractCacheElement".
    /// <br /> <br />
    /// Специально предназначен для работы с объектами типа "ReplacementsOfDay".
    /// </summary>
    public class ChangesOfDayCache : AbstractCacheElement<ReplacementsOfDay>
    {
        /// <summary>
        /// Название группы, для которой предназначены замены.
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="cachingValue">Замены, которые нужно кэшировать.</param>
        /// <param name="groupName">Название группы, для которой предназначены замены.</param>
        public ChangesOfDayCache(ReplacementsOfDay cachingValue, string groupName) : base(cachingValue)
        {
            GroupName = groupName;
        }

        /// <summary>
        /// Получает хэш-код кэшированного элемента.
        /// </summary>
        /// <returns>Хэш-код.</returns>
        public override int GetCachedValueHashCode()
        {
            // Добавляем к основному хэшу результат хэширования названия группы (чтобы избежать коллизии).
            return CachedElement.GetHashCode() + GroupName.GetHashCode();
        }

        /// <summary>
        /// Получает безопасное значение кэша.
        /// </summary>
        /// <returns>Если кэш ещё актуален — его значение;
        /// В ином случае — "null".</returns>
        public override ReplacementsOfDay? GetCacheSafely()
        {
            return base.GetCacheSafely();
        }
    }
}
