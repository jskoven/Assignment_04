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
            entity.Property(e => e.Tasks);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.Property(e => e.AssignedTo).HasMaxLength(50);

            entity.Property(e => e.Description);
            entity.Property(e => e.state).HasConversion(v =>v.ToString(),
                v =>(Task.State)Enum.Parse(typeof(Task.State),v));

            entity.HasMany(d => d.Tags)
                .WithMany(p => p.Tasks);
        });
        
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            
            entity.HasMany(d => d.Tasks)
                .WithMany(p => p.Tags);
        });
        
    }
    


}
