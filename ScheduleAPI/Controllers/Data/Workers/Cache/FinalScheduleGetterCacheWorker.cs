using ScheduleAPI.Controllers.API.Changes;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Cache.CachedTypes;
using ScheduleAPI.Models.Cache;
using ScheduleAPI.Models.Elements.Schedule.Final;

namespace ScheduleAPI.Controllers.Data.Workers.Cache
{
    public class FinalScheduleGetterCacheWorker
    {
        #region Область: Поля.

        /// <summary>
        /// Хранилище кэша, содержащее кэшированные финальные расписания. <br />
        /// Прямое указание типов (вместо привязки к базовым элементам (как в "Dependency Inversion")), позволяет проще понять код.
        /// </summary>
        private readonly CachedVault<FinalDaySchedule, FinalDayScheduleCache> cachedSchedules;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public FinalScheduleGetterCacheWorker()
        {
            cachedSchedules = new();
            cachedSchedules.TryToRestoreCachedValues();
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
        public FinalDaySchedule? TryToFindTargetCachedChangesValue(int dayIndex, string groupName)
        {
            var cachedElement = cachedSchedules.Get(el =>
            {
                var basicTargetingDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek());
                var basicCachedValueDate = DateOnly.FromDateTime(el.CachedElement.ScheduleDate.GetValueOrDefault(new DateTime(0)));

                return Helper.CheckDaysToEqualityIncludingFutureDates(basicTargetingDate, basicCachedValueDate) && groupName.Equals(el.GroupName);
            });

            return cachedElement;
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
                case FinalDaySchedule schedule:
                    cachedSchedules.Add(schedule.GenerateCachedValue(args));
                    return true;

                default:
                    ChangesController.Logger?.LogWarning("Попытка кэшировать значение, которое не поддерживает сохранение в кэш.\n" +
                                                         "(FinalScheduleGetterCacheWorker -> TryToAddValueToCachedVault: {type}).", cacheableValue.GetType() as Type);
                    return false;
            }
        }
        #endregion
    }
}
