using ScheduleAPI.Controllers.Data.Getter.Changes;
using ScheduleAPI.Controllers.Data.Workers.Cache;
using ScheduleAPI.Models.Elements.Schedule.Changes;
using ScheduleAPI.Models.Elements.Schedule.Final;

namespace ScheduleAPI.Controllers.Data.Getter.Schedule
{
    public class FinalScheduleGetter
    {
        #region Область: Поля.

        private IHostEnvironment environment;

        private static FinalScheduleGetterCacheWorker cacheWorker;
        #endregion

        #region Область: Конструкторы.

        public FinalScheduleGetter(IHostEnvironment env) => 
                environment = env;

        static FinalScheduleGetter() => 
                cacheWorker = new FinalScheduleGetterCacheWorker();
        #endregion

        #region Область: Методы.

        public FinalDaySchedule GetDaySchedule(int dayIndex, string groupName)
        {
            if (cacheWorker.TryToFindTargetCachedFinalScheduleValue(dayIndex, groupName) is FinalDaySchedule schedule && schedule != null)
                return schedule;
            else
                return GenerateFinalDaySchedule(dayIndex, groupName);
        }

        private FinalDaySchedule GenerateFinalDaySchedule(int dayIndex, string groupName)
        {
            var baseGetter = new AssetGetter(environment);
            var changesGetter = new TargetChangesGetter(dayIndex, groupName);

            var schedule = baseGetter.GetDaySchedule(dayIndex, groupName);
            var changes = changesGetter.GetDayChanges();

            var toReturn = new FinalDaySchedule(schedule, changes);
            cacheWorker.TryToAddValueToCachedVault(toReturn, groupName);

            return toReturn;
        }

        public FinalWeekSchedule GetWeekSchedule(int dayIndex, string groupName)
        {
            var baseGetter = new AssetGetter(environment);
            var changesGetter = new TargetChangesGetter(default, groupName);

            var schedule = baseGetter.GetWeekSchedule(groupName);
            var changes = changesGetter.GetWeekChanges();

            return new FinalWeekSchedule(groupName, schedule.DaySchedules, changes ?? Enumerable.Empty<ChangesOfDay>().ToList());
        }
        #endregion
    }
}
