using ScheduleAPI.Models.Cache.CachedTypes;
using ScheduleAPI.Models.Cache.CachedTypes.Basic;
using System.Text.Json.Serialization;

namespace ScheduleAPI.Models.Result.Schedule.Replacements
{
    /// <summary>
    /// Класс, представляющий сущность замен на один день.
    /// <br/>
    /// Нужно для работы API.
    /// </summary>
    public class ReplacementsOfDay : ICacheable<ReplacementsOfDay, ChangesOfDayCache>
    {
        #region Область: Свойства.

        /// <summary>
        /// Свойство, отвечающее за то, что элемент с заменами на указанный день найден.
        /// <br/>
        /// Это нужно, чтобы можно было выводить разные сообщения, в зависимости от того,
        /// не найдены ли замены для текущей даты вообще или не найдены только для указанной группы.
        /// </summary>
        public bool ChangesFound { get; set; }

        /// <summary>
        /// Логическое значение, отвечающее за то, замены на весь день или нет.
        /// </summary>
        public bool AbsoluteChanges { get; set; }

        /// <summary>
        /// Свойство с датой, на которую предназначены замены.
        /// </summary>
        public DateTime? ChangesDate { get; set; }

        /// <summary>
        /// Список с парами замен.
        /// </summary>
        public List<Lesson> NewLessons { get; set; }

        [JsonIgnore]
        public bool CachingIsEnabled { get; } = true;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// <br/>
        /// Нужен для заполнения значений через инициализацию.
        /// </summary>
        public ReplacementsOfDay()
        {
            ChangesFound = false;
            AbsoluteChanges = false;
            NewLessons = Enumerable.Empty<Lesson>().ToList();
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="absoluteChanges">Замены на весь день?</param>
        /// <param name="lessons">Список с новыми парами.</param>
        public ReplacementsOfDay(bool absoluteChanges, List<Lesson> lessons)
        {
            AbsoluteChanges = absoluteChanges;
            NewLessons = lessons;
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="absoluteChanges">Замены на весь день?</param>
        /// <param name="changesDate">Дата, для которой предназначены замены.</param>
        /// <param name="lessons">Список с новыми парами.</param>
        public ReplacementsOfDay(bool absoluteChanges, DateTime? changesDate, List<Lesson> lessons)
        {
            AbsoluteChanges = absoluteChanges;
            ChangesDate = changesDate;
            NewLessons = lessons;
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="changesFound">Найден ли элемент с заменами на указанный день.</param>
        /// <param name="absoluteChanges">Замены на весь день?</param>
        /// <param name="changesDate">Дата (даже предполагаемая) замен.</param>
        /// <param name="newLessons">Список с новыми парами.</param>
        public ReplacementsOfDay(bool changesFound, bool absoluteChanges, DateTime? changesDate, List<Lesson> newLessons)
        {
            ChangesFound = changesFound;
            AbsoluteChanges = absoluteChanges;
            ChangesDate = changesDate;
            NewLessons = newLessons;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Вычисляет хэш-код для объекта.
        /// </summary>
        /// <returns>Хэш-код.</returns>
        public override int GetHashCode()
        {
            int baseValue = 0;

            baseValue += ChangesFound ? 100 : 50;
            baseValue += AbsoluteChanges ? 200 : 25;
            baseValue -= ChangesDate.GetValueOrDefault().GetHashCode();

            foreach (Lesson lesson in NewLessons)
            {
                baseValue += lesson.GetHashCode();
            }

            return baseValue;
        }

        public ChangesOfDayCache? GenerateCachedValue(params object[] args)
        {
            var groupName = args.FirstOrDefault() as string ?? string.Empty;
            if (CachingIsEnabled && ChangesFound)
                return new ChangesOfDayCache(this, groupName);
            else
                return null;
        }
        #endregion
    }
}
