namespace ScheduleAPI.Models.Cache.CachedTypes.Basic
{
    /// <summary>
    /// Контракт на реализацию безопасного получения кэшированного элемента.
    /// </summary>
    /// <typeparam name="T">Обобщенный тип.</typeparam>
    public interface ISafeCacheGetter<T>
    {
        /// <summary>
        /// Получает кэшированный элемент безопасно (с проверкой времени кэширования).
        /// </summary>
        /// <returns>Безопасное значение.</returns>
        T? GetCacheSafely();
    }
}
