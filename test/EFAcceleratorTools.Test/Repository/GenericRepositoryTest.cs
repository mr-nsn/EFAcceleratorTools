using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses.Selects;
using EFAcceleratorTools.Test.Customization;
using EFAcceleratorTools.Test.Fixtures.Courses;
using Microsoft.EntityFrameworkCore;

namespace EFAcceleratorTools.Test.Repository;

[Collection(nameof(CourseCollection))]
public class GenericRepositoryTest
{
    private readonly CourseFixture _courseFixture;

    public GenericRepositoryTest(CourseFixture courseFixture)
    {
        _courseFixture = courseFixture;
        _courseFixture.Reset();
    }

    [Theory(DisplayName = "CourseRepository - AddAndCommitAsync - Should add and persist a course")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task AddAndCommitAsync_ShouldAddAndPersistCourse(Course course)
    {
        // Act
        var added = await _courseFixture.RepositoryImpl.AddAndCommitAsync(course);

        // Assert
        Assert.NotNull(added);
        Assert.NotEqual(0, added.Id);
        Assert.Equal(course.Title, added.Title);

        var persisted = await _courseFixture.RepositoryImpl.GetByIdAsync(added.Id);
        Assert.NotNull(persisted);
        Assert.Equal(course.Title, persisted.Title);
    }

    [Theory(DisplayName = "CourseRepository - AddAsync - Should add a course to the database")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task AddAsync_ShouldAddCourse(Course course)
    {
        // Act
        await _courseFixture.RepositoryImpl.AddAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Assert
        var result = await _courseFixture.RepositoryImpl.GetByIdAsync(course.Id);
        Assert.NotNull(result);
        Assert.Equal(course.Title, result.Title);
    }

    [Theory(DisplayName = "CourseRepository - AddRangeAndCommitAsync - Should add and persist multiple courses")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task AddRangeAndCommitAsync_ShouldAddAndPersistCourses(List<Course> courses)
    {
        // Act
        var added = await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Assert
        Assert.NotNull(added);
        Assert.Equal(courses.Count, added.Count);

        Assert.All(added, course =>
        {
            Assert.NotEqual(0, course.Id);
            Assert.Contains(courses, c => c.Title == course.Title);
            var persisted = _courseFixture.RepositoryImpl.GetByIdAsync(course.Id).Result;
            Assert.NotNull(persisted);
            Assert.Equal(course.Title, persisted.Title);
        });
    }

    [Theory(DisplayName = "CourseRepository - AddRangeAsync - Should add multiple courses to the database")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task AddRangeAsync_ShouldAddMultipleCourses(List<Course> courses)
    {
        // Act
        await _courseFixture.RepositoryImpl.AddRangeAsync(courses);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Assert
        var results = await _courseFixture.RepositoryImpl.GetAllAsync();
        Assert.All(courses, course =>
            Assert.Contains(results, c => c.Title == course.Title));
    }

    [Theory(DisplayName = "CourseRepository - AnyAsync - Should return true if any course matches the predicate, otherwise false")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task AnyAsync_ShouldReturnTrueIfAnyCourseMatchesPredicate(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Act
        var anyWithTest = await _courseFixture.RepositoryImpl.AnyAsync(c => c.Title != null && c.Title.Contains("Test"));

        // Assert
        var expected = courses.Any(c => c.Title != null && c.Title.Contains("Test"));
        Assert.Equal(expected, anyWithTest);
    }

    [Theory(DisplayName = "CourseRepository - CommitAsync - Should persist changes and return affected rows count")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task CommitAsync_ShouldPersistChanges(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAsync(courses);

        // Act
        var affectedRows = await _courseFixture.RepositoryImpl.CommitAsync();

        // Assert
        Assert.True(affectedRows > 0);

        var persisted = await _courseFixture.RepositoryImpl.GetAllAsync();
        Assert.Equal(courses.Count, persisted.Count);
    }

    [Theory(DisplayName = "CourseRepository - Detach - Should detach the entity from the context")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task Detach_ShouldDetachEntityFromContext(Course course)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Act
        var exception = Record.Exception(() => _courseFixture.RepositoryImpl.Detach(course));

        // Assert
        Assert.Null(exception);
        Assert.True(_courseFixture.Context.Entry(course).State == EntityState.Detached);
    }

    [Theory(DisplayName = "CourseRepository - DetachAll - Should detach all entities from the context")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task DetachAll_ShouldDetachAllEntitiesFromContext(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Act
        var exception = Record.Exception(() => _courseFixture.RepositoryImpl.DetachAll());

        // Assert
        Assert.Null(exception);
        Assert.All(courses, c => Assert.True(_courseFixture.Context.Entry(c).State == EntityState.Detached));
    }

    [Fact(DisplayName = "CourseRepository - DisableChangeTracker - Should disable change tracking without exception")]
    public void DisableChangeTracker_ShouldNotThrow()
    {
        // Act
        var exception = Record.Exception(() => _courseFixture.RepositoryImpl.DisableChangeTracker());

        // Assert
        Assert.Null(exception);
        Assert.False(_courseFixture.Context.ChangeTracker.AutoDetectChangesEnabled);
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
            Assert.NotEqual(string.Empty, c.Title?.Trim());
            Assert.Null(c.InstructorId);
            Assert.Null(c.Instructor);
            Assert.Null(c.Modules);
        });
    }

    [Fact(DisplayName = "CourseRepository - EnableChangeTracker - Should enable change tracking without exception")]
    public void EnableChangeTracker_ShouldNotThrow()
    {
        // Act
        var exception = Record.Exception(() => _courseFixture.RepositoryImpl.EnableChangeTracker());

        // Assert
        Assert.Null(exception);
        Assert.True(_courseFixture.Context.ChangeTracker.AutoDetectChangesEnabled);
    }

    [Theory(DisplayName = "CourseRepository - FindAsync - Should return courses matching the predicate")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task FindAsync_ShouldReturnMatchingCourses(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Example predicate: find courses with Title containing "Test"
        var expected = courses.Where(c => c.Title != null && c.Title.Contains("Test")).ToList();

        // Act
        var results = await _courseFixture.RepositoryImpl.FindAsync(c => c.Title != null && c.Title.Contains("Test"));

        // Assert
        Assert.Equal(expected.Count, results.Count);
        Assert.All(results, course => Assert.Contains("Test", course.Title));
    }

    [Theory(DisplayName = "CourseRepository - FindByAsync - Should return a single course matching the predicate")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task FindByAsync_ShouldReturnSingleMatchingCourse(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Pick a course to search for
        var expected = courses.FirstOrDefault(c => c.Title != null);
        if (expected == null)
            return; // No valid course to test

        // Act
        var result = await _courseFixture.RepositoryImpl.FindByAsync(c => c.Title == expected.Title);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Title, result.Title);
        Assert.Equal(expected.Id, result.Id);
    }

    [Theory(DisplayName = "CourseRepository - GetAllAsync - Should returns all the database records of the entity")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task GetAllAsync_ShouldReturnsAllDataBaseRecords(List<Course> courses)
    {
        // Arrange
        var fields = CourseSelects.BasicFields;
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Act
        var results = await _courseFixture.RepositoryImpl.GetAllAsync();

        // Assert
        Assert.NotEmpty(results);
        Assert.Equal(courses.Count, results.Count);
    }

    [Theory(DisplayName = "CourseRepository - GetByIdAsync - Should return the correct course")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task GetByIdAsync_ShouldReturnCourse(Course course)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Act
        var result = await _courseFixture.RepositoryImpl.GetByIdAsync(course.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Title, result.Title);
    }

    [Theory(DisplayName = "CourseRepository - RemoveAndCommitAsync - Should remove and persist removal of the course")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task RemoveAndCommitAsync_ShouldRemoveAndPersistCourse(Course course)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Act
        await _courseFixture.RepositoryImpl.RemoveAndCommitAsync(course.Id);

        // Assert
        var result = await _courseFixture.RepositoryImpl.GetByIdAsync(course.Id);
        Assert.Null(result);
    }

    [Theory(DisplayName = "CourseRepository - RemoveAsync - Should remove the course")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task RemoveAsync_ShouldRemoveCourse(Course course)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        await _courseFixture.RepositoryImpl.RemoveAsync(course.Id);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Act
        var result = await _courseFixture.RepositoryImpl.GetByIdAsync(course.Id);

        // Assert
        Assert.Null(result);
    }

    [Theory(DisplayName = "CourseRepository - UpdateAndCommitAsync - Should update and persist a course")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task UpdateAndCommitAsync_ShouldUpdateAndPersistCourse(Course course)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        course.Title = "Updated And Committed Title";

        // Act
        var updated = await _courseFixture.RepositoryImpl.UpdateAndCommitAsync(course);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Updated And Committed Title", updated.Title);

        var persisted = await _courseFixture.RepositoryImpl.GetByIdAsync(course.Id);
        Assert.NotNull(persisted);
        Assert.Equal("Updated And Committed Title", persisted.Title);
    }

    [Theory(DisplayName = "CourseRepository - UpdateAsync - Should update the course")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task UpdateAsync_ShouldUpdateCourse(Course course)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        course.Title = "Updated Title";
        await _courseFixture.RepositoryImpl.UpdateAsync(course);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Act
        var result = await _courseFixture.RepositoryImpl.GetByIdAsync(course.Id);

        // Assert
        Assert.Equal("Updated Title", result?.Title);
    }

    [Theory(DisplayName = "CourseRepository - UpdateRangeAndCommitAsync - Should update and persist multiple courses")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task UpdateRangeAndCommitAsync_ShouldUpdateAndPersistCourses(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Update all course titles
        var updatedTitle = "Batch Updated And Committed Title";
        foreach (var course in courses)
            course.Title = updatedTitle;

        // Act
        var updatedCourses = await _courseFixture.RepositoryImpl.UpdateRangeAndCommitAsync(courses);

        // Assert
        Assert.NotNull(updatedCourses);
        Assert.Equal(courses.Count, updatedCourses.Count);
        Assert.All(updatedCourses, c => Assert.Equal(updatedTitle, c.Title));

        var persisted = await _courseFixture.RepositoryImpl.GetAllAsync();
        Assert.All(persisted, c => Assert.Equal(updatedTitle, c.Title));
    }

    [Theory(DisplayName = "CourseRepository - UpdateRangeAsync - Should update multiple courses")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task UpdateRangeAsync_ShouldUpdateMultipleCourses(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Update all course titles
        var updatedTitle = "Batch Updated Title";
        foreach (var course in courses)
            course.Title = updatedTitle;

        // Act
        await _courseFixture.RepositoryImpl.UpdateRangeAsync(courses);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Assert
        var results = await _courseFixture.RepositoryImpl.GetAllAsync();
        Assert.All(results, c => Assert.Equal(updatedTitle, c.Title));
    }
}
