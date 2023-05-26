using Microsoft.EntityFrameworkCore;
using ScheduleAPI.Controllers.Data.General;
using ScheduleAPI.Models.Entities;
using ScheduleAPI.Models.Entities.Tables;
using ScheduleAPI.Models.Entities.Wrappers;
using ScheduleAPI.Models.Entities.Wrappers.Group;
using System.Runtime.CompilerServices;

namespace ScheduleAPI.Controllers.Data.Getter.Db
{
    public class DbDataGetter
    {
        #region Область: Внутренние Свойства и Константы.

        private const string SelectFromFunctionQueryTemplate = """
            SELECT * 
            FROM {0} 
            (
                {1}
            );
            """;

        private readonly DataContext dataContext;
        #endregion

        #region Область: Инициализаторы.

        public DbDataGetter(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        #endregion

        #region Область: Обработчики Данных.

        #region Подобласть: Поиск Преподавателей.

        public List<Teacher> GetAllTeachers() =>
               dataContext.Teachers.OrderBy(teacher =>
                                            teacher.Id)
                                   .ToList();

        public List<Teacher> SearchTeachers((string? name, string? surname, string? patronymic) searchInfo)
        {
            searchInfo = NormalizeAndConvertTeacherBioData(searchInfo.name, searchInfo.surname, searchInfo.patronymic);
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.UtilityFunctionsInfo.GetTeachersFunctionParameters,
                                                             EscapeFunctionParameter(searchInfo.name),
                                                             EscapeFunctionParameter(searchInfo.surname),
                                                             EscapeFunctionParameter(searchInfo.patronymic));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.UtilityFunctionsInfo.GetTeachersFunctionName,
                                                        parameters);

            var data = dataContext.Teachers.FromSqlRaw(query.ToString());
            return data.ToList();
        }
        #endregion

        #region Подобласть: Работа с Группами.

        public GroupBasicScheduleWrapper GetBasicScheduleForGroup(string targetGroup, DateOnly targetDate)
        {
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetGroupFunctionsInfo.GetGroupBasicScheduleFunctionParameters,
                                                             EscapeFunctionParameter(targetGroup),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetGroupFunctionsInfo.GetGroupBasicScheduleFunctionName,
                                                        parameters);

            var requestData = GetTargetDataForBasicScheduleRequest(targetDate);
            var basicScheduleData = dataContext.BasicSchedules.FirstOrDefault(schedule =>
                                                                              schedule.CycleId == requestData.Item2 &&
                                                                              schedule.DayIndex == requestData.Item1 &&
                                                                              EF.Functions.ILike(schedule.TargetGroup, targetGroup));
            var lessonsData = dataContext.UtilityLessonGroups.FromSqlRaw(query.ToString()).ToList();

