using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class User
{
    public int? Id { get; set; }
    [StringLength(100)]
    public string Name { get; set; }
    [StringLength(100)]
    [Key]
    public string Email { get; set; }
    public virtual List<WorkItem> WorkItems { get; set; }

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
