namespace Assignment3.Entities;

public class Task
{
    public int Id {get; set;}

    [StringLength(100)]
    public string Title {get;} 
    
    public string? Description {get; set;}
    public State state {get; set;}
    public virtual List<Tag>? Tags {get; set;}
    public User? user;

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
