namespace ScheduleAPI.Models.Cache
{
    /// <summary>
    /// Хранилище кэшированных данных. 
    /// <br />
    /// В реализации использует обобщенные типы и функции, реализованные в элементах кэшированных классов, 
    /// что обеспечивает совместимость с любым типом, унаследованном от "AbstractCacheElement<T>".
    /// <br /><br />
    /// Несоответствие отправленных типов будет расценено как ошибки компиляции.
    /// </summary>
    /// <typeparam name="T">Любой специфический тип кэшированных данных (например: "ChangesOfDayCache").</typeparam>
    /// <typeparam name="Y">Целевой тип, который будет храниться в кэше (для предыдущего примера таковым будет "ChangesOfDay").</typeparam>
    public class CachedVault<T, Y> where T : AbstractCacheElement<Y> 
                                   where Y : class
    {

        public List<T> CachedValues { get; set; }


        public static int MaxCachedCount { get; private set; }


        public CachedVault(int maxCachedCount = 10)
        {
            MaxCachedCount = maxCachedCount;
            CachedValues = new List<T>(maxCachedCount);
        }


        public bool Add(T newElement)
        {
            var repeating = CachedValues.Any(el => el.GetCachedValueHashCode() == newElement.GetCachedValueHashCode());
            var oldValueExpirationState = repeating && CachedValues.First(el => el.GetCachedValueHashCode() == newElement.GetCachedValueHashCode()).CheckToRelevance();

            if (repeating && !oldValueExpirationState)
            {
                return false;
            }

            else
            {
                if (repeating)
                    CachedValues.Remove(CachedValues.First(el => el.GetCachedValueHashCode() == newElement.GetCachedValueHashCode()));

                if (CachedValues.Count >= MaxCachedCount)
                    CachedValues.RemoveAt(0);

                CachedValues.Add(newElement);
                return true;
            }
        }


        public Y? Get(Func<T, bool> predicate)
        {
            var predicatedCachedValue = CachedValues.FirstOrDefault(el => predicate(el));

            if (predicatedCachedValue != null)
            {
                var result = predicatedCachedValue.GetCacheSafety();

                if (result == null)
                {
                    CachedValues.Remove(predicatedCachedValue);

                    return null;
                }

                else
                {
                    return result;
                }
            }

            else
            {
                return null;
            }
        }


        public void UpdateValues()
        {
            CachedValues.ForEach(value =>
            {
                if (value is ISafeCacheGetter<T> returnValue)
                {
                    var actual = returnValue.GetCacheSafety();

                    if (actual == null)
                    {
                        value = null;
                    }
                }
            });

            CachedValues.RemoveAll(val => val == null);
        }
    }
}
