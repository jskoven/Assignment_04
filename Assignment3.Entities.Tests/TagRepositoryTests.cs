using Assignment3.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Assignment3.Entities.Tests;

public class TagRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);

        context.Database.EnsureCreated();
        _context = context;
        _repository = new TagRepository(_context);
    }

    [Fact]
    public void Create_tag_should_return_created_response()
    {
        //Arrange
        var (response, id) = _repository.Create(new TagCreateDTO("tag"));
        //Assert
        response.Should().Be(Response.Created);
        id.Should().Be(1);
    }

    [Fact]
    public void creating_2_tags_should_not_cause_error()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new TagCreateDTO("tag1"));
        var (response2, id2) = _repository.Create(new TagCreateDTO("tag2"));
        //Assert
        response2.Should().Be(Response.Created);
        response1.Should().Be(Response.Created);
        id2.Should().Be(2);
        id1.Should().Be(1);
    }
    
    [Fact]
    public void creating_two_tags_with_identical_names_should_be_conflict()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new TagCreateDTO("tag1"));
        var (response2, id2) = _repository.Create(new TagCreateDTO("tag1"));
        //Assert
        response1.Should().Be(Response.Created);
        response2.Should().Be(Response.Conflict);

    }

    [Fact]
    public void test()
    {
        var (response1, id1) = _repository.Create(new TagCreateDTO("tag1"));
        //var actual = _repository.Read()

        
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
