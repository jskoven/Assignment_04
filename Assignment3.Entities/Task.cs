namespace Assignment3.Entities;

public class Task
{
    public int Id {get; set;}

    [StringLength(100)]
    public string Title {get;} 
    public User? AssignedTo {get; set;}
    public string? Description {get; set;}
    public State state {get; set;}
    public virtual ICollection<Tag>? Tags {get; set;}

    public Task(string title, State state){
        this.Title = title;
        this.state = state;
    }

    public enum State{
        New,
        Active,
        Resolved,
        Closed,
        Removed

    }

}
