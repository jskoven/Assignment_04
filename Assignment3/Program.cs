using Assignment3.Core;
using Assignment3.Entities;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
var connectionString = configuration.GetConnectionString("ConnectionString");

var context=new KanbanContextFactory().CreateDbContext(args);

var repository = new WorkItemRepository(context);

repository.Create(new WorkItemCreateDTO("HEy",null,null,new List<string>()));