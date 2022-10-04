namespace Assignment3.Core;

public class WorkItemRepository:IWorkItemRepository
{
    private KanbanContext _context;
    public WorkItemRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int WorkItemId) Create(WorkItemCreateDTO workItem)
    {
        var entity = _context.WorkItems.FirstOrDefault(c => c.Title == workItem.Title);
        Response response;
        
        if (entity != null)
        {
            response = Response.Conflict;
        }
        else if (workItem.AssignedToId != null &&
                 _context.Users.FirstOrDefault(c => c.Id == workItem.AssignedToId) is null)
        {
            return (Response.BadRequest, -1);
        }
        else
        {
            entity = new WorkItem(workItem.Title);
            entity.user = _context.Users.FirstOrDefault(c => c.Id == workItem.AssignedToId); 
            entity.Description = workItem.Description;
            if (workItem.Tags != null)
            {
                entity.Tags = (from c in _context.Tags
                    where workItem.Tags.Contains(c.Name)
                    select c).ToList();
            }
           
            
            _context.WorkItems.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        
        return (response, entity.Id);
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAll()
    {
        var workItems = from c in _context.WorkItems
            orderby c.Title
            select new WorkItemDTO(c.Id, c.Title, c.user.Name, (IReadOnlyCollection<string>)c.Tags.Select(x=>x.Name), c.state);

        return workItems.ToArray();
    }
    
    public IReadOnlyCollection<WorkItemDTO> ReadAllRemoved()
    {
        var workItems = from c in _context.WorkItems
            orderby c.Title
            where c.state==State.Removed
            select new WorkItemDTO(c.Id, c.Title, c.user.Name, (IReadOnlyCollection<string>)c.Tags.Select(x=>x.Name), c.state);

        return workItems.ToArray();
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByTag(string tag)
    {
        var workItems = from c in _context.WorkItems
            where ((IReadOnlyCollection<string>)c.Tags.Select(x=>x.Name)).Contains(tag)
            select new WorkItemDTO(c.Id, c.Title, c.user.Name, (IReadOnlyCollection<string>)c.Tags.Select(x=>x.Name), c.state);

        return workItems.ToArray();
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByUser(int userId)
    {
        var workItemsFiltered = new List<WorkItem>();
        foreach (var it in _context.WorkItems)
        {
            if (it.user?.Id==userId) workItemsFiltered.Add(it);
        }
        var workItems = workItemsFiltered.Select(c=>new WorkItemDTO(c.Id, c.Title, c.user.Name, c.Tags.Select(x=>x.Name).ToList(), c.state));

        
        return workItems.ToArray();
        
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByState(State state)
    {
        var workItems = from c in _context.WorkItems
            where c.state==state
            select new WorkItemDTO(c.Id, c.Title, c.user.Name, (IReadOnlyCollection<string>)c.Tags.Select(x=>x.Name), c.state);

        return workItems.ToArray();
    }
    public WorkItemDetailsDTO Read(int workItemId)
    {
        var workItems = (from c in _context.WorkItems
            where c.Id==workItemId
            select new WorkItemDetailsDTO(c.Id, c.Title, c.Description,c.Created, c.user.Name, (IReadOnlyCollection<string>)c.Tags.Select(x=>x.Name), c.state, c.StateUpdated)).FirstOrDefault();

        return workItems;
    }
    public Response Update(WorkItemUpdateDTO workItem)
    {
        var entity = _context.WorkItems.Find(workItem.Id);
        Response response;
        
        if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.WorkItems.FirstOrDefault(c => c.Id != workItem.Id && c.Title == workItem.Title) != null)
        {
            response = Response.Conflict;
        }
        else if (workItem.AssignedToId !=null && _context.Users.FirstOrDefault(c => c.Id == workItem.AssignedToId) is null)
        {
            response = Response.BadRequest;
        }
        else
        {
         
            entity.Title = workItem.Title;
            entity.Description = workItem.Description;
            entity.Tags = (from c in _context.Tags
                where workItem.Tags.Contains(c.Name)
                select c).ToList();
            entity.user = _context.Users.FirstOrDefault(c => c.Id == workItem.AssignedToId);
            entity.state = workItem.State;
            entity.StateUpdated=DateTime.Now;
            response = Response.Updated;
        }
        return response;
    }
    public Response Delete(int workItemId)
    {
        var workItem = _context.WorkItems.FirstOrDefault(c => c.Id == workItemId);
        Response response;

        if (workItem is null)
        {
            response = Response.NotFound;
        }
        else if (workItem.state == State.Active)
        {
            workItem.state = State.Removed;
            _context.SaveChanges();
            response = Response.Deleted;
        } else if (workItem.state == State.Closed || workItem.state == State.Resolved ||
                   workItem.state == State.Removed)
        {
            response = Response.Conflict;
        }
        else
        {
            _context.WorkItems.Remove(workItem);
            _context.SaveChanges();

            response = Response.Deleted;
        }

        return response;
    }
}
