﻿using ScheduleAPI.Controllers.Other.SiteParser;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Controllers.Other.DocumentParser;
using ScheduleAPI.Models.ScheduleElements;
using ScheduleAPI.Models.Cache;
using ScheduleAPI.Models.Cache.CachedTypes;

namespace ScheduleAPI.Models.Getter
{
    /// <summary>
    /// Класс, обертывающий функционал получения замен.
    /// <br />
    /// В связи с переработкой класс был изменен и переделан под статический.
    /// </summary>
    public static class ChangesGetter
    {
        #region Область: Поля.

        /// <summary>
        /// Хранилище кэша, содержащее кэшированные замены для расписаний.
        /// </summary>
        private static readonly CachedVault<ChangesOfDayCache, ChangesOfDay> cachedChanges;
        #endregion

        #region Область: Конструкторы.

        /// <summary>
        /// Статический конструктор класса.
        /// </summary>
        static ChangesGetter()
        {
            cachedChanges = new();
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
        public static ChangesOfDay GetDayChanges(int dayIndex, string groupName)
        {
            ChangeElement element = default;
            List<MonthChanges> changes = default;

            #region Подобласть: Проверка сохраненного кэша.

            var cachedElement = cachedChanges.Get(el => 
            {
                // Чтобы в случае чего даты по умолчанию не совпали, указываем разные значения в конструкторах.
                var basicTargetingDate = DateOnly.FromDateTime(dayIndex.GetDateTimeInWeek().GetValueOrDefault(new DateTime(0)));
                var basicCachedValueDate = DateOnly.FromDateTime(el.CachedElement.ChangesDate.GetValueOrDefault(new DateTime(1)));

                return basicTargetingDate.Equals(basicCachedValueDate) && groupName.Equals(el.CachedElement.GroupName);
            });

            if (cachedElement != null)
            {
                // TODO: Временный логгер, нужный для проверки удаленного кэширования.
                Logger.WriteError(10, "Данные получены из кэша.");

                return cachedElement;
            }
            #endregion

            #region Подобласть: Обрабатываем возможные ошибки.

            try
            {
                changes = new Parser().ParseAvailableNodes();
                element = changes.TryToFindElementByNameOfDayWithoutPreviousWeeks(dayIndex.GetDayByIndex());
            }

            catch (Exception ex)
            {
                var exceptionReturn = new ChangesOfDay
                {
                    ChangesDate = dayIndex.GetDateTimeInWeek(),
                    GroupName = groupName
                };

                Logger.WriteError(3, $"При получении замен произошла ошибка парса страницы: {ex.Message}.");

                cachedChanges.Add(new(exceptionReturn));
                return exceptionReturn;
            }

            if (element == null)
            {
                var exceptionReturn = new ChangesOfDay
                {
                    ChangesDate = dayIndex.GetDateTimeInWeek(),
                    GroupName = groupName
                };

                Logger.WriteError(4, $"При получении замен искомое значение не обнаружено:" +
                $"День: {dayIndex}, Текущая дата — {DateTime.Now.ToShortDateString()}.");

                cachedChanges.Add(new(exceptionReturn));
                return exceptionReturn;
            }
            #endregion

            #region Подобласть: Работа с файлом замен.

            ChangesOfDay toReturn;
            string path = TryToDownloadFileFromGoogleDrive(element);

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    ChangesReader reader = new(path);

                    toReturn = reader.GetOnlyChanges(dayIndex.GetDayByIndex(), groupName);
                    toReturn.ChangesDate = element.Date;
                    toReturn.ChangesFound = true;
                    toReturn.GroupName = groupName;
                }

                catch (WrongDayInDocumentException exception)
                {
                    Logger.WriteError(2, exception.Message);

                    toReturn = new();
                }

                finally
                {
                    Task.Run(() =>
                    {
                        int delAttempts = 0;
                        bool deleted = false;

                        while (!deleted && delAttempts < 5)
                        {
                            try
                            {
                                File.Delete(path);

                                deleted = true;
                            }

                            catch (IOException)
                            {
                                delAttempts++;

                                Thread.Sleep(100);
                            }
                        }

                        if (!deleted)
                        {
                            Logger.WriteError(2, "Удалить файл с заменам не удалось.");
                        }
                    });
                }
            }

            else
            {
                Logger.WriteError(2, "Удалённый сервер хранения документов замен (Google Drive) оказался недоступен.", DateTime.Now);

                toReturn = new();
            }
            #endregion

            cachedChanges.Add(new ChangesOfDayCache(toReturn));
            return toReturn;
        }

        /// <summary>
        /// Метод, пытающийся скачать нужный файл с серверов Google Drive.
        /// <br/>
        /// Так как иногда сервер возвращает ошибку скачивания, это все нужно учитывать.
        /// </summary>
        /// <param name="element">Элемент замен, содержащий ссылку на документ с заменами.</param>
        /// <param name="attempts">Максимальное число попыток скачать документ.</param>
        /// <returns>Путь к скачанному документу.</returns>
        public static string TryToDownloadFileFromGoogleDrive(ChangeElement element, int attempts = 3)
        {
            int currentAttempt = 0;
            string path = string.Empty;

            while (currentAttempt < attempts && string.IsNullOrEmpty(path))
            {
                try
                {
                    path = Helper.DownloadFileFromURL(Helper.GetDownloadableFileLink(element.LinkToDocument));

                    if (string.IsNullOrEmpty(path))
                    {
                        Thread.Sleep(100);
                    }
                }

                catch (ArgumentException e)
                {
                    Logger.WriteError(2, $"Преобразование ссылки прошло неудачно, точная информация: {e.Message}.", DateTime.Now);
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
        public static List<ChangesOfDay> GetWeekChanges(string groupName)
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
