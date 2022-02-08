using Bool = System.Boolean;

namespace ScheduleAPI.Models
{
    /// <summary>
    /// Класс, представляющий сущность замен на один день.
    /// <br/>
    /// Нужно для работы API.
    /// </summary>
    public class ChangesOfDay
    {
        #region Область: Свойства.
        /// <summary>
        /// Свойство, отвечающее за то, что элемент с заменами на указанный день найден.
        /// <br/>
        /// Это нужно, чтобы можно было выводить разные сообщения, в зависимости от того,
        /// не найдены ли замены для текущей даты вообще или не найдены только для указанной группы.
        /// </summary>
        public Bool ChangesFound { get; set; }

        /// <summary>
        /// Логическое значение, отвечающее за то, замены на весь день или нет.
        /// </summary>
        public Bool AbsoluteChanges { get; set; }

        /// <summary>
        /// Свойство с датой, на которую предназначены замены.
        /// </summary>
        public DateTime? ChangesDate { get; set; }

        /// <summary>
        /// Список с парами замен.
        /// </summary>
        public List<Lesson> NewLessons { get; set; }

        /// <summary>
        /// Свойство, содержащее значения замен по умолчанию (если иные не найдены).
        /// </summary>
        public static ChangesOfDay DefaultChanges { get; private set; }
		#endregion

		#region Область: Конструкторы.
		/// <summary>
		/// Конструктор класса по умолчанию.
		/// <br/>
		/// Нужен для заполнения значений через инциализацию.
		/// </summary>
		public ChangesOfDay()
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
        public ChangesOfDay(Bool absoluteChanges, List<Lesson> lessons)
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
        public ChangesOfDay(Bool absoluteChanges, DateTime? changesDate, List<Lesson> lessons)
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
        public ChangesOfDay(Bool changesFound, Bool absoluteChanges, DateTime? changesDate, List<Lesson> newLessons)
        {
            ChangesFound = changesFound;
            AbsoluteChanges = absoluteChanges;
            ChangesDate = changesDate;
            NewLessons = newLessons;
        }

        /// <summary>
        /// Статический конструктор класса.
        /// </summary>
        static ChangesOfDay()
        {
            DefaultChanges = new(false, Enumerable.Empty<Lesson>().ToList())
            {
                ChangesFound = false,
            };
        }
        #endregion
    }
}
