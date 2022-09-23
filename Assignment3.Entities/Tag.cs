using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class Tag
{
    public int? Id { get; set; }
    [Key]
    [StringLength(50)]
    public string Name { get; set; }
    public virtual ICollection<Task>? Tasks { get; set; }

    public Tag(string name)
    {
        Name = name;
    }
}
