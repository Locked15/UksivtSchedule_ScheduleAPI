namespace ScheduleAPI.Models.Cache
{
    /// <summary>
    /// Базовый класс любого элемента, который планируется кэшировать.
    /// <br />
    /// Кэшированный элемент — это объект, к которому недавно обращались и который был сохранен в памяти для ускорения последующих обращений к нему.
    /// </summary>
    public abstract class AbstractCacheElement<T> : ISafeCacheGetter<T>
    {
        #region Область: Свойства.

        /// <summary>
        /// Кэшируемый элемент.
        /// </summary>
        public T CachedElement { get; set; }

        /// <summary>
        /// Время кэширования данного элемента.
        /// </summary>
        public DateTime CachingTime { get; set; }

        /// <summary>
        /// Время, в течение которого кэшированный элемент считается актуальным.
        /// <br />
        /// Наследные классы могут переопределять его значение. Базовый показатель: 2 часа.
        /// </summary>
        protected virtual TimeSpan ExpirationTime { get; set; } = TimeSpan.FromHours(2);
        #endregion

        #region Область: Конструктор.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="cachingElement">Кэшируемый элемент.</param>
        public AbstractCacheElement(T cachingElement)
        {
            CachedElement = cachingElement;
            CachingTime = DateTime.Now;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Проверяет кэшированный элемент на его актуальность.
        /// </summary>
        /// <returns>Актуальность элемента.</returns>
        public virtual bool CheckToRelevance()
        {
            var currentTime = DateTime.Now;

            if (currentTime - CachingTime < ExpirationTime)
                return true;

            else
                return false;
        }

        /// <summary>
        /// Получает хэш-код кэшированного элемента.
        /// </summary>
        /// <returns>Хэш-код.</returns>
        public virtual int GetCachedValueHashCode()
        {
            return CachedElement?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Безопасно получает значение кэша.
        /// </summary>
        /// <returns>Если кэш актуален — его значение;
        /// В ином случае — "null".</returns>
        public virtual T? GetCacheSafety()
        {
            if (CheckToRelevance())
                return CachedElement;

            else
                return default;
        }
        #endregion
    }
}
