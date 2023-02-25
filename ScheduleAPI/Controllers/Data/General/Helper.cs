using ScheduleAPI.Controllers.API.Changes;
using ScheduleAPI.Models.Result.Schedule;

namespace ScheduleAPI.Controllers.Other.General
{
    /// <summary>
    /// Класс-помощник, нужный для различных задач.
    /// </summary>
    public static class Helper
    {
        #region Область: Методы.

        /// <summary>
        /// Вычисляет путь к директории проекта (директория, отображаемая "Обозревателем решений"). <br />
        /// </summary>
        /// <returns>Путь к директории проекта.</returns>
        public static string GetSiteRootFolderPath()
        {
            string basicAppPath;

            // При разработке приложения используется опция сборки "Debug", так что будет исполняться этот код.
#if DEBUG
            basicAppPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? string.Empty;
#endif

            // На сервере приложение работает под опцией сборки "Release", так что будет выполняться данный код.
#if RELEASE
            basicAppPath = AppDomain.CurrentDomain.BaseDirectory;
#endif

            return basicAppPath;
        }

        /// <summary>
        /// Проверяет две даты на соответствие дней недели, с учетом следующих недель (и начала следующего месяца).
        /// </summary>
        /// <param name="first">Первая дата для проверки (потенциально прошлая/текущая).</param>
        /// <param name="second">Вторая дата для проверки (потенциально текущая/будущая).</param>
        /// <returns>Результат проверки на идентичность.</returns>
        public static bool CheckDaysToEqualityIncludingFutureDates(DateOnly first, DateOnly second)
        {
            bool isFuture = first.GetWeekNumber() < second.GetWeekNumber() || 
                            first.Month < second.Month;
            bool daysAreEqual = first.DayOfWeek == second.DayOfWeek;

            return first == second || (isFuture && daysAreEqual);
        }

        /// <summary>
        /// Если замены на весь день, то возвращаемое значение содержит только замены.
        /// Чтобы добавить пустые пары, используется этот метод.
        /// </summary>
        /// <param name="lessons">Расписание замен.</param>
        /// <returns>Расписание замен с заполнением.</returns>
        public static List<Lesson> FillEmptyLessons(List<Lesson> lessons)
        {
            for (int i = 0; i < 7; i++)
            {
                bool missing = true;

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
        public static DaySchedule GetOnPractiseSchedule(string day)
        {
            List<Lesson> lessons = new(7);

            for (int i = 0; i < 7; i++)
            {
                lessons.Add(new Lesson(i, "Практика", null, null)
                {
                    Changed = true
                });
            }

            return new DaySchedule(day, lessons);
        }

        /// <summary>
        /// Статический метод, позволяющий получить расписание для группы с ликвидацией задолженностей.
        /// </summary>
        /// <param name="day">День недели для создания расписания.</param>
        /// <returns>Расписание на день для группы с ликвидацией задолженностей.</returns>
        public static DaySchedule GetDebtLiquidationSchedule(string day)
        {
            List<Lesson> lessons = new(7);

            for (int i = 0; i < 7; i++)
            {
                lessons.Add(new Lesson(i, "Ликвидация задолженностей", null, null)
                {
                    Changed = true
                });
            }

            return new DaySchedule(day, lessons);
        }

        /// <summary>
        /// Метод, позволяющий раскрыть сокращенную запись номеров пар в полный вид.
        /// </summary>
        /// <param name="lessonNumbers">Сокращенный (возможно) вид записи номеров пар.</param>
        /// <param name="wrappedLesson">Пара, которая должна быть проведена.</param>
        /// <returns>Полный вид пар.</returns>
        public static List<Lesson> ExpandWrappedLesson(string lessonNumbers, Lesson wrappedLesson)
        {
            /* Как показала практика, иногда в конце номеров пар могут оставить лишнюю запятую.
               Например: '1,2,3,' и тогда будет вызвана ошибка. Нужно обработать этот случай и удалить лишние символы. */
            string[] splatted = lessonNumbers.Trim(',', '.').Split(new char[] { ',', '.' });
            List<Lesson> toReturn = new(1);

            /* В отличие от Java, C# способен преобразовать строки с пробелами в целые числа, ...
               ... так что можно сразу переходить в развертке значений.                            */
            foreach (string splattedOne in splatted)
            {
                if (int.TryParse(splattedOne, out int parsed))
                {
                    toReturn.Add(new Lesson(parsed, wrappedLesson.Name, wrappedLesson.Teacher, wrappedLesson.Place));
                }

                else
                {
                    ChangesController.Logger?.Log(LogLevel.Warning, "Не удалось преобразовать значение в числовой тип при раскрытии номеров пар.");
                }
            }

            return toReturn;
        }
        #endregion
    }
}
