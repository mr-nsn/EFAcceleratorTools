using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses.Selects;
using EFAcceleratorTools.Test.Customization;
using EFAcceleratorTools.Test.Fixtures.Courses;

namespace EFAcceleratorTools.Test.Repository
{
    [Collection(nameof(CourseCollection))]
    public class GenericRepositoryTest
    {
        private readonly CourseFixture _courseFixture;

        public GenericRepositoryTest(CourseFixture courseFixture)
        {
            _courseFixture = courseFixture;
            _courseFixture.Reset();
        }

        [Theory(DisplayName = "CourseRepository - DynamicSelectAsync - Should returns only the fields passed as parameter")]
        [InlineAutoDataCustom()]
        [InlineAutoDataCustom()]
        [InlineAutoDataCustom()]
        [InlineAutoDataCustom()]
        [InlineAutoDataCustom()]
        public async Task DynamicSelectAsync_ShouldReturnsOnlyFieldsPassedAsParameter(List<Course> courses)
        {
            // Arrange
            var fields = CourseSelects.BasicFields;
            await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

            // Act
            var results = await _courseFixture.RepositoryImpl.DynamicSelectAsync(fields);

            // Assert
            Assert.All(results, (c) =>
            {
                Assert.NotEqual(0, c.Id);
                Assert.NotEqual(string.Empty,c.Title?.Trim());
                Assert.Null(c.InstructorId);
                Assert.Null(c.Instructor);
                Assert.Null(c.Modules);
            });
        }
    }
}
