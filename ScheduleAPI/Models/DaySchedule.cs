using System.Text;

/// <summary>
/// Область кода с расписанием на день.
/// </summary>
namespace ScheduleAPI.Models
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
        public String Day { get; set; }

        /// <summary>
        /// Свойство, содержащее список пар для данного дня.
        /// </summary>
        public List<Lesson> Lessons { get; set; }
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public DaySchedule()
        {

        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="day">День недели.</param>
        /// <param name="lessons">Пары в этот день.</param>
        public DaySchedule(String day, List<Lesson> lessons)
        {
            Day = day;
            Lessons = lessons;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод, позволяющий произвести слияние оригинального расписания и замен.
        /// </summary>
        /// <param name="changes">Замены.</param>
        /// <param name="absoluteChanges">Замены на весь день?</param>
        /// <returns>Измененное расписание.</returns>
        public DaySchedule MergeChanges(List<Lesson> changes, Boolean absoluteChanges)
        {
            List<Lesson> mergedSchedule = Lessons;

            if (absoluteChanges)
            {
                //Чтобы избавиться от возможных проблем со ссылками в будущем, ...
                //... создаем новый объект:
                return new DaySchedule(Day, FillEmptyLessons(changes));
            }

            foreach (Lesson change in changes)
            {
                Int32 lessonIndex = change.Number;

                if (change.Name.ToLower().Equals("нет"))
                {
                    change.Name = null;
                }

                mergedSchedule[lessonIndex] = change;
            }

            return new DaySchedule(Day, mergedSchedule);
        }

        /// <summary>
        /// Если замены на весь день, то возвращаемое значение содержит только замены.
        /// Чтобы добавить пустые пары, используется этот метод.
        /// </summary>
        /// <param name="lessons">Расписание замен.</param>
        /// <returns>Расписание замен с заполнением.</returns>
        private List<Lesson> FillEmptyLessons(List<Lesson> lessons)
        {
            for (int i = 0; i < 7; i++)
            {
                Boolean missing = true;

                foreach (Lesson lesson in lessons)
                {
                    if (lesson.Number == i)
                    {
                        missing = false;

                        break;
                    }
                }

                if (missing)
                {
                    lessons.Add(new Lesson(i));
                }
            }

            //Добавленные "пустые" пары находятся в конце списка, так что ...
            //... мы сортируем их в порядке номера пар:
            lessons.OrderBy(lesson => lesson.Number);

            return lessons;
        }

        /// <summary>
        /// Статический метод, позволяющий получить расписание для группы с практикой.
        /// </summary>
        /// <param name="day">День недели для создания расписания.</param>
        /// <returns>Расписание на день для группы с практикой.</returns>
        public static DaySchedule GetOnPractiseSchedule(String day)
        {
            List<Lesson> lessons = new List<Lesson>(7);

            for (int i = 0; i < 7; i++)
            {
                lessons.Add(new Lesson(i, "Практика", null, null));
            }

            return new DaySchedule(day, lessons);
        }

        /// <summary>
        /// Статический метод, позволяющий получить расписание для группы с ликвидацией задолженностей.
        /// </summary>
        /// <param name="day">День недели для создания расписания.</param>
        /// <returns>Расписание на день для группы с ликвидацией задолженностей.</returns>
        public static DaySchedule GetDebtLiquidationSchedule(String day)
        {
            List<Lesson> lessons = new List<Lesson>(7);

            for (int i = 0; i < 7; i++)
            {
                lessons.Add(new Lesson(i, "Ликвидация задолженностей", null, null));
            }

            return new DaySchedule(day, lessons);
        }

        /// <summary>
        /// Метод для преобразования объекта в его строковый вариант.
        /// </summary>
        /// <returns>Строковая репрезентация объекта.</returns>
        public override String ToString()
        {
            StringBuilder toReturn = new(Day + ":\n" +
            "{");

            foreach (Lesson lesson in Lessons)
            {
                toReturn.Append("\n\t{").Append("\n\t\tНомер пары: ").Append(lesson.Number)
                .Append("\n\t\t").Append("Название пары: ").Append(lesson.Name).Append("\n\t\t")
                .Append("Кабинет: ").Append(lesson.Place).Append("\n\t\t")
                .Append("Преподаватель: ").Append(lesson.Teacher).Append("\n\t}");
            }

            toReturn.Append("\n}");

            return toReturn.ToString();
        }

        /// <summary>
        /// Метод для сравнения объектов.
        /// </summary>
        /// <param name="obj">Объект, с которым нужно провести сравнение.</param>
        /// <returns>Равенство объектов.</returns>
        public Boolean Equals(DaySchedule obj)
        {
            if (Lessons.Count != obj.Lessons.Count ||
            Day != obj.Day)
            {
                return false;
            }

            if (ToString() != obj.ToString())
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
