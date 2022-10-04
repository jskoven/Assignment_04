using Assignment3.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace Assignment3.Entities.Tests;

public class WorkItemRepositoryTests:IDisposable
{
    private readonly KanbanContext _context;
    private readonly WorkItemRepository _repository;
    public WorkItemRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);

        context.Database.EnsureCreated();
        _context = context;
        _repository = new WorkItemRepository(_context);
        _context.Database.EnsureCreated();
        _context.Users.Add(new User("Torben", "Torben@Gmail.com"));
        _context.Users.Add(new User("mille", "mille@Gmail.com"));
        _context.Tags.Add(new Tag { Name = "Urgent"});
        _context.Tags.Add(new Tag { Name = "LowPriority"});
        _context.Tags.Add(new Tag { Name = "Hard"});
        _context.Tags.Add(new Tag { Name = "Easy"});
        _context.SaveChanges();

        _repository.Create(new WorkItemCreateDTO("Fix bugs", 1, null, new[] { "Hard", "Urgent" }));
        _repository.Create(new WorkItemCreateDTO("Make tests", 2, "make unit tests to test components", new[] {"LowPriority"}));
        _repository.Create(new WorkItemCreateDTO("Write more code", null, null, null!));
        _context.SaveChanges();
      
    }

    [Fact]
    public void CreateTestSuccess()
    {
        //Arrange
        var (reponse,created)=_repository.Create(new WorkItemCreateDTO("Make Cofee", 1, null, new[] { "Easy", "Urgent" }));
        
        //Assert
        reponse.Should().Be(Response.Created);
        created.Should().Be(4);
        _repository.Read(4).Should().NotBeNull();
    }
    
    [Fact]
    public void CreateTestFailBecauseConflict()
    {
        //Arrange
        var (reponse,created)=_repository.Create(new WorkItemCreateDTO("Fix bugs", 1, null, new[] { "Urgent" }));
        
        //Assert
        reponse.Should().Be(Response.Conflict);
        created.Should().Be(1);
        _repository.Read(4).Should().BeNull();
    }
    
    [Fact]
    public void CreateTestFailBecauseInvalidUser()
    {
        //Arrange
        var (reponse,created)=_repository.Create(new WorkItemCreateDTO("Try Again", 3, null, new[] { "Urgent" }));
        
        //Assert
        reponse.Should().Be(Response.BadRequest);
        created.Should().Be(-1);
        _repository.Read(4).Should().BeNull();
    }
    
    [Fact]
    public void UpdateTestSuccess()
    {
        //Arrange
        var reponse=_repository.Update(new WorkItemUpdateDTO(1, "Fix Bugs", 2,"Get the bugs fixed now",new[] { "Easy"},State.Active));
        _context.SaveChanges();
        //Assert
        reponse.Should().Be(Response.Updated);
        
        _repository.Read(1).Should().BeEquivalentTo(new WorkItemDTO(1, "Fix Bugs", "mille",new[] { "Easy"},State.Active));
    }
    
    [Fact]
    public void UpdateTestFailedBecauseInvalidId()
    {
        //Arrange
        var reponse=_repository.Update(new WorkItemUpdateDTO(5, "Fix Bugs", 2,"Get the bugs fxed now",new[] { "Easy"},State.Active));
        
        //Assert
        reponse.Should().Be(Response.NotFound);
    }
    
    [Fact]
    public void ReadAllTest()
    {
        //Arrange
        var items = _repository.ReadAll();
        
        //Assert
        items.Should().BeEquivalentTo(new[]
        {
            new WorkItemDTO(1, "Fix bugs", "Torben",  new[] { "Hard", "Urgent" }.ToList(), State.New),
            new WorkItemDTO(2, "Make tests", "mille", new[] { "LowPriority" }.ToList(), State.New)
        ,new WorkItemDTO(3, "Write more code", null!, new string[] { }.ToList(), State.New)});
    }

    [Fact]
    public void ReadAllRemovedTest()
    {
        //Arrange
        _repository.Update(new WorkItemUpdateDTO(1, "Fix Bugs", 2, "Get the bugs fixed now", new[] { "Easy" },
            State.Active));
        _repository.Update(new WorkItemUpdateDTO(3, "idk", 2, "Get the bugs fixed now", new[] { "Easy" },
            State.Active));

        _repository.Delete(1);
        _repository.Delete(3);

        var items = _repository.ReadAllRemoved();

        //Assert
        items.Select(c => c.Id).Should().BeEquivalentTo(new[] { 1, 3 });
    }
    
    [Fact]
    public void ReadAllByTagTest()
    {
        //Arrange
        var items = _repository.ReadAllByTag("Hard");

        //Assert
        items.Select(c => c.Id).Should().BeEquivalentTo(new[] { 1});
    }
    
    [Fact]
    public void ReadAllByUserTest()
    {
        //Arrange
        var items = _repository.ReadAllByUser(2);

        //Assert
        items.Select(c => c.Id).Should().BeEquivalentTo(new[] { 2});
    }
    
    [Fact]
    public void ReadAllByStateTest()
    {
        //Arrange
        var items = _repository.ReadAllByState(State.New);

        //Assert
        items.Select(c => c.Id).Should().BeEquivalentTo(new[] { 1,2,3});
    }

    [Fact]
    public void ReadSuccess()
    {
        //Arrange
        var read = _repository.Read(3);
        
        //Assert
        read.Should()
            .BeEquivalentTo(new WorkItemDTO(3, "Write more code", null!, new string[] { }.ToList(), State.New));
        read.Created.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(5));
        read.StateUpdated.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(5));
    }
    
    [Fact]
    public void ReadFailBecauseInvalidId()
    {
        //Arrange
        var read = _repository.Read(4);
        
        //Assert
        read.Should().BeNull();
    }

    [Fact]
    public void DeleteTestSuccessDeleteFromNew()
    {
        //Arrange
        var response = _repository.Delete(1);
        
        //Assert
        response.Should().Be(Response.Deleted);
        _repository.Read(1).Should().BeNull();
    }
    
    [Fact]
    public void DeleteTestSuccessDeleteFromActive()
    {
        //Arrange
        _repository.Update(new WorkItemUpdateDTO(3, "idk", 2, "Get the bugs fixed now", new[] { "Easy" },
            State.Active));
        var response = _repository.Delete(3);
        
        //Assert
        response.Should().Be(Response.Deleted);
        _repository.Read(3).State.Should().Be(State.Removed);
    }
    
    [Fact]
    public void DeleteTestFailBecauseInvalidId()
    {
        //Arrange
        var response = _repository.Delete(4);
        
        //Assert
        response.Should().Be(Response.NotFound);
    }
    
    [Fact]
    public void DeleteTestFailBecauseTryingToDeleteResolved()
    {
        //Arrange
        _repository.Update(new WorkItemUpdateDTO(1, "Fix Bugs", 2, "Get the bugs fixed now", new[] { "Easy" },
            State.Resolved));
        var response = _repository.Delete(1);
        
        //Assert
        response.Should().Be(Response.Conflict);
        _repository.Read(1).Should().NotBeNull();
    }
    
    
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
