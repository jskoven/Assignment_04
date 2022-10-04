using Assignment3.Core;
using Microsoft.EntityFrameworkCore;
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
        
        var testTag = new Tag {Id = 1, Name = "testTagName"};
        context.Tags.Add(testTag);
        context.WorkItems.Add(new WorkItem("workitem1") { Tags = new List<Tag>(){testTag}, state = State.Active, Description = "workItemDescription"});
        context.SaveChanges();
        
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
        id.Should().Be(2);
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
        id2.Should().Be(3);
        id1.Should().Be(2);
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
    public void read_should_return_name_of_tag_when_given_id()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new TagCreateDTO("tag1"));
        var actual = _repository.Read(2);
        //Assert
        actual.Name.Should().Be("tag1");
    }

    [Fact]
    public void read_should_still_give_correct_name_with_more_tags()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new TagCreateDTO("tag1"));
        var (response2, id2) = _repository.Create(new TagCreateDTO("tag2"));
        var (response3, id3) = _repository.Create(new TagCreateDTO("tag3"));

        //Assert
        var actual = _repository.Read(4);
        actual.Name.Should().Be("tag3");
    }

    [Fact]
    public void update_should_change_tag()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new TagCreateDTO("tagOriginal"));
        
        //Assert
        var actual = _repository.Read(2);
        actual.Name.Should().Be("tagOriginal");
        
        //Act
        var update =  _repository.Update(new TagUpdateDTO(2, "tagNew"));
        
        //Assert
        actual = _repository.Read(2);
        actual.Name.Should().Be("tagNew");
        update.Should().Be(Response.Updated);

    }
    [Fact]
    public void updating_non_existent_tag_should_give_NotFound()
    {
        //Arrange
        var actual = _repository.Update(new TagUpdateDTO(2, "tagNew"));
        
        //Assert
        actual.Should().Be(Response.NotFound);

    }

    [Fact]
    public void Delete_should_remove_tag()
    {
        //Arrange
        var (response1, id1) = _repository.Create(new TagCreateDTO("tag"));

        //act
        var deletion = _repository.Delete(2);
        
        //Assert
        deletion.Should().Be(Response.Deleted);
        var found = _repository.Read(2);
        found.Should().Be(null);

    }

    [Fact]
    public void delete_given_tag_with_workitem_should_give_conflict()
    {
        var response = _repository.Delete(1);
        response.Should().Be(Response.Conflict);

    }

    [Fact]
    public void read_all_should_contain_all_tags()
    {
        var (response1, id1) = _repository.Create(new TagCreateDTO("tag1"));
        var (response2, id2) = _repository.Create(new TagCreateDTO("tag2"));
        var (response3, id3) = _repository.Create(new TagCreateDTO("tag3"));

        var actual = _repository.ReadAll();

        actual.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void read_all_should_only_have_one_when_none_added()
    {
        //There is one element in the returned collection, since a single tag
        //has been added in the constructor to test the deletion conflict method.
        var actual = _repository.ReadAll();
        actual.Should().HaveCountLessOrEqualTo(1);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
