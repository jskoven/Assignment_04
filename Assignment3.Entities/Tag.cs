using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class Tag
{
    private int? Id { get; set; }
    [Key]
    [StringLength(50)]
    private string Name { get; set; }
    private Task[]? Tasks { get; set; }

    public Tag(string name)
    {
        Name = name;
    }
}
