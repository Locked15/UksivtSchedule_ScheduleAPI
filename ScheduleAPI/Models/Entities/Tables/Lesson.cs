using System;
using System.Collections.Generic;

namespace ScheduleAPI.Models.Entities.Tables;

public partial class Lesson
{
    public int Id { get; set; }

    public int Number { get; set; }

    public string Name { get; set; } = null!;

    public int? TeacherId { get; set; }

    public string? Place { get; set; }

    public bool IsChanged { get; set; }

    public int? ScheduleId { get; set; }

    public int? ReplacementId { get; set; }

    public virtual ScheduleReplacement? Replacement { get; set; }

    public virtual FinalSchedule? Schedule { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
