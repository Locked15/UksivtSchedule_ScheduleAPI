using System.Text.Json;
using ScheduleAPI.Controllers.API.Schedule;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.Exceptions.Data;
using ScheduleAPI.Models.Result.Schedule;

namespace ScheduleAPI.Controllers.Data.Getter.Schedule
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
        /// Поле, содержащее объект-расписание, возвращаемое по умолчанию при возникновении ошибок. <br />
        /// Поля 'As-It', готово к выводу сразу, без дополнительных действий (вроде перевода названия дня).
        /// </summary>
        private static readonly DaySchedule defaultDaySchedule;
        #endregion

        #region Область: Константы.

        /// <summary>
        /// Содержит название файла со всеми расписаниями. <br />
        /// Включая расширение файла.
        /// </summary>
        private const string AllGroupsFileName = "AllGroups.json";
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
        static AssetGetter() =>
               defaultDaySchedule = new("Воскресенье", Enumerable.Empty<Lesson>().ToList());
        #endregion

        #region Область: Общие Методы.

        /// <summary>
        /// Метод для получения списка с отделениями-папками из ассетов. <br />
        /// Ранее называлось: 'GetFolders'.
        /// </summary>
        /// <returns>Список с отделениями.</returns>
        public List<string> GetBranches()
        {
            string currentPath = Path.Combine(Helper.GetSiteRootFolderPath(), "Assets", "Schedule");
            List<string> folders = Directory.GetDirectories(currentPath).ToList();
            folders = folders.Select(folder => folder.TrimEnd(Path.DirectorySeparatorChar)).ToList();

            return folders.Select(folder => Path.GetFileName(folder)).ToList();
        }

        /// <summary>
        /// Метод для получения списка с названиями направлений обучения из ассетов. <br />
        /// Ранее называлось: 'GetSubFolders'.
        /// </summary>
        /// <param name="folder">Папка отделения обучения.</param>
        /// <returns>Список с названиями направлений.</returns>
        public List<string> GetAffiliates(string folder)
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
        /// Метод для получения полного списка всех доступных групп, представленных в ассетах.
        /// </summary>
        /// <returns>Полный список с группами.</returns>
        public List<string> GetAllAvailableGroups()
        {
            List<string> groups = new List<string>(1);
            foreach (var folder in GetBranches())
            {
                foreach (var subFolder in GetAffiliates(folder))
                {
                    groups.AddRange(GetGroupNames(folder, subFolder));
                }
            }

            return groups;
        }

        /// <summary>
        /// Метод для получения расписания на указанный день.
        /// </summary>
        /// <param name="dayIndex">Индекс нужного дня.</param>
        /// <param name="groupName">Название нужной группы.</param>
        /// <exception cref="GroupNotFoundException"/>
        public DaySchedule GetDaySchedule(int dayIndex, string groupName)
        {
            groupName = groupName.ToUpper();
            (string fullPath, string groupBranch, string subFolder) = GetFolderStructureInfo(groupName);
            fullPath = Path.Combine(fullPath, "Assets", "Schedule", groupBranch, subFolder, $"{groupName}.json");

            if (File.Exists(fullPath))
            {
                using (StreamReader reader = new(fullPath, System.Text.Encoding.Default))
                {
                    WeekSchedule? week = JsonSerializer.Deserialize<WeekSchedule>(reader.ReadToEnd(), new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    week?.DaySchedules.ForEach(day => day.Day = day.Day?.GetTranslatedDay());

                    try
                    {
                        if (week == null || week.DaySchedules[dayIndex] == null)
                        {
                            ScheduleController.Logger?.Log(LogLevel.Error, "При получении данных (День) произошла ошибка: " +
                                                                           "Группа — {groupName}, День — {daySchedule}.",
                                                           week?.GroupName, week?.DaySchedules[dayIndex]?.Day);
                        }
                        return week?.DaySchedules[dayIndex] ?? defaultDaySchedule;
                    }
                    catch (Exception _) when (_ is ArgumentOutOfRangeException || _ is IndexOutOfRangeException)
                    {
                        ScheduleController.Logger?.Log(LogLevel.Error, "При возвращении расписания произошёл выход за пределы массива.");
                        return defaultDaySchedule;
                    }
                }
            }
            else
            {
                ScheduleController.Logger?.Log(LogLevel.Error, "Файл с расписанием не обнаружен: " +
                                               "Отделение — {groupBranch}, Подраздел — {subFolder}, Группа — {groupName}.",
                               groupBranch, subFolder, groupName);

                return ProcessNotFoundSchedule();
            }
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
        /// Метод для получения ПОЛНОГО списка ВСЕХ доступных расписаний из ассетов. <br />
        /// Может выполняться медленно, <strong>рекомендуется асинхронное выполнение</strong>.
        /// </summary>
        /// <returns>Список с объектами расписания на неделю.</returns>
        public List<WeekSchedule>? GetAllAvailableSchedules()
        {
            var path = Path.Combine(environment.ContentRootPath, "Assets", "Schedule", AllGroupsFileName);

            if (File.Exists(path))
            {
                return GetAllSchedulesFromUnitedFile();
            }
            else
            {
                var schedules = GetAllSchedulesFromCommonFiles();
                WriteSchedulesToUnitedFile(schedules);

                return GetAllSchedulesFromCommonFiles();
            }
        }

        /// <summary>
        /// Метод для получения списка всех доступных преподателей. <br />
        /// Этот метод выполняет LINQ-запрос к общему списку расписания. <br />
        /// Может выполняться медленно, <strong>рекомендуется асинхронное выполнение</strong>.
        /// </summary>
        /// <returns>Полный список со всеми преподавателями.</returns>
        public List<string> GetAllTeachers()
        {
            var schedules = GetAllAvailableSchedules();
            var teachers = new List<string>(schedules?.Count ?? 1);

            schedules?.ForEach(week =>
                               week.DaySchedules.ForEach(day =>
                                                         day.Lessons.ForEach(lesson =>
                                                                             teachers.Add(lesson?.Teacher ?? string.Empty))));
            teachers = teachers.DistinctBy(teacher =>
                                           teacher.RemoveStringChars(), StringComparer.OrdinalIgnoreCase).ToList();

            return teachers;
        }
        #endregion

        #region Область: Приватные Методы.

        /// <summary>
        /// Выполняет определенную логику в случае, если указанная группа не была обнаружена в списке доступных расписаний. <br />
        /// Таким образом эту логику можно легко переопределить.
        /// </summary>
        /// <returns>Расписание на день. В теории.</returns>
        /// <exception cref="GroupNotFoundException"></exception>
        private DaySchedule ProcessNotFoundSchedule()
        {
            throw new GroupNotFoundException();
        }

        /// <summary>
        /// Формирует список со всеми расписаниями на основе содержимого "объединенного" файла. <br />
        /// Этот файл хранит в себе все расписания, размещенные в одном месте. 
        /// <br /> <br />
        /// Получение данных из объединенного файла работает быстрее, чем обычный алгоритм.
        /// </summary>
        /// <returns>Полный список со ВСЕМИ расписаниями ВСЕХ групп.</returns>
        private List<WeekSchedule>? GetAllSchedulesFromUnitedFile()
        {
            var path = Path.Combine(environment.ContentRootPath, "Assets", "Schedule", AllGroupsFileName);
            using StreamReader sr = new(path, System.Text.Encoding.Default);

            return JsonSerializer.Deserialize<List<WeekSchedule>>(sr.ReadToEnd(), SerializeFormatter.JsonOptions);
        }

        /// <summary>
        /// Формирует список со всеми расписаниями на основе содержимого обычных файлов расписания. <br />
        /// Этот подход работает медленнее, чем "объединенный файл". <br />
        /// С другой стороны, если объединенный файл недоступен — это единственный вариант получения списка расписаний.
        /// </summary>
        /// <returns>Полный список расписания для ВСЕХ групп.</returns>
        private List<WeekSchedule>? GetAllSchedulesFromCommonFiles()
        {
            var groups = GetAllAvailableGroups();
            var schedules = new List<WeekSchedule>(groups.Count);
            foreach (var group in groups)
            {
                schedules.Add(GetWeekSchedule(group));
            }

            return schedules;
        }

        /// <summary>
        /// Записывает указанные расписания в специфический файл.
        /// </summary>
        /// <param name="schedules">Расписания, которые будут записаны в файл.</param>
        private void WriteSchedulesToUnitedFile(List<WeekSchedule>? schedules)
        {
            if (schedules != null)
            {
                var path = Path.Combine(environment.ContentRootPath, "Assets", "Schedule", AllGroupsFileName);
                var value = Newtonsoft.Json.JsonConvert.SerializeObject(schedules, Newtonsoft.Json.Formatting.Indented);

                using StreamWriter sw = new(path, false, System.Text.Encoding.Default);
                sw.Write(value);
            }
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
        private (string, string, string) GetFolderStructureInfo(string groupName)
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
