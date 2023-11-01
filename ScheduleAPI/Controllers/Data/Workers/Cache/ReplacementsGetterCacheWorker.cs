using ScheduleAPI.Controllers.API.V1.Schedule.Replacements;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Models.Cache;
using ScheduleAPI.Models.Cache.CachedTypes;
using ScheduleAPI.Models.Elements.Documents;
using ScheduleAPI.Models.Result.Schedule.Replacements;

namespace ScheduleAPI.Controllers.Data.Workers.Cache
{
    /// <summary>
    /// Содержит вынесенную логику для работы с хранилищами кэша из класса "ReplacementsGetter".
    /// </summary>
    public class ReplacementsGetterCacheWorker
    {
        #region Область: Поля.

        /// <summary>
        /// Хранилище кэша, содержащее кэшированные замены для расписаний. <br />
        /// Прямое указание типов (вместо привязки к базовым элементам (как в "Dependency Inversion")), позволяет проще понять код.
        /// <br /><br />
        /// "ChangesOfDayCache" сразу видно, в отличие от "AbstractCacheElement<ReplacementsOfDay>".
        /// </summary>
        private readonly CachedVault<ReplacementsOfDay, ChangesOfDayCache> cachedChanges;

        /// <summary>
        /// Хранилище кэша, содержащее кэшированные документы с заменами для определенных дней.
        /// </summary>
        private readonly CachedVault<ReplacementsDocument, ReplacementsDocumentCache> cachedDocuments;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public ReplacementsGetterCacheWorker()
        {
            cachedChanges = new();
            cachedChanges.TryToRestoreCachedValues();

            cachedDocuments = new(8);
            cachedDocuments.TryToRestoreCachedValues();
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Пытается найти искомый объект замен среди кэшированных данных.
        /// Операция выполняется с помощью лямбда-операции, что при большом количестве данных может снизить производительность.
        /// </summary>
        /// <param name="dayIndex">Индекс дня, который необходимо найти.</param>
        /// <param name="groupName">Название группы, которую необходимо найти.</param>
        /// <returns>Результат поиска.</returns>
        public ReplacementsOfDay? TryToFindTargetCachedChangesValue(int dayIndex, string groupName)
        {
            var cachedElement = cachedChanges.Get(el =>
            {
                var basicTargetingDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
                var basicCachedValueDate = DateOnly.FromDateTime(el.CachedElement.ChangesDate.GetValueOrDefault(new DateTime(0)));

                return Helper.CheckDaysToEqualityIncludingFutureDates(basicTargetingDate, basicCachedValueDate) && groupName.Equals(el.GroupName);
            });

            return cachedElement;
        }

        /// <summary>
        /// Пытается найти искомый объект-документ с заменами среди кэшированных документов.
        /// К сожалению, в данный момент кэширование документов не работает должным образом.
        /// </summary>
        /// <param name="targetDate">Дата, на которую нужно найти документ с заменами.</param>
        /// <returns>Результат поиска.</returns>
        public ReplacementsDocument? TryToFindTargetCachedDocumentValue(DateOnly targetDate)
        {
            var cachedDocument = cachedDocuments.Get(doc =>
                                                     doc.CachedElement.DocumentDate.Equals(targetDate));

            return cachedDocument;
        }

        /// <summary>
        /// Пытается добавить отправленный объект в хранилище кэшированных значений.
        /// </summary>
        /// <param name="cacheableValue">Значение, которое необходимо закэшировать. Имеет динамический тип.</param>
        /// <param name="args">Аргументы, сопровождающие процесс кэширования, перечисленные через запятую.</param>
        /// <returns>Успех кэширования.</returns>
        public bool TryToAddValueToCachedVault(dynamic cacheableValue, params object[] args)
        {
            switch (cacheableValue)
            {
                case ReplacementsOfDay changes:
                    cachedChanges.Add(changes.GenerateCachedValue(args));
                    return true;
                case ReplacementsDocument document:
                    cachedDocuments.Add(document.GenerateCachedValue(args));
                    return true;

                default:
                    ReplacementsController.Logger?.LogWarning("Попытка кэшировать значение, которое не поддерживает сохранение в кэш.\n" +
                                                         "(TargetReplacementsGetter -> TryToAddValueToCachedVault: {type}).", cacheableValue.GetType() as Type);
                    return false;
            }
        }
        #endregion
    }
}
