using System.Text.Json.Serialization;

namespace ScheduleAPI.Models.Entities.Tables;

public partial class Teacher
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string? Name { get; set; }

    public string? Patronymic { get; set; }

    /// <summary>
    /// Сериализация этого свойства приводит к зацикливанию.
    /// Так что оно указано как игнорируемое при работе с JSON.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
