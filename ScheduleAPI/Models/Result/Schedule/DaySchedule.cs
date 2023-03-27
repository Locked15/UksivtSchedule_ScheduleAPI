using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Models.Result.Schedule.Changes;
using System.Text;

/// <summary>
/// Область кода с расписанием на день.
/// </summary>
namespace ScheduleAPI.Models.Result.Schedule
{
    /// <summary>
    /// Класс, представляющий расписание на один день.
    /// </summary>
    public class DaySchedule
    {
        #region Область: Свойства.

        /// <summary>
        /// Свойство, содержащее название текущего дня.
        /// </summary>
        public string? Day { get; set; }

        /// <summary>
        /// Свойство, содержащее список пар для данного дня.
        /// </summary>
        public List<Lesson> Lessons { get; set; }
        #endregion

        #region Область: Константы.

        private const string ToStringTemplate = """
            Day: {0};
            Lessons: 
            (
                {1}.
            ).
            """;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public DaySchedule()
        {
            Lessons = Enumerable.Empty<Lesson>().ToList();
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="day">День недели.</param>
        /// <param name="lessons">Пары в этот день.</param>
        public DaySchedule(string? day, List<Lesson> lessons)
        {
            Day = day;
            Lessons = lessons;
        }
        #endregion

        #region Область: Методы.

        public DaySchedule MergeChanges(ChangesOfDay? changes)
        {
            if (changes == null)
                return new DaySchedule(Day, Lessons);
            else
                return MergeChanges(changes.NewLessons, changes.AbsoluteChanges);
        }

        /// <summary>
        /// Метод, позволяющий произвести слияние оригинального расписания и замен.
        /// </summary>
        /// <param name="changes">Замены.</param>
        /// <param name="absoluteChanges">Замены на весь день?</param>
        /// <returns>Измененное расписание.</returns>
        public DaySchedule MergeChanges(List<Lesson> changes, bool absoluteChanges)
        {
            List<Lesson> mergedSchedule = Lessons;

            if (absoluteChanges)
            {
                //Чтобы избавиться от возможных проблем со ссылками в будущем, ...
                //... создаем новый объект:
                var toReturn = new DaySchedule(Day, Helper.FillEmptyLessons(changes));
                toReturn.Lessons.ForEach(les => les.Changed = true);
            }

            foreach (Lesson change in changes)
            {
                int lessonIndex = change.Number;
                if (change.Name?.ToLower()?.Equals("нет") ?? false)
                {
                    change.Name = null;
                }

                change.Changed = true;
                mergedSchedule[lessonIndex] = change;
            }

            return new DaySchedule(Day, mergedSchedule);
        }

        /// <summary>
        /// Метод для преобразования объекта в его строковый вариант.
        /// </summary>
        /// <returns>Строковая репрезентация объекта.</returns>
        public override string ToString()
        {
            StringBuilder builder = new();
            foreach (Lesson lesson in Lessons)
                builder.Append(lesson.ToString());

            return string.Format(ToStringTemplate, Day, builder.ToString());
        }

        /// <summary>
        /// Метод для сравнения объектов.
        /// </summary>
        /// <param name="obj">Объект, с которым нужно провести сравнение.</param>
        /// <returns>Равенство объектов.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is DaySchedule day)
            {
                if (Lessons.Count != day.Lessons.Count || Day != day.Day)
                    return false;
                if (ToString() != day.ToString())
                    return false;

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
