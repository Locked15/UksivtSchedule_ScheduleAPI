using Newtonsoft.Json;
using ScheduleAPI.Other;
using ScheduleAPI.Other.General;

namespace ScheduleAPI.Models.Getter
{
    /// <summary>
    /// Класс, нужный для получения данных посредством заложенных в приложение ассетов.
    /// <br/>
    /// </summary>
    [Obsolete("Аварийный способ получения расписания при ошибках БД. " +
    "Обычное использование не рекомендуется.")]
    public class AssetGetter
    {
        #region Область: Поля.
        /// <summary>
        /// Поле, содержащее объект, содержащий информацию о среде выполнения API.
        /// </summary>
        private readonly IHostEnvironment environment;

        /// <summary>
        /// Поле, содержащее объект-расписание, возвращаемое по умолчанию при возникновении ошибок.
        /// </summary>
        private static readonly DaySchedule defaultDaySchedule;

        /// <summary>
        /// Поле, содержащее объект-расписание на неделю, возвращаемое по умолчанию при возникновении ошибок.
        /// </summary>
        private static readonly WeekSchedule defaultWeekSchedule;
        #endregion

        #region Область: Конструкторы класса.
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="env">Среда, в которой работает API.</param>
        public AssetGetter(IHostEnvironment env)
        {
            environment = env;
        }

        /// <summary>
        /// Статический конструктор класса.
        /// </summary>
        static AssetGetter()
        {
            defaultDaySchedule = new("Monday", Enumerable.Empty<Lesson>().ToList());

            defaultWeekSchedule = new("19П-3", Enumerable.Empty<DaySchedule>().ToList());
        }
        #endregion

        #region Область: Методы.
        /// <summary>
        /// Метод для получения расписания на указанный день.
        /// </summary>
        /// <param name="dayIndex">Индекс нужного дня.</param>
        /// <param name="groupName">Название нужной группы.</param>
        public DaySchedule GetDaySchedule(Int32 dayIndex, String groupName)
        {
            groupName = groupName.ToUpper();
            (String fullPath, String groupBranch, String subFolder) = GetValues(groupName);
            fullPath = Path.Combine(fullPath, "Assets", groupBranch, subFolder, groupName + ".json");

            if (File.Exists(fullPath))
            {
                using (StreamReader reader = new(fullPath, System.Text.Encoding.Default))
                {
                    // Чтобы избавиться от форматирования полностью, придется сперва получить сериализованный объект.
                    WeekSchedule? week = JsonConvert.DeserializeObject<WeekSchedule>(reader.ReadToEnd());
                    week?.Days.ForEach(day => day.Day = day.Day.GetTranslatedDay());

                    if (week == null || week.Days[dayIndex] == null)
                    {
                        Logger.WriteError(1, $"При получении данных (День) произошла ошибка: " +
                        $"Группа — {week?.GroupName}, День — {week?.Days[dayIndex]?.Day}.");
                    }
                    return week?.Days[dayIndex] ?? defaultDaySchedule;
                }
            }

            Logger.WriteError(1, $"Файл с расписанием не обнаружен: " +
            $"Отделение — {groupBranch}, Подраздел — {subFolder}, Группа — {groupName}.");
            return defaultDaySchedule;
        }

        /// <summary>
        /// Метод для получения расписания на неделю для указанной группы.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Расписание на неделю для указанной группы.</returns>
        public WeekSchedule GetWeekSchedule(String groupName)
        {
            groupName = groupName.ToUpper();
            List<DaySchedule> schedule = new(1);

            for (int i = 0; i < 7; i++)
            {
               schedule.Add(GetDaySchedule(i, groupName));
            }

            return new(groupName, schedule);
        }

        /// <summary>
        /// Внутренний метод для получения значений иерархии ассетов по названию группы.
        /// </summary>
        /// <param name="groupName">Название нужной группы.</param>
        /// <returns>Кортеж из значений:
        /// <br/>
        /// Полный путь до папки проекта (...\\ScheduleAPI\\ScheduleAPI);
        /// <br/>
        /// Название папки, определяющей отделение группы;
        /// <br/>
        /// Название папки, разделяющей ассеты по названиям групп (П, ВЕБ, БД и т.д.).
        /// </returns>
        private (String, String, String) GetValues(String groupName)
        {
            try
            {
                (String, String, String) values = new();
                values.Item1 = environment.ContentRootPath;
                values.Item2 = groupName.GetPrefixFromName();
                values.Item3 = groupName.GetSubFolderFromName();

                values.Item1 = values.Item1[0..values.Item1.LastIndexOf(Path.DirectorySeparatorChar)];

                return values;
            }

            catch
            {
                return new(String.Empty, String.Empty, String.Empty);
            }
        }
        #endregion
    }
}
