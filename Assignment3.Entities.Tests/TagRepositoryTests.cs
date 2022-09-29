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
    public void Test()
    {
        var (response, id) = _repository.Create(new TagCreateDTO("test"));
        response.Should().Be(Response.Created);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
