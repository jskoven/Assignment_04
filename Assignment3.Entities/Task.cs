namespace Assignment3.Entities;

public class Task
{
    int Id;
    [StringLength(100)]
    string Title; 
    User AssignedTo;
    string Description;
    State state;
    Tag[] tags;

    enum State{
        New,
        Active,
        Resolved,
        Closed,
        Removed

    }

}
