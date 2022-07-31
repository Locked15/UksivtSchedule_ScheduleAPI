namespace ScheduleAPI.Models.Cache
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
        T? GetCacheSafety();
    }
}
