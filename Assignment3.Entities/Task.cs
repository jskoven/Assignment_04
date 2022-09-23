namespace Assignment3.Entities;

public class Task
{
    int Id {get; set;}

    [StringLength(100)]
    string Title {get;} 
    User? AssignedTo {get; set;}
    string? Description {get; set;}
    public State state {get; set;}
    Tag[]? tags;

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
