namespace ScheduleAPI.Models.Entities.Tables;

public partial class TargetCycle
{
    public int Id { get; set; }

    public int Year { get; set; }

    public int Semester { get; set; }

    public virtual ICollection<BasicSchedule> BasicSchedules { get; set; } = new List<BasicSchedule>();

    public virtual ICollection<FinalSchedule> FinalSchedules { get; set; } = new List<FinalSchedule>();

    public virtual ICollection<ScheduleReplacement> ScheduleReplacements { get; set; } = new List<ScheduleReplacement>();
}
