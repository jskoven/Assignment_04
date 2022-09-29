using Assignment3.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Data.Sqlite;

namespace Assignment3;

internal class KanbanContextFactory : IDesignTimeDbContextFactory<KanbanContext>
{
    public KanbanContext CreateDbContext(string[] args)
    {
        //var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        //var connectionString = configuration.GetConnectionString("ConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>();
        //optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSqlite("Data Source=app.db");

        return new KanbanContext(optionsBuilder.Options);
    }
}