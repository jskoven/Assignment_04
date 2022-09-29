using Assignment3.Core;

namespace Assignment3.Entities;

public class WorkItem
{
    public int Id {get; set;}

    [StringLength(100)]
    public string Title {get; } 
    
    public string? Description {get; set;}
    public State state {get; set;}
    public virtual List<Tag>? Tags {get; set;}
    public User? user;

    public WorkItem(string title){
        this.Title = title;
    }

   

}
