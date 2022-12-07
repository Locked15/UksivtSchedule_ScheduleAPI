using ScheduleAPI.Models.Cache.CachedTypes.Basic;
using ScheduleAPI.Models.Elements.Schedule;
using ScheduleAPI.Models.Elements.Schedule.Final;

namespace ScheduleAPI.Models.Cache.CachedTypes
{
    public class FinalDayScheduleCache : AbstractCacheElement<FinalDaySchedule>
    {
        /// <summary>
        /// Название группы, для которой указано финальное расписание.
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="cachingElement">Кэшируемый элемент.</param>
        /// <param name="groupName">Название группы, для которой предназначено расписание.</param>
        public FinalDayScheduleCache(FinalDaySchedule cachingElement, string groupName) : base(cachingElement)
        {
            GroupName = groupName;
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
        public override FinalDaySchedule? GetCacheSafely()
        {
            return base.GetCacheSafely();
        }
    }
}
