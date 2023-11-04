namespace ScheduleAPI.Controllers.API
{
    public interface IScheduleController
    {
        #region Область: Параметры выборки расписания.

        protected const int DefaultDayIndex = 0;

        protected const int DefaultTeacherIndex = 0;

        protected const string DefaultGroupName = "20П-3";
        #endregion

        #region Область: Параметры поиска.

        protected const string DefaultBranch = "Programming";

        protected const string DefaultAffiliation = "П";

        protected const string DefaultTeacherSearchRequest = "Карим";
        #endregion
    }
}
