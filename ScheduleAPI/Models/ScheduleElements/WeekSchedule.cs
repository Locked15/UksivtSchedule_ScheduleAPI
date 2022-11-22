/// <summary>
/// Область с классом недельного расписания.
/// </summary>
namespace ScheduleAPI.Models.ScheduleElements
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
        public string GroupName { get; set; }

        /// <summary>
        /// Свойство, содержащее расписание группы на неделю.
        /// </summary>
        public List<DaySchedule> DaySchedules { get; set; }
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public WeekSchedule()
        {

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

        /// <summary>
        /// Метод для сравнения двух объектов.
        /// </summary>
        /// <param name="obj">Объект, с которым нужно провести сравнение.</param>
        /// <returns>Их равенство.</returns>
        public bool Equals(WeekSchedule obj)
        {
            if (GroupName != obj.GroupName || DaySchedules.Count != obj.DaySchedules.Count)
            {
                return false;
            }

            for (int i = 0; i < DaySchedules.Count; i++)
            {
                if (obj.DaySchedules[i].ToString() != DaySchedules[i].ToString())
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
