using ScheduleAPI.Models.Entities.Tables;
using ScheduleAPI.Models.Entities.Views;

namespace ScheduleAPI.Models.Entities.Wrappers.Group
{
    public class GroupFinalScheduleWrapper
    {
        public DateOnly TargetDate { get; set; }

        public bool FinalSchedulesForDateIsAvailable { get; set; }

        public FinalSchedule? FinalSchedule { get; set; }

        public List<UtilityLessonGroup> LessonsInfo { get; set; }

        public GroupFinalScheduleWrapper(DateOnly targetDate, bool finalSchedulesForDateIsAvailable, FinalSchedule? finalSchedule, List<UtilityLessonGroup> lessonsInfo)
        {
            TargetDate = targetDate;
            FinalSchedulesForDateIsAvailable = finalSchedulesForDateIsAvailable;
            FinalSchedule = finalSchedule;
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
