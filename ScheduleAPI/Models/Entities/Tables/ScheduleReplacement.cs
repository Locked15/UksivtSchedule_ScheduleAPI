using System.Text.Json.Serialization;

namespace ScheduleAPI.Models.Entities.Tables;

public partial class ScheduleReplacement
{
    public int Id { get; set; }

    /// <summary>
    /// Это свойство содержит данные реализации.
    /// </summary>
    [JsonIgnore]
    public int CommitHash { get; set; }

    public bool? IsAbsolute { get; set; }

    public string TargetGroup { get; set; } = null!;

    [JsonIgnore]
    public DateOnly ReplacementDate { get; set; }

    /// <summary>
    /// Это значение не имеет смысла возвращать.
    /// Для пользователя целевой цикл ясен из целевой даты.
    /// </summary>
    [JsonIgnore]
    public int CycleId { get; set; }

    /// <summary>
    /// Сериализация приводит к ошибке зацикливания.
    /// </summary>
    [JsonIgnore]
    public virtual TargetCycle Cycle { get; set; } = null!;

    /// <summary>
    /// Сериализация этого значения приводит к ошибке зацикливания.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
