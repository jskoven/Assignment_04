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
        
        if (entity is null)
        {
            entity = new WorkItem(workItem.Title);

            _context.WorkItems.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }
        return (response, entity.Id);
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAll()
    {
        var workItems = from WorkItemDTO c in _context.WorkItems
            orderby c.Title
            select new WorkItemDTO(c.Id, c.Title, c.AssignedToName, c.Tags, c.State);

        return workItems.ToArray();
    }
    
    public IReadOnlyCollection<WorkItemDTO> ReadAllRemoved()
    {
        var workItems = from WorkItemDTO c in _context.WorkItems
            orderby c.Title
            where c.State==State.Removed
            select new WorkItemDTO(c.Id, c.Title, c.AssignedToName, c.Tags, c.State);

        return workItems.ToArray();
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByTag(string tag)
    {
        var workItems = from WorkItemDTO c in _context.WorkItems
            where c.Tags.Contains(tag)
            select new WorkItemDTO(c.Id, c.Title, c.AssignedToName, c.Tags, c.State);

        return workItems.ToArray();
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByUser(int userId)
    {
        var workItems = from WorkItemDTO c in _context.WorkItems
            join UserDTO u in _context.Users on c.AssignedToName equals u.Name
            where u.Id == userId
            select new WorkItemDTO(c.Id, c.Title, c.AssignedToName, c.Tags, c.State);

        return workItems.ToArray();
        
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByState(State state)
    {
        var workItems = from WorkItemDTO c in _context.WorkItems
            where c.State==state
            select new WorkItemDTO(c.Id, c.Title, c.AssignedToName, c.Tags, c.State);

        return workItems.ToArray();
    }
    public WorkItemDetailsDTO Read(int workItemId)
    {
        var workItems = (from WorkItemDetailsDTO c in _context.WorkItems
            where c.Id==workItemId
            select new WorkItemDetailsDTO(c.Id, c.Title,c.Description , c.Created,c.AssignedToName, c.Tags, c.State,c.StateUpdated)).First();

        return workItems;
    }
    public Response Update(WorkItemUpdateDTO workItem)
    {
        var entity = _context.WorkItems.Find(workItem.Title);
        Response response;
        
        if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.WorkItems.FirstOrDefault(c => c.Id != workItem.Id && c.Title == workItem.Title) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            return Response.Conflict;
        }
        
        return response;
    }
    public Response Delete(int taskId)
    {
        return Response.Conflict;
    }
}
