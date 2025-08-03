using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Repository;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<ICollection<Course>> MassiveQueryAsync();
    Task RemoveRangeCascadeAndCommitAsync(params long[] ids);
}
