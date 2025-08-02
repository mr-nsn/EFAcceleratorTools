using EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;
using EFAcceleratorTools.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace EFAcceleratorTools.Test.Fixtures.Courses;

public class CourseFixture : FixtureBase
{
    public Mock<ICourseRepository> RepositoryMock { get; private set; }

    public CourseRepository RepositoryImpl { get; private set; }

    public CourseFixture()
    {
        var logger = new Mock<IApplicationLogger>().Object;

        RepositoryMock = new Mock<ICourseRepository>();
        RepositoryImpl = new CourseRepository(Context, ContextFactory, logger);
    }

    public override void Reset()
    {
        base.Reset();

        var logger = new Mock<IApplicationLogger>().Object;

        RepositoryMock = new Mock<ICourseRepository>();
        RepositoryImpl = new CourseRepository(Context, ContextFactory, logger);
    }
}
