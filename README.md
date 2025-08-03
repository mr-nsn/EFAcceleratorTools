# EFAcceleratorTools

🚀 High-performance utilities to boost your Entity Framework Core experience.  
Built for developers who need more power, flexibility, and speed when querying data.

EFAcceleratorTools is a .NET library designed to enhance productivity and performance when working with Entity Framework Core. It provides dynamic projection, advanced pagination, parallel query execution, simplified entity mapping, and a robust generic repository pattern—all with a lightweight and extensible design.

📄 License: [MIT](./LICENSE)

---

## ✨ Features

- 🧠 **Dynamic Selects** – Project typed objects based on property names.
- 📄 **Pagination Utilities** – Effortless pagination helpers for IQueryable queries.
- 🧵 **Parallel Query Execution** – Process large query sets in parallel with ease.
- 🗺️ **Simplified Entity Mapping** – Easily configure and manage entity mappings for your domain models.
- 📦 **Generic Repository** – Reusable, extensible repository pattern for CRUD operations and advanced queries.
- 🔧 **Extensible & Lightweight** – Designed to be modular and easy to integrate.
- More features coming soon!

---

## 🚀 Getting Started

Install via NuGet:
``` bash
dotnet add package EFAcceleratorTools --version 1.0.1
```
---

## 🛠️ Usage Examples

### Dynamic Select

Select only specific properties dynamically from your entities:
```csharp
var courses = await _context.Courses
    .DynamicSelect(nameof(Course.Id), nameof(Course.Title))
    .ToListAsync();
```

### Pagination

Effortlessly paginate your queries:
```csharp
var queryFilter = new QueryFilterBuilder<Course>()
    .WithPage(1)
    .WithPageSize(100)
    .WithFields(SelectsDefaults<Course>.BasicFields)
    .Build();

var firstPage = await _context.Courses
    .OrderBy(x => x.Id)
    .GetPagination(queryFilter)
    .ToPaginationResultListAsync();
```

### Entity Mapping

Map your entities with ease:
```csharp
public class CourseMap : EntityTypeConfiguration<Course>
{
    public override void Map(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("TB_COURSE");

        builder.HasKey(x => x.Id)
               .HasName("SQ_COURSE");

        builder.Property(x => x.Id)
               .HasColumnName("SQ_COURSE")
               .HasColumnType("bigint")
               .UseIdentityColumn();

        builder.Property(x => x.InstructorId)
               .HasColumnName("SQ_INSTRUCTOR")
               .HasColumnType("bigint");

        builder.Property(x => x.Title)
               .HasColumnName("TX_TITLE")
               .HasColumnType("nvarchar(100)");

        builder.Property(x => x.CreatedAt)
               .HasColumnName("DT_CREATION")
               .HasColumnType("nvarchar(100)");
    }
}

...

public static class MappingsHolder
{
    public static Dictionary<Type, IEntityTypeConfiguration> GetMappings()
    {
        var mappings = new Dictionary<Type, IEntityTypeConfiguration>();

        mappings.Add(typeof(Course), new CourseMap());

        return mappings;
    }
}

...

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.RegisterModelsMapping(MappingsHolder.GetMappings());
}
```

### Parallel Query Execution
Execute queries in parallel to improve performance on large datasets:
```csharp
ParallelQueryExecutor.DoItParallelAsync
(
    () => _dataContextFactory.CreateDbContext().Courses.OrderBy(x => x.Id).AsQueryable(),
    new ParallelParams
    {
        TotalRegisters = _dataContext.Courses.Count(),
        BatchSize = 1000,
        MaximumDegreeOfParalelism = Environment.ProcessorCount,
        MaxDegreeOfProcessesPerThread = 1
    },
    _logger
);
```

### Generic Repository

Leverage a robust, reusable repository for your entities:
```csharp
public interface ICourseRepository : IGenericRepository<Course> { }

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    private readonly DataContext _dataContext;
    private readonly IDbContextFactory<DataContext> _dataContextFactory;
    private readonly IApplicationLogger _logger;

    public CourseRepository(DataContext context, IDbContextFactory<DataContext> dataContextFactory, IApplicationLogger logger) : base(context, new DbContextFactoryAdapter(dataContextFactory))
    {
        _dataContext = context;
        _dataContextFactory = dataContextFactory;
        _logger = logger;
    }
}

...

var allCourses = await _courseRepository.GetAllAsync();
```

---

## 📚 Example Project

See the `EFAcceleratorTools.Examples` project for a complete working example, including setup and advanced scenarios.

---

## 🤝 Contributing

Contributions are welcome! Please open issues or submit pull requests for new features, bug fixes, or documentation improvements.
Contributing guidelines are still under construction.

---

## 📄 License

This project is licensed under the MIT License.