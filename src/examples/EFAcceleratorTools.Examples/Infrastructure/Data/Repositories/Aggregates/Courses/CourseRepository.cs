using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using EFAcceleratorTools.Repository;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(DataContext context) : base(context)
    {
        
    }
}
