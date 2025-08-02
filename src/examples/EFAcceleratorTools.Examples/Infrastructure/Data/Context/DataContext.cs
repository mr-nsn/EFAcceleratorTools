using EFAcceleratorTools.Examples.Configs;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Examples.Infrastructure.Data.Mappings;
using EFAcceleratorTools.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Context;

public class DataContextFactory : IDbContextFactory<DataContext>
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public DataContextFactory(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public virtual DataContext CreateDbContext()
    {
        var appConfig = _configuration.GetSection("ConsoleAppConfiguration");
        var config = appConfig.Get<ConsoleAppConfiguration>();

        var optionsBuilder = new DbContextOptionsBuilder();
        var connectionString = config?.ConnectionConfiguration.ConnectionStrings.DefaultConnection;
        var timeout = config?.ConnectionConfiguration.MaxTimeoutInSeconds;

        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseSqlServer(connectionString, options =>
        {
            options.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(Convert.ToDouble(timeout)), errorNumbersToAdd: null);
            options.CommandTimeout(timeout);
        });

        return new DataContext(optionsBuilder.Options);
    }
}

public class DbContextFactoryAdapter : IDbContextFactory<DbContext>
{
    private readonly IDbContextFactory<DataContext> _innerFactory;

    public DbContextFactoryAdapter(IDbContextFactory<DataContext> innerFactory)
    {
        _innerFactory = innerFactory;
    }

    public DbContext CreateDbContext()
    {
        return _innerFactory.CreateDbContext();
    }
}

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
