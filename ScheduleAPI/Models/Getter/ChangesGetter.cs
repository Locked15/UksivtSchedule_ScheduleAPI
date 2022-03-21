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
            ChangeElement element = default;
            List<MonthChanges> changes = default;

            #region Подобласть: Обрабатываем возможные ошибки.
            try
            {
                changes = new Parser().ParseAvailableNodes();
                element = changes.TryToFindElementByNameOfDayWithoutPreviousWeeks(dayIndex.GetDayByIndex());
            }

            catch (Exception ex)
            {
                Logger.WriteError(3, $"При получении замен произошла ошибка парса страницы: {ex.Message}.");

                return new();
            }

            if (element == null)
            {
                Logger.WriteError(4, $"При получении замен искомое значение не обнаружено:" +
                $"День: {dayIndex}, Текущая дата — {DateTime.Now.ToShortDateString()}.");

                return new();
            }
            #endregion

            #region Подобласть: Работа с файлом замен.
            ChangesOfDay toReturn;
            String path = TryToDownloadFileFromGoogleDrive(element);

            if (!String.IsNullOrEmpty(path))
            {
                try
                {
                    ChangesReader reader = new(path);

                    toReturn = reader.GetOnlyChanges(dayIndex.GetDayByIndex(), groupName);
                    toReturn.ChangesDate = element.Date;
                    toReturn.ChangesFound = true;
                }

                catch (WrongDayInDocumentException exception)
                {
                    Logger.WriteError(2, exception.Message);

                    toReturn = new();
                }

                finally
                {
                    File.Delete(path);
                }
            }

            else
            {
                Logger.WriteError(2, "Удалённый сервер хранения документов замен (Google Drive) оказался недоступен.", DateTime.Now);

                toReturn = new();
            }

            return toReturn;
            #endregion
        }

        /// <summary>
        /// Метод, пытающийся скачать нужный файл с серверов Google Drive.
        /// <br/>
        /// Так как иногда сервер возвращает ошибку скачивания, это все нужно учитывать.
        /// </summary>
        /// <param name="element">Элемент замен, содержащий ссылку на документ с заменами.</param>
        /// <param name="attempts">Максимальное число попыток скачать документ.</param>
        /// <returns>Путь к скачанному документу.</returns>
        public String TryToDownloadFileFromGoogleDrive(ChangeElement element, Int32 attempts = 3)
        {
            Int32 currentAttempt = 0;
            String path = String.Empty;

            while (currentAttempt < attempts && String.IsNullOrEmpty(path))
            {
                try
                {
                    path = Helper.DownloadFileFromURL(Helper.GetDownloadableFileLink(element.LinkToDocument));
                }

                catch (ArgumentException e)
                {
                    Logger.WriteError(2, $"Преобразование ссылки прошло неудачно, точная информация: {e.Message}.", DateTime.Now);
                }

                catch (Exception e)
                {
                    Logger.WriteError(2, $"При скачивании произошла непредвиденная ошибка: {e.Message}.", DateTime.Now);
                }

                finally
                {
                    currentAttempt++;
                }
            }

            return path;
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
