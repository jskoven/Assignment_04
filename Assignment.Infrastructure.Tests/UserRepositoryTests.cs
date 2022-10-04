using Assignment3.Core;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Entities.Tests;

public class UserRepositoryTests : IDisposable
{
    
    private readonly KanbanContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();

        var testuser = new User() {Id = 1, Name = "testUser",Email = "testUserEmail@TestGmail.com",WorkItems = new List<WorkItem>()};
        context.Users.Add(testuser);
        var testWorkItem = new WorkItem("workitem1"){ user = testuser, state = State.Active, Description = "workItemDescription" };
        context.WorkItems.Add(testWorkItem);
        testuser.WorkItems.Add(testWorkItem);
        context.SaveChanges();
        
        _context = context;
        _repository = new UserRepository(_context);
    }

    [Fact]
    public void create_returns_created()
    {
        //Arrange
        var (response,userId) = _repository.Create(new UserCreateDTO("John", "John@gmail.com"));

        //Assert
        response.Should().Be(Response.Created);
    }

    [Fact]
    public void two_with_same_email_should_be_conflict()
    {
        var (response1,userId1) = _repository.Create(new UserCreateDTO("John", "John@gmail.com"));
        var (response2,userId2) = _repository.Create(new UserCreateDTO("Johnny", "John@gmail.com"));

        response2.Should().Be(Response.Conflict);
    }

    [Fact]
    public void read_should_return_correct_user()
    {
        var (response1,userId1) = _repository.Create(new UserCreateDTO("John", "John@gmail.com"));

        var actual = _repository.Read(2);
        actual.Email.Should().Be("John@gmail.com");
    }

    [Fact]
    public void read_with_no_user_should_return_null()
    {
        var actual = _repository.Read(2);
        actual.Should().Be(null);
    }

    [Fact]
    public void read_should_still_give_correct_name_with_more_users()
    {
        var (response1,userId1) = _repository.Create(new UserCreateDTO("John", "John1@gmail.com"));
        var (response2,userId2) = _repository.Create(new UserCreateDTO("John", "John2@gmail.com"));
        var (response3,userId3) = _repository.Create(new UserCreateDTO("John", "John3@gmail.com"));

        var actual = _repository.Read(2);
        actual.Email.Should().Be("John1@gmail.com");
    }

    [Fact]
    public void update_should_change_user()
    {
        var (response1,userId1) = _repository.Create(new UserCreateDTO("John", "John1@gmail.com"));
        var response = _repository.Update(new UserUpdateDTO(2, "John", "JohnsNewEmail@gmail.com"));
        var updated = _repository.Read(2);

        response.Should().Be(Response.Updated);
        updated.Email.Should().Be("JohnsNewEmail@gmail.com");

    }

    [Fact]
    public void updating_non_existent_user_should_give_NotFound()
    {
        var response = _repository.Update(new UserUpdateDTO(2, "John", "JohnsNewEmail@gmail.com"));

        response.Should().Be(Response.NotFound);
    }

    [Fact]
    public void Delete_should_remove_user()
    {
        //Arrange
        var (response,userId) = _repository.Create(new UserCreateDTO("John", "John@gmail.com"));

        //act
        var deletion = _repository.Delete(2);
        
        //Assert
        deletion.Should().Be(Response.Deleted);
        var found = _repository.Read(2);
        found.Should().Be(null);
    }
    
    [Fact]
    public void delete_given_user_with_workitem_should_give_conflict()
    {
        var response = _repository.Delete(1);
        response.Should().Be(Response.Conflict);
    }
    
    [Fact]
    public void read_all_should_contain_all_users()
    {
        var (response1,userId1) = _repository.Create(new UserCreateDTO("John", "John1@gmail.com"));
        var (response2,userId2) = _repository.Create(new UserCreateDTO("John", "John2@gmail.com"));
        var (response3,userId3) = _repository.Create(new UserCreateDTO("John", "John3@gmail.com"));

        var actual = _repository.ReadAll();

        actual.Should().HaveCountGreaterThanOrEqualTo(3);
    }
    
    [Fact]
    public void read_all_should_only_have_one_when_none_added()
    {
        //There is one element in the returned collection, since a single user
        //has been added in the constructor to test the deletion conflict method.
        var actual = _repository.ReadAll();
        actual.Should().HaveCountLessOrEqualTo(1);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
