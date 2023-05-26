using System;
using System.Collections.Generic;

namespace ScheduleAPI.Models.Entities.Views;

public partial class UtilityAtomicDate
{
    public int DayOfWeek { get; set; }

    public int DayOfMonth { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public DateOnly ToDateOnly() => new(Year, Month, DayOfMonth);
}
