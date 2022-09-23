using System.ComponentModel.DataAnnotations;

namespace Assignment3.Entities;

public class User
{
    private int? Id { get; set; }
    [StringLength(100)]
    private string Name { get; set; }
    [StringLength(100)]
    [Key]
    private string Email { get; set; }
    private List<Task>? Tasks { get; set; }

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
