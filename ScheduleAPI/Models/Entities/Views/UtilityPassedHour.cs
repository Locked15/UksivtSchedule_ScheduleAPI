using System;
using System.Collections.Generic;

namespace ScheduleAPI.Models.Entities.Views;

public partial class UtilityPassedHour
{
    public string? TargetCycle { get; set; }

    public string? TargetGroup { get; set; }

    public string? LessonName { get; set; }

    public int? HoursPassed { get; set; }
}
