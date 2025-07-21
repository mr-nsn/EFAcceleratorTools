using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Examples.Infrastructure.Data.Mappings;
using EFAcceleratorTools.Mapping;
using Microsoft.EntityFrameworkCore;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Context;

public class DataContext : DbContext
{
    #region DbSets

    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Lesson> Lessons { get; set; }
    public virtual DbSet<Module> Modules { get; set; }
    public virtual DbSet<Instructor> Instructors { get; set; }
    public virtual DbSet<Profile> Profiles { get; set; }

    #endregion

    public DataContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.RegisterModelsMapping(MappingsHolder.GetMappings());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
