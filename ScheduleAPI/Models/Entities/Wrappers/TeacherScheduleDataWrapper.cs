using ScheduleAPI.Models.Entities.Views;

namespace ScheduleAPI.Models.Entities.Wrappers
{
    public class TeacherScheduleDataWrapper
    {
        public DateOnly TargetDate { get; set; }

        public bool ReplacementsForDateIsAvailable { get; set; }

        public List<UtilityLessonTeacher> LessonsInfo { get; set; }

        public TeacherScheduleDataWrapper(DateOnly targetDate, bool replacementsForDateIsAvailable, List<UtilityLessonTeacher> lessonsInfo)
        {
            TargetDate = targetDate;
            ReplacementsForDateIsAvailable = replacementsForDateIsAvailable;
            LessonsInfo = lessonsInfo;
        }
    }
}
