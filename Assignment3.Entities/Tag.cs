using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class Tag
{
    public int? Id { get; set; }
    [Key] [StringLength(50)] public string Name { get; set; } = null!;
    public virtual List<WorkItem> Tasks { get; set; }

    public Tag(string name)
    {
        Name = name;
    }
}
