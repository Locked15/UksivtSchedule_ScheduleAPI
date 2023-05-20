using ScheduleAPI.Models.Result.Schedule.Replacements;

namespace ScheduleAPI.Models.Result.Schedule.Final
{
    /// <summary>
    /// Класс-обертка для итогового расписания. <br />
    /// Он содержит в себе итоговое расписание и примечания к нему. <br />
    /// Это обертка над расписанием на неделю.
    /// <br /><br />
    /// Это нужно, чтобы используя контроллер итогового расписания можно было получать дополнительные сведения о расписании.
    /// </summary>
    public class FinalWeekSchedule
    {
        #region Область: Свойства.

        public string? GroupName { get; set; }

        public List<FinalDaySchedule> FinalSchedules { get; set; }
        #endregion

        #region Область: Констукторы.

        public FinalWeekSchedule()
        {
            FinalSchedules = Enumerable.Empty<FinalDaySchedule>().ToList();
        }

        public FinalWeekSchedule(string? groupName, List<FinalDaySchedule> finalSchedules)
        {
            GroupName = groupName;
            FinalSchedules = finalSchedules;
        }

        public FinalWeekSchedule(string? groupName, List<DaySchedule> baseSchedules, List<ReplacementsOfDay> changesList)
        {
            GroupName = groupName;
            FinalSchedules = new List<FinalDaySchedule>(1);

            for (int i = 0; i < baseSchedules.Count; i++)
            {
                var schedule = baseSchedules[i];
                var changes = changesList.ElementAtOrDefault(i);

                FinalSchedules.Add(new FinalDaySchedule(schedule, changes));
            }
        }
        #endregion

        #region Область: Методы.

        public override bool Equals(object? obj)
        {
            if (obj is FinalWeekSchedule week)
            {
                if (GroupName != week.GroupName || FinalSchedules.Count != week.FinalSchedules.Count)
                    return false;
                for (int i = 0; i < FinalSchedules.Count; i++)
                {
                    if (week.FinalSchedules[i].ToString() != FinalSchedules[i].ToString())
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод для получения хэш-кода объекта.
        /// </summary>
        /// <returns>Хэш-код.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
