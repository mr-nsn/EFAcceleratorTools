using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using EFAcceleratorTools.Interfaces;
using EFAcceleratorTools.Models;
using EFAcceleratorTools.ParallelProcessing;
using EFAcceleratorTools.Repository;
using Microsoft.EntityFrameworkCore;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;

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

    public async Task<ICollection<Course>> MassiveQueryAsync()
    {
        return await ParallelQueryExecutor.DoItParallelAsync
        (
            () => _dataContextFactory.CreateDbContext().Courses.AsQueryable(),
            new ParallelParams
            {
                TotalRegisters = _dataContext.Courses.Count(),
                BatchSize = 1,
                MaximumDegreeOfParalelism = Environment.ProcessorCount,
                MaxDegreeOfProcessesPerThread = 1
            },
            _logger
        );
    }
}
