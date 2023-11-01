namespace ScheduleAPI.Models.Entities.Views;

public partial class UtilityLessonTeacher
{
    public int? LessonNumber { get; set; }

    public string? LessonName { get; set; }

    public string? LessonPlace { get; set; }

    public string? LessonGroup { get; set; }

    public bool? LessonIsChanged { get; set; }

    public int? LessonHoursPassed { get; set; }
}
