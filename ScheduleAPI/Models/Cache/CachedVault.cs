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
    public class CachedVault<T, Y> where T : AbstractCacheElement<Y> where Y : class
    {
        #region Область: Свойства.

        /// <summary>
        /// Список, содержащий кэшированные значения.
        /// </summary>
        public List<T> CachedValues { get; set; }

        /// <summary>
        /// Максимальное количество кэшированных элементов.
        /// <br />
        /// За счет конструктора по умолчанию его базовое значение: 10.
        /// </summary>
        public static int MaxCachedCount { get; private set; }

        /// <summary>
        /// Свойство-индексатор, использующееся для упрощения доступа к элементам списка.
        /// <br />
        /// Для обеспечения большей стабильности, через индексатор можно только получить элемент, но не присваивать его.
        /// </summary>
        /// <param name="index">Индекс элемента, который необходимо получить.</param>
        /// <returns>Элемент по указанному индексу.</returns>
        public T this[int index] => CachedValues[index];
        #endregion

        #region Область: Конструктор.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="maxCachedCount">Максимальное количество элементов для кэширования. Значение по умолчанию: 10.</param>
        public CachedVault(int maxCachedCount = 10)
        {
            MaxCachedCount = maxCachedCount;
            CachedValues = new List<T>(maxCachedCount);
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Функция добавления нового элемента в список кэшированных элементов.
        /// </summary>
        /// <param name="newElement">Новый элемент в списке.</param>
        /// <returns>Был ли добавлен новый элемент (в кэше его не было), либо нет (он там уже есть).</returns>
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
                    UpdateValues();

                CachedValues.Add(newElement);
                return true;
            }
        }

        /// <summary>
        /// Получает элемент из сводного кэша.
        /// </summary>
        /// <param name="predicate">Предикат (логическое условие), по которому необходимо найти элемент.</param>
        /// <returns>Значение кэшированного элемента. <br />
        /// Null, если он не был найден.</returns>
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

        /// <summary>
        /// Полностью обновляет кэшированные элементы. <br />
        /// В процессе обновления все старые элементы удаляются.
        /// Если в кэше слишком много элементов, то производится его очистка.
        /// </summary>
        public void UpdateValues()
        {
            var toRemove = new List<T>(1);

            CachedValues.ForEach(value =>
            {
                var actual = value.GetCacheSafety();

                if (actual == null || CachedValues.Count > MaxCachedCount)
                {
                    toRemove.Add(value);
                }
            });

            toRemove.ForEach(remove => CachedValues.Remove(remove));
        }
        #endregion
    }
}
