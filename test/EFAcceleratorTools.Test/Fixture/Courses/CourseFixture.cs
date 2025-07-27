using EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;
using Moq;

namespace EFAcceleratorTools.Test.Fixtures.Courses;

public class CourseFixture : FixtureBase
{
    public Mock<ICourseRepository> RepositoryMock { get; private set; }

    public CourseRepository RepositoryImpl { get; private set; }

    public CourseFixture()
    {
        RepositoryMock = new Mock<ICourseRepository>();
        RepositoryImpl = new CourseRepository(Context);
    }

    public override void Reset()
    {
        base.Reset();

        RepositoryMock = new Mock<ICourseRepository>();
        RepositoryImpl = new CourseRepository(Context);
    }
}
