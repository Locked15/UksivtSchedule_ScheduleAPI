using ScheduleAPI.Controllers.Data.Getter.Changes;
using ScheduleAPI.Controllers.Data.Workers.Cache;
using ScheduleAPI.Models.Elements.Schedule;
using ScheduleAPI.Models.Elements.Schedule.Final;

namespace ScheduleAPI.Controllers.Data.Getter.Schedule
{
    public class FinalScheduleGetter
    {
        #region Область: Поля.

        private IHostEnvironment environment;

        private static FinalScheduleGetterCacheWorker cacheWorker;
        #endregion

        #region Область: Свойства.

        public int DayIndex { get; set; }

        public string GroupName { get; set; }
        #endregion

        #region Область: Конструкторы.

        public FinalScheduleGetter(int dayIndex, string groupName, IHostEnvironment env) 
        { 
            DayIndex = dayIndex;
            GroupName = groupName;

            environment = env;
        }

        static FinalScheduleGetter() => 
                cacheWorker = new FinalScheduleGetterCacheWorker();
        #endregion

        #region Область: Методы.

        public FinalDaySchedule GetDaySchedule()
        {
            if (cacheWorker.TryToFindTargetCachedChangesValue(DayIndex, GroupName) is FinalDaySchedule schedule && schedule != null)
                return schedule;
            else
                return GenerateFinalDaySchedule();
        }

        private FinalDaySchedule GenerateFinalDaySchedule()
        {
            var baseGetter = new AssetGetter(environment);
            var changesGetter = new TargetChangesGetter(DayIndex, GroupName);

            var schedule = baseGetter.GetDaySchedule(DayIndex, GroupName);
            var changes = changesGetter.GetDayChanges();

            var toReturn = new FinalDaySchedule(schedule, changes);
            cacheWorker.TryToAddValueToCachedVault(toReturn, GroupName);

            return toReturn;
        }

        public FinalWeekSchedule GetWeekSchedule()
        {
            var baseGetter = new AssetGetter(environment);
            var changesGetter = new TargetChangesGetter(default, GroupName);

            var schedule = baseGetter.GetWeekSchedule(GroupName);
            var changes = changesGetter.GetWeekChanges();

            return new FinalWeekSchedule(GroupName, schedule.DaySchedules, changes ?? Enumerable.Empty<ChangesOfDay>().ToList());
        }
        #endregion
    }
}
