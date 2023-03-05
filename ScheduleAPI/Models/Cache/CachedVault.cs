using System.Text;
using System.Text.Json;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Controllers.ViewsControllers;
using ScheduleAPI.Models.Cache.CachedTypes.Basic;
using ScheduleAPI.Models.Elements;

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
    /// <typeparam name="T">Целевой тип, который будет храниться в кэше (для предыдущего примера таковым будет "ChangesOfDay").</typeparam>
    /// <typeparam name="Y">Любой специфический тип кэшированных данных (например: "ChangesOfDayCache").</typeparam>
    public class CachedVault<T, Y> where T : ICacheable<T, Y> where Y : AbstractCacheElement<T>
    {
        #region Область: Константы.

        private const string CacheFolderName = ".saved-cache";
        #endregion

        #region Область: Свойства.

        /// <summary>
        /// Список, содержащий кэшированные значения.
        /// </summary>
        public List<Y> CachedValues { get; set; }

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
        public Y this[int index] => CachedValues[index];
        #endregion

        #region Область: Функции инициализации.

        /// <summary>
        /// Конструктор класса. <br />
        /// Так как возможности отредактировать "AppIdleTime" нет (время до выключения приложения), то о максимальном размере пула кэша можно не волноваться, поэтому значение по умолчанию изменено на 50.
        /// </summary>
        /// <param name="maxCachedCount">Максимальное количество элементов для кэширования. Значение по умолчанию: 15.</param>
        public CachedVault(int maxCachedCount = 15)
        {
            MaxCachedCount = maxCachedCount;
            CachedValues = new List<Y>(maxCachedCount);
        }

        /// <summary>
        /// Выполняет попытку восстановить кэшированные значения из сохраненной копии. <br />
        /// Сохраненные копии кэша хранятся в ассетах.
        /// </summary>
        /// <returns>Успех восстановления.</returns>
        public bool TryToRestoreCachedValues()
        {
            string pathToSavedCache = GetPathToSavedCacheFile();

            if (File.Exists(pathToSavedCache))
            {
                try
                {
                    using var stream = new StreamReader(pathToSavedCache, Encoding.Default);
                    CachedValues = JsonSerializer.Deserialize<List<Y>>(stream.ReadToEnd()) ??
                                   throw new Exception("При попытке создать поток чтения сохраненного кэша было получено значение \'null\'.");

                    return true;
                }

                catch (Exception ex)
                {
                    HomeController.BaseLogger?.Log(LogLevel.Warning, "При десериализации кэша из сохраненного значения произошла ошибка.\nТочный текст ошибки: {message}.", ex.Message);
                }
            }

            return false;
        }
        #endregion

        #region Область: Функции работы с кэшем.

        /// <summary>
        /// Функция добавления нового элемента в список кэшированных элементов. <br />
        /// По умолчанию игнорирует пустые (null) элементы и не добавляет их в хранилище.
        /// </summary>
        /// <param name="newElement">Новый элемент в списке.</param>
        /// <param name="insertNullValues">Определяет, будут ли вносится пустые (null) значения в хранилище кэша. Значение по умолчанию: false.</param>
        /// <returns>Был ли добавлен новый элемент (в кэше его не было), либо нет (он там уже есть).</returns>
        public bool Add(Y newElement, bool insertNullValues = false)
        {
            if (newElement != null)
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
                        UpdateCachedValuesList();

                    CachedValues.Add(newElement);
                    UpdateSavedCacheAsync();

                    return true;
                }
            }
            else if (insertNullValues)
            {
                CachedValues.Add(newElement);
                UpdateSavedCacheAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Получает элемент из сводного кэша. <br />
        /// По умолчанию из выборки исключаются пустые элементы (null), но это поведение можно изменить.
        /// </summary>
        /// <param name="predicate">Предикат (логическое условие), по которому необходимо найти элемент.</param>
        /// <param name="saveNullValues">Устанавливает, будут ли учитываться пустые (null) значения в выборке. Значение по умолчанию: false.</param>
        /// <returns>Значение кэшированного элемента. <br />
        /// Null, если он не был найден.</returns>
        public T? Get(Func<Y, bool> predicate, bool saveNullValues = false)
        {
            var predicatedCachedValue = CachedValues.Where(el => el != null || saveNullValues).FirstOrDefault(el => predicate(el));

            if (predicatedCachedValue != null)
            {
                var result = predicatedCachedValue.GetCacheSafely();

                if (result == null)
                {
                    // Чтобы оптимизировать работу (не проверять коллекцию полностью), удаляем только один устаревший элемент, после чего перезаписываем запись кэша.
                    CachedValues.Remove(predicatedCachedValue);
                    UpdateSavedCacheAsync();

                    return default;
                }

                else
                {
                    return result;
                }
            }

            else
            {
                return default;
            }
        }

        /// <summary>
        /// Выполняет полное обновление кэша текущего хранилища: <br/>
        /// 1. Проверяет все значения в кэше и удаляет устаревшие; <br/>
        /// 2. Обновляет файл с сохраненным кэшем.
        /// </summary>
        public void UpdateCurrentVaultCachedValues()
        {
            UpdateCachedValuesList();
            UpdateSavedCacheAsync();
        }

        /// <summary>
        /// Очищает текущее хранилище кэша.
        /// </summary>
        /// <param name="deleteSavedCache">Удалить сохраненные значения кэша текущего хранилища? Значение по умолчанию: True.</param>
        public void ClearCurrentVaultCachedValues(bool deleteSavedCache = true)
        {
            CachedValues.Clear();

            if (deleteSavedCache)
            {
                try
                {
                    File.Delete(GetPathToSavedCacheFile());
                }

                catch
                {
                    HomeController.BaseLogger?.Log(LogLevel.Warning, "Удалить сохраненный кэш не удалось.");
                }
            }
        }
        #endregion

        #region Область: Внутренние функции.

        /// <summary>
        /// Полностью обновляет кэшированные элементы. <br />
        /// В процессе обновления все старые элементы удаляются.
        /// Если в кэше слишком много элементов, то производится его очистка.
        /// </summary>
        private void UpdateCachedValuesList()
        {
            var toRemove = new List<Y>(1);

            CachedValues.ForEach(value =>
            {
                var actual = value.GetCacheSafely();

                if (actual == null || CachedValues.Count > MaxCachedCount)
                {
                    toRemove.Add(value);
                }
            });

            toRemove.ForEach(remove => CachedValues.Remove(remove));
        }

        /// <summary>
        /// Асинхронно обновляет файл с сохраненными кэшированными значениями.
        /// </summary>
        private async void UpdateSavedCacheAsync()
        {
            await Task.Run(() =>
            {
                string pathToSavedCache = GetPathToSavedCacheFile();

                try
                {
                    using var stream = new StreamWriter(pathToSavedCache, false, Encoding.Default);

                    var serializedValue = JsonSerializer.Serialize(CachedValues, JsonSettingsModel.JsonOptions);
                    stream.Write(serializedValue);
                }

                catch (Exception ex)
                {
                    HomeController.BaseLogger?.Log(LogLevel.Warning, "При сохранении значений кэша в постоянный файл произошла ошибка. Точный текст: {message}.", ex.Message);
                }
            });
        }

        /// <summary>
        /// Вычисляет путь до файла с сохранением значений кэша текущего хранилища.
        /// </summary>
        /// <returns>Путь к директории.</returns>
        private static string GetPathToSavedCacheFile()
        {
            string cachedValueClassName = typeof(Y).Name;
            string pathToSavedCache = Path.Combine(Helper.GetSiteRootFolderPath(), "Assets", CacheFolderName, $"{cachedValueClassName}.cache");

            return pathToSavedCache;
        }
        #endregion
    }
}
