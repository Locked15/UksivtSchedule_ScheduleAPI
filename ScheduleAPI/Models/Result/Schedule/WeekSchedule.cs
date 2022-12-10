using System.Text;
/// <summary>
/// Область с классом недельного расписания.
/// </summary>
namespace ScheduleAPI.Models.Result.Schedule
{
    /// <summary>
    /// Класс, представляющий сущность Расписания на неделю.
    /// </summary>
    public class WeekSchedule
    {
        #region Область: Свойства.

        /// <summary>
        /// Свойство, содержащее название группы.
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// Свойство, содержащее расписание группы на неделю.
        /// </summary>
        public List<DaySchedule> DaySchedules { get; set; }
        #endregion

        #region Область: Константы.

        private const string ToStringTemplate = """
            Group Name: {0};
            Day Schedules:
            (
                {1}.
            ).
            """;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public WeekSchedule()
        {
            DaySchedules = Enumerable.Empty<DaySchedule>().ToList();
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <param name="daySchedules">Список дней с расписанием.</param>
        public WeekSchedule(string groupName, List<DaySchedule> daySchedules)
        {
            GroupName = groupName;
            DaySchedules = daySchedules;
        }
        #endregion

        #region Область: Методы.

        public override string ToString()
        {
            StringBuilder builder = new();
            foreach (DaySchedule daySchedule in DaySchedules)
                builder.Append(daySchedule.ToString());

            return string.Format(ToStringTemplate, GroupName, builder.ToString());
        }

        /// <summary>
        /// Метод для сравнения двух объектов.
        /// </summary>
        /// <param name="obj">Объект, с которым нужно провести сравнение.</param>
        /// <returns>Их равенство.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is WeekSchedule week)
            {
                if (GroupName != week.GroupName || DaySchedules.Count != week.DaySchedules.Count)
                    return false;
                for (int i = 0; i < DaySchedules.Count; i++)
                {
                    if (week.DaySchedules[i].ToString() != DaySchedules[i].ToString())
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Возвращает хэш-код для объекта.
        /// </summary>
        /// <returns>Хэш-код.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
