using ScheduleAPI.Models.Entities.Tables;
using ScheduleAPI.Models.Entities.Views;

namespace ScheduleAPI.Models.Entities.Wrappers.Group
{
    public class GroupReplacementsWrapper
    {
        public DateOnly TargetDate { get; set; }

        public bool ReplacementsForDateIsAvailable { get; set; }

        public ScheduleReplacement? ScheduleReplacement { get; set; }

        public List<UtilityLessonGroup> LessonsInfo { get; set; }

        public GroupReplacementsWrapper(DateOnly targetDate, bool replacementsForDateIsAvailable, ScheduleReplacement? scheduleReplacement, List<UtilityLessonGroup> lessonsInfo)
        {
            TargetDate = targetDate;
            ReplacementsForDateIsAvailable = replacementsForDateIsAvailable;
            ScheduleReplacement = scheduleReplacement;
            LessonsInfo = lessonsInfo;

            ClearLessonsTeacherEmptyProperty();
        }

        private void ClearLessonsTeacherEmptyProperty()
        {
            foreach (var lesson in LessonsInfo)
            {
                if (string.IsNullOrWhiteSpace(lesson.LessonTeacher))
                    lesson.LessonTeacher = null;
            }
        }
    }
}
