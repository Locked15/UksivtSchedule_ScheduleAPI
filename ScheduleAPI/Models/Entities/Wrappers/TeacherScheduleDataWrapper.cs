using ScheduleAPI.Models.Entities.Views;

namespace ScheduleAPI.Models.Entities.Wrappers
{
    public class TeacherScheduleDataWrapper
    {
        public int DayIndex { get; set; }

        public DateOnly TargetDate { get; set; }

        public bool ReplacementsForDateIsAvailable { get; set; }

        public List<UtilityLessonTeacher> LessonsInfo { get; set; }

        public TeacherScheduleDataWrapper(int dayIndex, DateOnly targetDate, bool replacementsForDateIsAvailable, List<UtilityLessonTeacher> lessonsInfo)
        {
            DayIndex = dayIndex;
            TargetDate = targetDate;
            ReplacementsForDateIsAvailable = replacementsForDateIsAvailable;
            LessonsInfo = lessonsInfo;
        }
    }
}
