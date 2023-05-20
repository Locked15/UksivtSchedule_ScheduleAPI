using System;
using System.Collections.Generic;

namespace ScheduleAPI.Models.Entities.Views;

public partial class UtilityLessonGroup
{
    public int? LessonNumber { get; set; }

    public string? LessonName { get; set; }

    public string? LessonTeacher { get; set; }

    public string? LessonPlace { get; set; }

    public bool? LessonIsChanged { get; set; }

    public int? LessonHoursPassed { get; set; }
}
