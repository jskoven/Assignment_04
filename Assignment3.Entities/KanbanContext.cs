using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Assignment3.Entities;

public class KanbanContext: DbContext
{
    public KanbanContext(DbContextOptions<KanbanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();

            entity.Property(e => e.Description);
            
            entity.Property(e => e.state).HasConversion(v =>v.ToString(),
                v =>(Task.State)Enum.Parse(typeof(Task.State),v));
            
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasMany(e => e.Tasks)
                .WithMany(e => e.Tags)
                .UsingEntity(e => e.ToTable("TaskTags"));
            
        });

    }
    


}
