/// <summary>
/// Область с классом пары.
/// </summary>
namespace ScheduleAPI.Models.Elements.Schedule
{
    /// <summary>
    /// Класс, представляющий сущность пары.
    /// </summary>
    public class Lesson
    {
        #region Область: Свойства.

        /// <summary>
        /// Свойство, содержащее номер пары.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Свойство, содержащее название пары.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Свойство, содержащее имя преподавателя.
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// Свойство, содержащее место проведения пары.
        /// </summary>
        public string Place { get; set; }
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса по умолчанию.
        /// </summary>
        public Lesson()
        {

        }

        /// <summary>
        /// Конструктор класса для заполнения пустой пары.
        /// </summary>
        /// <param name="number">Номер пары.</param>
        public Lesson(int number)
        {
            Number = number;

            Name = null;
            Teacher = null;
            Place = null;
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="number">Номер пары.</param>
        /// <param name="name">Названия пары.</param>
        /// <param name="teacher">Имя преподавателя.</param>
        /// <param name="place">Место проведения.</param>
        public Lesson(int number, string name, string teacher, string place)
        {
            Number = number;
            Name = name;
            Teacher = teacher;
            Place = place;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод для проверки пары на полноту (наличие самой пары).
        /// </summary>
        /// <returns>Логическое значение, отвечающее за полноту.</returns>
        public bool CheckHaveValue()
        {
            return Name != null;
        }
        #endregion
    }
}
