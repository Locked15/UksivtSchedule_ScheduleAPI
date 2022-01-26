using ScheduleAPI.Other;
using ScheduleAPI.Other.General;
using ScheduleAPI.Other.SiteParser;
using ScheduleAPI.Other.DocumentParser;

namespace ScheduleAPI.Models.Getter
{
    /// <summary>
    /// Класс, обертывающий функционал получения замен.
    /// </summary>
    public class ChangesGetter
    {
        #region Область: Поля.
        /// <summary>
        /// Поле, содержащее информацию о окружении приложения.
        /// </summary>
        private readonly IHostEnvironment environment;
        #endregion

        #region Область: Конструктор.
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="env">Окружение приложения.</param>
        public ChangesGetter(IHostEnvironment env)
        {
            environment = env;
        }
        #endregion

        #region Область: Методы.
        /// <summary>
        /// Метод для получения замен на день.
        /// <br/>
        /// <strong>РЕКОМЕНДУЕТСЯ АСИНХРОННОЕ ВЫПОЛНЕНИЕ.</strong>
        /// </summary>
        /// <param name="dayIndex">Индекс дня.</param>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Объект, содержащий замены для группы.</returns>
        public ChangesOfDay GetDayChanges(Int32 dayIndex, String groupName)
        {
            List<MonthChanges> changes = default;
            ChangeElement element = default;

            #region Подобласть: Обрабатываем возможные ошибки.
            try
            {
                changes = new Parser().ParseAvailableNodes();
                element = changes.TryToFindElementByNameOfDayWithoutPreviousWeeks(dayIndex.GetDayByIndex());
            }

            catch (Exception ex)
            {
                Logger.WriteError(3, $"При получении замен произошла ошибка парса страницы: {ex.Message}.");

                return ChangesOfDay.DefaultChanges;
            }

            if (element == null)
            {
                Logger.WriteError(4, $"При получении замен искомое значение не обнаружено:" +
                $"День: {dayIndex}, Текущая дата — {DateTime.Now.ToShortDateString()}.");

                return ChangesOfDay.DefaultChanges;
            }
            #endregion

            // Для красоты выделим в отдельный блок:
            else
            {
                String path = environment.ContentRootPath;
                path = path[0..path.LastIndexOf(Path.DirectorySeparatorChar)];
                path = Helper.DownloadFileFromURL(Helper.GetDownloadableFileLink(element.LinkToDocument), Path.Combine(path, "Assets", "!Changes"));

                ChangesReader reader = new(path);
                ChangesOfDay toReturn = reader.GetOnlyChanges(dayIndex.GetDayByIndex(), groupName);

                File.Delete(path);
                return toReturn;
            }
        }

        /// <summary>
        /// Метод для получения замен на неделю.
        /// <br/>
        /// <strong>РЕКОМЕНДУЕТСЯ АСИНХРОННОЕ ВЫПОЛНЕНИЕ.</strong>
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Список с объектами, содержащими замены на каждый день.</returns>
        public List<ChangesOfDay> GetWeekChanges(String groupName)
        {
            List<ChangesOfDay> list = new(1);

            for (int i = 0; i < 7; i++)
            {
                list.Add(GetDayChanges(i, groupName));
            }

            return list;
        }
        #endregion
    }
}
