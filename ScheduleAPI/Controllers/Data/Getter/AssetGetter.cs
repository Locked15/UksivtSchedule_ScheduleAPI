using System.Text.Json;
using ScheduleAPI.Controllers.API.Schedule;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Elements.Schedule;

namespace ScheduleAPI.Controllers.Data.Getter
{
    /// <summary>
    /// Класс, нужный для получения данных посредством заложенных в приложение ассетов.
    /// </summary>
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
        /// Метод для получения списка с отделениями-папками из ассетов.
        /// </summary>
        /// <returns>Список с отделениями.</returns>
        public List<string> GetFolders()
        {
            string currentPath = Path.Combine(Helper.GetSiteRootFolderPath(), "Assets", "Schedule");
            List<string> folders = Directory.GetDirectories(currentPath).ToList();
            folders = folders.Select(folder => folder.TrimEnd(Path.DirectorySeparatorChar)).ToList();

            return folders.Select(folder => Path.GetFileName(folder)).ToList();
        }

        /// <summary>
        /// Метод для получения списка с названиями направлений обучения из ассетов.
        /// </summary>
        /// <param name="folder">Папка отделения обучения.</param>
        /// <returns>Список с названиями направлений.</returns>
        public List<string> GetSubFolders(string folder)
        {
            try
            {
                string currentPath = Path.Combine(Helper.GetSiteRootFolderPath(), "Assets", "Schedule", folder);
                List<string> folders = Directory.GetDirectories(currentPath).ToList();

                folders = folders.Select(folder => folder.TrimEnd(Path.DirectorySeparatorChar)).ToList();

                return folders.Select(folder => Path.GetFileName(folder)).ToList();
            }

            catch (DirectoryNotFoundException)
            {
                ScheduleController.Logger?.Log(LogLevel.Warning, "Попытка обратиться к папке, которой не существует.");

                return Enumerable.Empty<string>().ToList();
            }
        }

        /// <summary>
        /// Метод для получения названий групп из ассетов.
        /// </summary>
        /// <param name="folder">Отделение обучения.</param>
        /// <param name="subFolder">Нужное направление обучения.</param>
        /// <returns>Список с группами по данному адресу.</returns>
        public List<string> GetGroupNames(string folder, string subFolder)
        {
            try
            {
                string currentPath = Path.Combine(Helper.GetSiteRootFolderPath(), "Assets", "Schedule", folder, subFolder);
                List<string> files = Directory.GetFiles(currentPath).ToList();

                files = files.Select(file => file.TrimEnd(Path.DirectorySeparatorChar)).ToList();

                return files.Select(file => Path.GetFileNameWithoutExtension(file)).ToList();
            }

            catch (DirectoryNotFoundException)
            {
                ScheduleController.Logger?.Log(LogLevel.Warning, "Попытка обратиться к папке, которой не существует.");

                return Enumerable.Empty<string>().ToList();
            }
        }

        /// <summary>
        /// Метод для получения расписания на указанный день.
        /// </summary>
        /// <param name="dayIndex">Индекс нужного дня.</param>
        /// <param name="groupName">Название нужной группы.</param>
        public DaySchedule GetDaySchedule(int dayIndex, string groupName)
        {
            groupName = groupName.ToUpper();
            (string fullPath, string groupBranch, string subFolder) = GetValues(groupName);
            fullPath = Path.Combine(fullPath, "Assets", "Schedule", groupBranch, subFolder, $"{groupName}.json");

            if (File.Exists(fullPath))
            {
                using (StreamReader reader = new(fullPath, System.Text.Encoding.Default))
                {
                    WeekSchedule? week = JsonSerializer.Deserialize<WeekSchedule>(reader.ReadToEnd(), new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    week?.DaySchedules.ForEach(day => day.Day = day.Day.GetTranslatedDay());

                    if (week == null || week.DaySchedules[dayIndex] == null)
                    {
                        ScheduleController.Logger?.Log(LogLevel.Error, "При получении данных (День) произошла ошибка: " +
                                                       "Группа — {groupName}, День — {daySchedule}.",
                                                       week?.GroupName, week?.DaySchedules[dayIndex]?.Day);
                    }
                    return week?.DaySchedules[dayIndex] ?? defaultDaySchedule;
                }
            }

            ScheduleController.Logger?.Log(LogLevel.Error, "Файл с расписанием не обнаружен: " +
                                           "Отделение — {groupBranch}, Подраздел — {subFolder}, Группа — {groupName}.",
                                           groupBranch, subFolder, groupName);
            return defaultDaySchedule;
        }

        /// <summary>
        /// Метод для получения расписания на неделю для указанной группы.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Расписание на неделю для указанной группы.</returns>
        public WeekSchedule GetWeekSchedule(string groupName)
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
        private (string, string, string) GetValues(string groupName)
        {
            try
            {
                (string, string, string) values = new()
                {
                    Item1 = environment.ContentRootPath,
                    Item2 = groupName.GetPrefixFromName(),
                    Item3 = groupName.GetSubFolderFromName()
                };

                return values;
            }

            catch
            {
                return new(string.Empty, string.Empty, string.Empty);
            }
        }
        #endregion
    }
}
