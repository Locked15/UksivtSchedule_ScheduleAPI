using System.Data.SqlClient;
using ScheduleAPI.Controllers.Other.General;
using ScheduleAPI.Models.ScheduleElements;

namespace ScheduleAPI.Models.Getter
{
    /// <summary>
    /// Класс, нужный для получения данных посредством заложенных в приложение ассетов.
    /// <br/>
    /// </summary>
    public static class DbGetter
    {
        #region Область: Поля.

        /// <summary>
        /// Поле, содержащее запрос на получение расписания для группы по указаному дню.
        /// <i>
        /// <br/> <br/>
        /// Содержит 2 вводимых значения: <br/>
        /// 1. Название группы (String); <br/>
        /// 2. Индекс дня (Int32).
        /// <br/> <br/>
        /// Возвращает 6 значений: <br/>
        /// 1. Название пары; <br/>
        /// 2. Подгруппа; <br/>
        /// 3. Пара четной/нечетной недели; <br/>
        /// 4. Номер пары (НЕБЕЗОПАСНО); <br/>
        /// 5. Преподаватель (НЕБЕЗОПАСНО, Частично); <br/>
        /// 6. Место проведения пары (НЕБЕЗОПАСНО, Частично).
        /// </i>
        /// </summary>
        private static readonly string mainScheduleQuery;

        /// <summary>
        /// Поле, содержащее объект-расписание, возвращаемое по умолчанию при возникновении ошибок.
        /// </summary>
        private static readonly DaySchedule defaultDaySchedule;
        #endregion

        #region Область: Конструкторы класса.

        /// <summary>
        /// Статический конструктор класса.
        /// </summary>
        static DbGetter()
        {
            mainScheduleQuery = "GetScheduleData @GroupName, @DayInd;";

            defaultDaySchedule = new("Понедельник", Enumerable.Empty<Lesson>().ToList());
        }
        #endregion

        #region Область: Методы.

        /// <summary>
        /// Метод для получения расписания на указанный день.
        /// </summary>
        /// <param name="dayIndex">Индекс нужного дня.</param>
        /// <param name="groupName">Название нужной группы.</param>
        /// <param name="selectUnsecure">Выбирать значения из "небезопасных" столбцов?</param>
        public static DaySchedule GetDaySchedule(int dayIndex, string groupName, bool selectUnsecure = false)
        {
            #region Подобласть: Переменные для работы.

            groupName = groupName.ToUpper().RemoveStringChars();
            SqlConnection connect = DataBaseConnector.Connection;

            SqlCommand command = new(mainScheduleQuery, connect);
            command.Parameters.AddWithValue("@GroupName", groupName);
            command.Parameters.AddWithValue("@DayInd", dayIndex);
            #endregion

            #region Подобласть: Проверка на воскресенье.

            // В БД нет данных о воскресенье, поэтому эти данные надо внести вручную.
            if (dayIndex == 6)
            {
                List<Lesson> lessons = new();

                for (int i = 0; i < dayIndex; i++)
                {
                    lessons.Add(new(i));
                }

                return new(dayIndex.GetDayByIndex(), lessons);
            }
            #endregion

            connect.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int i = 0;
                    List<Lesson> lessons = new(1);

                    while (reader.Read())
                    {
                        // Безопасные значения:
                        string lessonName = reader.GetString(0);
                        int? subGroup = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                        int? subWeek = reader.IsDBNull(2) ? null : reader.GetInt32(2);

                        // Небезопасные значения:
                        int lessonNumber = selectUnsecure ? reader.GetInt32(3) : i;
                        string teacher = selectUnsecure ? (reader.IsDBNull(4) ? "[Нет Данных]" : reader.GetString(4)) : "[Нет Данных]";
                        string place = selectUnsecure ? (reader.IsDBNull(5) ? "[Нет Данных]" : reader.GetString(5)) : "[Нет Данных]";

                        lessonName = CheckToSubGroup(subGroup, lessonName);
                        lessonName = CheckToSubWeek(subWeek, lessonName);

                        lessons.Add(new Lesson(lessonNumber, lessonName, teacher, place));
                        ++i;
                    }

                    connect.Close();
                    return new DaySchedule(dayIndex.GetDayByIndex(), lessons);
                }
            }

            connect.Close();

            Logger.WriteError(2, "Данные в БД (День) не обнаружены: " +
            $"Группа: {groupName}, День: {dayIndex}.");
            return defaultDaySchedule;
        }

        /// <summary>
        /// Метод для получения расписания на неделю для указанной группы.
        /// </summary>
        /// <param name="groupName">Название группы.</param>
        /// <returns>Расписание на неделю для указанной группы.</returns>
        public static WeekSchedule GetWeekSchedule(string groupName, bool selectUnsecure = false)
        {
            groupName = groupName.ToUpper();
            List<DaySchedule> days = new(1);

            for (int i = 0; i < 7; i++)
            {
                days.Add(GetDaySchedule(i, groupName, selectUnsecure));
            }

            return new(groupName, days);
        }

        /// <summary>
        /// Метод для изменения названия пары в зависимости от того, пара для подгруппы или нет.
        /// </summary>
        /// <param name="subGroup">Значение, определяющее для подгруппы или нет.</param>
        /// <param name="lessonName">Оригинальное название пары.</param>
        /// <returns>Итоговое название пары.</returns>
        private static string CheckToSubGroup(int? subGroup, string lessonName)
        {
            if (subGroup.HasValue)
            {
                lessonName += $" ({subGroup.Value} подгруппа)";
            }

            return lessonName;
        }

        /// <summary>
        /// Метод для изменения названия пары в зависимости от того, пара для четной/нечетной недели или нет.
        /// </summary>
        /// <param name="subGroup">Значение, определяющее для какой недели пара.</param>
        /// <param name="lessonName">Оригинальное название пары.</param>
        /// <returns>Итоговое название пары.</returns>
        private static string CheckToSubWeek(int? subWeek, string lessonName)
        {
            if (subWeek.HasValue)
            {
                string subValue = subWeek.Value == 1 ? "Нечетная" : "Четная";
                lessonName += $" ({subValue} неделя)";
            }

            return lessonName;
        }
        #endregion
    }
}
