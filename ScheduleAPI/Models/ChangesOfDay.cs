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
            AbsoluteChanges = false;
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
        /// Статический конструктор класса.
        /// </summary>
        static ChangesOfDay()
        {
            DefaultChanges = new(false, Enumerable.Empty<Lesson>().ToList());
        }
        #endregion
    }
}
