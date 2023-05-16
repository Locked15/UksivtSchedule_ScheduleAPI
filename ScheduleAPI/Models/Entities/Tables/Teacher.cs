using System;
using System.Collections.Generic;

namespace ScheduleAPI.Models.Entities.Tables;

public partial class Teacher
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string? Name { get; set; }

    public string? Patronymic { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
