using System.Text.Json.Serialization;

namespace ScheduleAPI.Models.Cache.CachedTypes.Basic
{
    /// <summary>
    /// Определяет класс как пригодный для кэширования.
    /// </summary>
    /// <typeparam name="T">Тип, который пригоден для кэширования. Может быть практически любым (например, 'ChangesOfDay').</typeparam>
    /// <typeparam name="Y">Соответствующий тип кэша, аналогичный базовому типу. Для приведенного ранее примера таковым является 'ChangesOfDayCache'.</typeparam>
    public interface ICacheable<T, Y> where Y : AbstractCacheElement<T>
    {
        /// <summary>
        /// Определяет, включено ли кэширование для данного типа или нет.
        /// Устанавливается в момент компиляции прямо в коде, не может быть изменено в среде выполнения.
        /// <br /> <br />
        /// Изначально это должен был быть контракт на константу, но в C# нельзя так делать, так что это read-only свойство.
        /// </summary>
        [JsonIgnore]
        bool CachingIsEnabled { get; }

        /// <summary>
        /// Создает новый кэшированный объект.
        /// Возвращает 'null', если кэширование для данного типа отключено.
        /// </summary>
        /// <param name="args">Список агрументов для проведения кэширования.</param>
        /// <returns>Объект типа кэша.</returns>
        Y? GenerateCachedValue(params object[] args);
    }
}
