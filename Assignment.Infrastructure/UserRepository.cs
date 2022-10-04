using Microsoft.EntityFrameworkCore;

namespace Assignment3.Core;

public class UserRepository : IUserRepository
{
    private readonly KanbanContext _context;
    public UserRepository(KanbanContext context)
    {
        _context = context;
    }
    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = _context.Users.FirstOrDefault(c => c.Name == user.Name);
        Response response;

        if (entity is null)
        {
            entity = new User();
            entity.Name = user.Name;
            entity.Email = user.Email;
            _context.Users.Add(entity);
            _context.SaveChanges();
            response = Response.Created;
        }else
        {
            response = Response.Conflict;
        }
        
        var created = new UserDTO(entity.Id ,entity.Name,entity.Email);
        return (response, created.Id);
    }

    public IReadOnlyCollection<UserDTO> ReadAll()
    {
        var users = from t in _context.Users
            orderby t.Name
            select new UserDTO(t.Id, t.Name, t.Email);

        return users.ToArray();
    }

    public UserDTO Read(int userId)
    {
        var users = from t in _context.Users
            where t.Id == userId
            select new UserDTO(t.Id, t.Name, t.Email);
        return users.FirstOrDefault()!;
    }

    public Response Update(UserUpdateDTO user)
    {
        var entity = _context.Users.Find(user.Id);
        Response response;
        if (entity is null)
        {
            response = Response.NotFound;
        }
        
        //TODO: Ask about this else if statement
        else if (_context.Users.FirstOrDefault(c => c.Id != user.Id && c.Email == user.Email) != null)
        {
            response = Response.Conflict;
        }
        
        else
        {
            entity.Name = user.Name;
            entity.Email = user.Email;
            _context.SaveChanges();
            response = Response.Updated;
        }

        return response;
    }

    public Response Delete(int userId, bool force = false)
    {
        Response response;
        var user = _context.Users.Include(c => c.WorkItems).FirstOrDefault(t => t.Id == userId);

        if (user is null)
        {
            response = Response.NotFound;
        }else if (user.WorkItems.Any() && !force)
        {
            response = Response.Conflict;
        }
        else if (user.WorkItems.Any() && force)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
            response = Response.Deleted;
        }
        else
        {
            _context.Users.Remove(user);
            _context.SaveChanges();

            response = Response.Deleted;
        }

        return response;

    }
}