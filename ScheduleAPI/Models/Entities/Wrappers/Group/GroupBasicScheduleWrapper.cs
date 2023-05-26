using ScheduleAPI.Models.Entities.Tables;
using ScheduleAPI.Models.Entities.Views;

namespace ScheduleAPI.Models.Entities.Wrappers.Group
{
    public class GroupBasicScheduleWrapper
    {
        public int DayIndex { get; set; }

        public DateOnly TargetDate { get; set; }

        public BasicSchedule? BasicSchedule { get; set; }

        public List<UtilityLessonGroup> LessonsInfo { get; set; }

        public GroupBasicScheduleWrapper(int dayIndex, DateOnly targetDate, BasicSchedule? basicSchedule, List<UtilityLessonGroup> lessonsInfo)
        {
            DayIndex = dayIndex;
            TargetDate = targetDate;
            BasicSchedule = basicSchedule;
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
