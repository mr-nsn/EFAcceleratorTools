using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses.Selects;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using EFAcceleratorTools.Interfaces;
using EFAcceleratorTools.Models;
using EFAcceleratorTools.ParallelProcessing;
using EFAcceleratorTools.Repository;
using EFAcceleratorTools.Select;
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
            () => _dataContextFactory.CreateDbContext().Courses.OrderBy(x => x.Id).AsQueryable(),
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

    public async Task RemoveRangeCascadeAndCommitAsync(params long[] ids)
    {
        var courses = await _dataContext.Courses
            .Where(c => ids.Contains(c.Id))
            .DynamicSelect(CourseSelects.AllRelationships)
            .ToArrayAsync();

        var profiles = courses
            .Where(c => c.Instructor?.Profile is not null)
            .Select(c => c.Instructor?.Profile!)
            .ToArray();

        var instructors = courses
            .Where(c => c.Instructor is not null)
            .Select(c => c.Instructor!)
            .ToArray();

        var lessons = courses
            .Where(c => c.Modules?.Count > 0)
            .SelectMany(c => c.Modules!)
            .Where(m => m.Lessons?.Count > 0)
            .SelectMany(m => m.Lessons!)
            .ToArray();

        var modules = courses
            .Where(c => c.Modules?.Count > 0)
            .SelectMany(c => c.Modules!)
            .ToArray();

        _context.RemoveRange(profiles);
        _context.RemoveRange(instructors);
        _context.RemoveRange(lessons);
        _context.RemoveRange(modules);
        _context.RemoveRange(courses);

        await CommitAsync();
    }
}