            return new GroupBasicScheduleWrapper(requestData.Item1, targetDate, basicScheduleData, lessonsData);
        }

        public GroupReplacementsWrapper GetReplacementsForGroup(string targetGroup, DateOnly targetDate)
        {
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetGroupFunctionsInfo.GetGroupReplacementsFunctionParameters,
                                                             EscapeFunctionParameter(targetGroup),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetGroupFunctionsInfo.GetGroupReplacementsFunctionName,
                                                        parameters);

            var isTargetDateDataAvailable = dataContext.ScheduleReplacements.Max(replacement =>
                                                                                 replacement.ReplacementDate) >= targetDate;
            var replacementData = dataContext.ScheduleReplacements.FirstOrDefault(replacement =>
                                                                                  replacement.ReplacementDate == targetDate &&
                                                                                  EF.Functions.ILike(replacement.TargetGroup, targetGroup));
            var lessonsData = dataContext.UtilityLessonGroups.FromSqlRaw(query.ToString()).ToList();

            return new GroupReplacementsWrapper(targetDate, isTargetDateDataAvailable, replacementData, lessonsData);
        }

        public GroupFinalScheduleWrapper GetFinalScheduleForGroup(string targetGroup, DateOnly targetDate)
        {
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetGroupFunctionsInfo.GetGroupFinalScheduleFunctionParameters,
                                                             EscapeFunctionParameter(targetGroup),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetGroupFunctionsInfo.GetGroupFinalScheduleFunctionName,
                                                        parameters);

            var isTargetDateDataAvailable = dataContext.FinalSchedules.Max(schedule =>
                                                                           schedule.ScheduleDate) >= targetDate;
            var finalScheduleData = dataContext.FinalSchedules.FirstOrDefault(finalSchedule =>
                                                                              finalSchedule.ScheduleDate == targetDate &&
                                                                              EF.Functions.ILike(finalSchedule.TargetGroup, targetGroup));
            var lessonsData = dataContext.UtilityLessonGroups.FromSqlRaw(query.ToString()).ToList();

            return new GroupFinalScheduleWrapper(targetDate, isTargetDateDataAvailable, finalScheduleData, lessonsData);
        }
        #endregion

        #region Подобласть: Работа с Преподавателями.

        public TeacherScheduleDataWrapper GetBasicScheduleForTeacher(int teacherId, DateOnly targetDate)
        {
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherBasicScheduleFunctionParametersById,
                                                             EscapeFunctionParameter(teacherId),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherBasicScheduleFunctionName,
                                                        parameters);

            var isTargetDataAvailable = dataContext.UtilityAtomicDates.First()
                                                                      .ToDateOnly() >= targetDate;
            var lessonsData = dataContext.UtilityLessonTeachers.FromSqlRaw(query.ToString()).ToList();

            return new TeacherScheduleDataWrapper(targetDate.DayOfWeek.GetIndexFromDayOfWeek(), targetDate,
                                                  isTargetDataAvailable, lessonsData);
        }

        public TeacherScheduleDataWrapper GetBasicScheduleForTeacher(string? name, string? surname, string? patronymic, DateOnly targetDate)
        {
            var normalizedTargetTeacherBio = NormalizeAndConvertTeacherBioData(name, surname, patronymic);
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherBasicScheduleFunctionParametersByBio,
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item1),
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item2),
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item3),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherBasicScheduleFunctionName,
                                                        parameters);

            var isTargetDataAvailable = dataContext.UtilityAtomicDates.First()
                                                                      .ToDateOnly() >= targetDate;
            var lessonsData = dataContext.UtilityLessonTeachers.FromSqlRaw(query.ToString()).ToList();

            return new TeacherScheduleDataWrapper(targetDate.DayOfWeek.GetIndexFromDayOfWeek(), targetDate,
                                                  isTargetDataAvailable, lessonsData);
        }

        public TeacherScheduleDataWrapper GetReplacementsForTeacher(int teacherId, DateOnly targetDate)
        {
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherReplacementsFunctionParametersById,
                                                             EscapeFunctionParameter(teacherId),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherReplacementsFunctionName,
                                                        parameters);

            var isTargetDataAvailable = dataContext.FinalSchedules.Max(schedule =>
                                                                       schedule.ScheduleDate) >= targetDate;
            var lessonsData = dataContext.UtilityLessonTeachers.FromSqlRaw(query.ToString()).ToList();

            return new TeacherScheduleDataWrapper(targetDate.DayOfWeek.GetIndexFromDayOfWeek(), targetDate,
                                                  isTargetDataAvailable, lessonsData);
        }

        public TeacherScheduleDataWrapper GetReplacementsForTeacher(string? name, string? surname, string? patronymic, DateOnly targetDate)
        {
            var normalizedTargetTeacherBio = NormalizeAndConvertTeacherBioData(name, surname, patronymic);
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherReplacementsFunctionParametersByBio,
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item1),
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item2),
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item3),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherReplacementsFunctionName,
                                                        parameters);

            var isTargetDataAvailable = dataContext.ScheduleReplacements.Max(replacement =>
                                                                             replacement.ReplacementDate) >= targetDate;
            var lessonsData = dataContext.UtilityLessonTeachers.FromSqlRaw(query.ToString()).ToList();

            return new TeacherScheduleDataWrapper(targetDate.DayOfWeek.GetIndexFromDayOfWeek(), targetDate,
                                                  isTargetDataAvailable, lessonsData);
        }

        public TeacherScheduleDataWrapper GetFinalScheduleForTeacher(int teacherId, DateOnly targetDate)
        {
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherFinalScheduleFunctionParametersById,
                                                             EscapeFunctionParameter(teacherId),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherFinalScheduleFunctionName,
                                                        parameters);

            var isTargetDataAvailable = dataContext.FinalSchedules.Max(schedule =>
                                                                       schedule.ScheduleDate) >= targetDate;
            var lessonsData = dataContext.UtilityLessonTeachers.FromSqlRaw(query.ToString()).ToList();

            return new TeacherScheduleDataWrapper(targetDate.DayOfWeek.GetIndexFromDayOfWeek(), targetDate,
                                                  isTargetDataAvailable, lessonsData);
        }

        public TeacherScheduleDataWrapper GetFinalScheduleForTeacher(string? name, string? surname, string? patronymic, DateOnly targetDate)
        {
            var normalizedTargetTeacherBio = NormalizeAndConvertTeacherBioData(name, surname, patronymic);
            var parameters = FormattableStringFactory.Create(StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherFinalScheduleFunctionParametersByBio,
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item1),
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item2),
                                                             EscapeFunctionParameter(normalizedTargetTeacherBio.Item3),
                                                             EscapeFunctionParameter(targetDate));
            var query = FormattableStringFactory.Create(SelectFromFunctionQueryTemplate,
                                                        StoredFunctionsInfo.TargetTeacherFunctionsInfo.GetTeacherFinalScheduleFunctionName,
                                                        parameters);

            var isTargetDataAvailable = dataContext.FinalSchedules.Max(schedule =>
                                                                       schedule.ScheduleDate) >= targetDate;
            var lessonsData = dataContext.UtilityLessonTeachers.FromSqlRaw(query.ToString()).ToList();

            return new TeacherScheduleDataWrapper(targetDate.DayOfWeek.GetIndexFromDayOfWeek(), targetDate,
                                                  isTargetDataAvailable, lessonsData);
        }
        #endregion
        #endregion

        #region Область: Внутренние Функции.

        private (int, int) GetTargetDataForBasicScheduleRequest(DateOnly targetDate)
        {
            (int dayIndex, int cycleId) result;
            result.dayIndex = targetDate.DayOfWeek.GetIndexFromDayOfWeek();

            var targetSemester = 3M - Math.Ceiling(targetDate.Month / 6M);
            var dbCycleId = dataContext.TargetCycles.FirstOrDefault(cycle =>
                                                                     cycle.Year == targetDate.Year &&
                                                                     cycle.Semester == targetSemester)?.Id;
            result.cycleId = dbCycleId ?? -1;

            return result;
        }

        private static (string?, string?, string?) NormalizeAndConvertTeacherBioData(string? name, string? surname, string? patronymic)
        {
            var targetName = name?.Trim();
            var targetSurname = surname?.Trim();
            var targetPatronymic = patronymic?.Trim();

            /* Потому что в БД нет данных о полных именах и фамилиях (в документах они представлены в коротком виде). 
               Если никакие данные не отправлены, будет возвращён 'NULL'. В ином случае будет получен первый символ имени и отчества. */
            targetName = targetName?.FirstOrDefault().ToString();
            targetPatronymic = targetPatronymic?.FirstOrDefault().ToString();

            // В то же время информация о фамилиях присутствует в полном объёме, поэтому извлекать символ из фамилии не нужно.
            return (targetName, targetSurname, targetPatronymic);
        }

        private static string EscapeFunctionParameter(int? id) =>
                EscapeFunctionParameter(id?.ToString());

        private static string EscapeFunctionParameter(DateOnly? targetDate) =>
                EscapeFunctionParameter(targetDate?.ToString("yyyy.MM.dd"));

        private static string EscapeFunctionParameter(string? parameter) =>
                string.IsNullOrWhiteSpace(parameter) ? "NULL" : $"'{parameter}'";
        #endregion
    }
}
