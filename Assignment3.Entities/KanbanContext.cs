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

    public virtual DbSet<WorkItem> Tasks { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(100).IsRequired();

            entity.Property(e => e.Description);
            
            entity.Property(e => e.state).HasConversion(v =>v.ToString(),
                v =>(WorkItem.State)Enum.Parse(typeof(WorkItem.State),v));
            
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasMany(e => e.WorkItems)
                .WithMany(e => e.Tags)
                .UsingEntity(e => e.ToTable("WorkItemsTags"));
            entity.HasKey(c => c.Id);


        });
        

    }
    


}
