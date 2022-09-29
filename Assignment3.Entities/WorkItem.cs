using System.Collections.ObjectModel;
using Assignment3.Core;

namespace Assignment3.Entities;

public class WorkItem
{
    public int Id {get; set;}

    [StringLength(100)]
    public string Title {get; set;} 
    
    public string? Description {get; set;}
    
    public virtual List<Tag>? Tags {get; set;}
    public User? user;
    public DateTime Created { get; set; }
    public DateTime StateUpdated { get; set; }

    private State _state;
    public State state
    {
        get => _state;
        set
        {
            _state = value;
            StateUpdated = DateTime.Now;
        }
    }

    public WorkItem(string title){
        Title = title;
        Created=DateTime.Now;
        state = State.New;
    }
}