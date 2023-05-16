using System;
using System.Collections.Generic;

namespace ScheduleAPI.Models.Entities.Tables;

public partial class FinalSchedule
{
    public int Id { get; set; }

    public int CommitHash { get; set; }

    public string TargetGroup { get; set; } = null!;

    public DateOnly ScheduleDate { get; set; }

    public int CycleId { get; set; }

    public virtual TargetCycle Cycle { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
