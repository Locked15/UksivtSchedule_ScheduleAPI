using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Cache.CachedTypes;
using ScheduleAPI.Models.Cache.CachedTypes.Basic;
using ScheduleAPI.Models.Result.Schedule.Changes;
using System.Text.Json.Serialization;

namespace ScheduleAPI.Models.Result.Schedule.Final
{
    /// <summary>
    /// Класс-обертка для итогового расписания. <br />
    /// Он содержит в себе итоговое расписание и примечания к нему.
    /// <br /><br />
    /// Это нужно, чтобы используя контроллер итогового расписания можно было получать дополнительные сведения о расписании.
    /// </summary>
    public class FinalDaySchedule : ICacheable<FinalDaySchedule, FinalDayScheduleCache>
    {
        #region Область: Свойства.

        /// <summary>
        /// Найден ли файл с заменами для указанного дня.
        /// </summary>
        public bool ChangesFound { get; set; }

        /// <summary>
        /// Были ли в файле найдены замены для указанной группы.
        /// </summary>
        public bool ActualChanges { get; set; }

        /// <summary>
        /// Дата, на которую предназначено итоговое расписание.
        /// </summary>
        public DateTime? ScheduleDate { get; set; }

        /// <summary>
        /// Итоговое расписания (с учетом замен) для указанного дня.
        /// </summary>
        public DaySchedule Schedule { get; set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        [JsonIgnore]
        public bool CachingIsEnabled { get; } = true;
        #endregion

        #region Область: Константы.

        private const string ToStringTemplate = """
            Changes Found: {0};
            Changes is Actual: {1};
            Final Schedule: 
            (
                {2}
            ).
            """;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Конструктор класса по умолчанию. <br />
        /// Записывает 'false' в логические свойства и новый экземпляр расписания в 'Schedule'.
        /// </summary>
        public FinalDaySchedule()
        {
            ChangesFound = false;
            ActualChanges = false;

            Schedule = new DaySchedule();
        }

        /// <summary>
        /// Конструктор класса. <br />
        /// Записывает 'false' в логические свойства. <br />
        /// Устанавливает датой расписания сформированную дату по указанному дню в пределах текущей недели.
        /// </summary>
        /// <param name="schedule">Итоговое расписание.</param>
        public FinalDaySchedule(DaySchedule schedule)
        {
            ChangesFound = false;
            ActualChanges = false;

            Schedule = schedule;
            ScheduleDate = schedule.Day?.GetIndexByDay().GetDateTimeInWeek();
        }

        /// <summary>
        /// Конструктор класса. <br />
        /// Данный конструктор предназначен для упрощения работы с контроллерами.
        /// </summary>
        /// <param name="baseSchedule">Базовое расписание.</param>
        /// <param name="changes">Замены для базового расписания.</param>
        public FinalDaySchedule(DaySchedule baseSchedule, ChangesOfDay? changes)
        {
            ChangesFound = changes?.ChangesFound ?? false;
            ActualChanges = changes?.NewLessons.Any() ?? false;

            Schedule = baseSchedule.MergeChanges(changes);
            ScheduleDate = changes?.ChangesDate;
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="args"><inheritdoc /></param>
        /// <returns><inheritdoc /></returns>
        /// <exception cref="NotImplementedException"></exception>
        public FinalDayScheduleCache? GenerateCachedValue(params object[] args)
        {
            var groupName = args.FirstOrDefault() as string ?? string.Empty;
            if (CachingIsEnabled && ChangesFound)
                return new FinalDayScheduleCache(this, groupName);
            else
                return null;
        }

        /// <summary>
        /// Метод для преобразования объекта в его строковый вариант.
        /// </summary>
        /// <returns>Строковая репрезентация объекта.</returns>
        public override string ToString()
        {
            string val = string.Format(ToStringTemplate, ChangesFound, ActualChanges, Schedule.ToString());

            return val;
        }

        /// <summary>
        /// Метод для сравнения объектов.
        /// </summary>
        /// <param name="obj">Объект, с которым нужно провести сравнение.</param>
        /// <returns>Равенство объектов.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is FinalDaySchedule day)
            {
                if (Schedule.Lessons.Count != day.Schedule.Lessons.Count || Schedule.Day != day.Schedule.Day)
                    return false;
                if (ActualChanges != day.ActualChanges || ChangesFound != day.ChangesFound)
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
