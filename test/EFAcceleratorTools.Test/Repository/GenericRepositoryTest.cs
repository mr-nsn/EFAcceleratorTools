using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses.Selects;
using EFAcceleratorTools.Models.Builders;
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

    [Theory(DisplayName = "CourseRepository - Add - Should add a course to the database (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void Add_ShouldAddCourse(Course course)
    {
        // Act
        _courseFixture.RepositoryImpl.Add(course);
        _courseFixture.Context.SaveChanges();

        // Assert
        var result = _courseFixture.RepositoryImpl.GetById(course.Id);
        Assert.NotNull(result);
        Assert.Equal(course.Title, result.Title);
    }

    [Theory(DisplayName = "CourseRepository - AddAndCommit - Should add and persist a course (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void AddAndCommit_ShouldAddAndPersistCourse(Course course)
    {
        // Act
        var added = _courseFixture.RepositoryImpl.AddAndCommit(course);

        // Assert
        Assert.NotNull(added);
        Assert.NotEqual(0, added.Id);
        var persisted = _courseFixture.RepositoryImpl.GetById(added.Id);
        Assert.NotNull(persisted);
        Assert.Equal(course.Title, persisted.Title);
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

    [Theory(DisplayName = "CourseRepository - AddRange - Should add multiple courses to the database (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void AddRange_ShouldAddMultipleCourses(List<Course> courses)
    {
        // Act
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Assert
        var results = _courseFixture.RepositoryImpl.GetAll();
        Assert.All(courses, course => Assert.Contains(results, c => c.Title == course.Title));
    }

    [Theory(DisplayName = "CourseRepository - AddRangeAndCommit - Should add and persist multiple courses (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void AddRangeAndCommit_ShouldAddAndPersistCourses(List<Course> courses)
    {
        // Act
        var added = _courseFixture.RepositoryImpl.AddRangeAndCommit(courses);

        // Assert
        Assert.NotNull(added);
        Assert.Equal(courses.Count, added.Count);
        var results = _courseFixture.RepositoryImpl.GetAll();
        Assert.All(courses, course => Assert.Contains(results, c => c.Title == course.Title));
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

    [Theory(DisplayName = "CourseRepository - Any - Should return true if any course matches the predicate (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void Any_ShouldReturnTrueIfAnyCourseMatchesPredicate(Course course)
    {
        // Arrange
        _courseFixture.RepositoryImpl.Add(course);
        _courseFixture.Context.SaveChanges();

        // Act
        var any = _courseFixture.RepositoryImpl.Any(c => c.Title == course.Title);

        // Assert
        Assert.True(any);
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
        Assert.Equal(EntityState.Detached, _courseFixture.Context.Entry(course).State);
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
        Assert.All(courses, c => Assert.Equal(EntityState.Detached, _courseFixture.Context.Entry(c).State));
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

    [Theory(DisplayName = "CourseRepository - DynamicSelect - Should return only selected fields (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void DynamicSelect_ShouldReturnOnlySelectedFields(List<Course> courses)
    {
        // Arrange
        var fields = CourseSelects.BasicFields;
        _courseFixture.RepositoryImpl.AddRangeAndCommit(courses);

        // Act
        var results = _courseFixture.RepositoryImpl.DynamicSelect(fields);

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

    [Theory(DisplayName = "CourseRepository - FindAll - Should return courses matching the predicate (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void FindAll_ShouldReturnMatchingCourses(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Act
        var results = _courseFixture.RepositoryImpl.FindAll(c => c.Title == courses[0].Title);

        // Assert
        Assert.All(results, course => Assert.Equal(courses[0].Title, course.Title));
    }

    [Theory(DisplayName = "CourseRepository - FindAllAsync - Should return courses matching the predicate")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task FindAllAsync_ShouldReturnMatchingCourses(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Example predicate: find courses with Title containing "Test"
        var expected = courses.Where(c => c.Title != null && c.Title.Contains("Test")).ToList();

        // Act
        var results = await _courseFixture.RepositoryImpl.FindAllAsync(c => c.Title != null && c.Title.Contains("Test"));

        // Assert
        Assert.Equal(expected.Count, results.Count);
        Assert.All(results, course => Assert.Contains("Test", course.Title));
    }

    [Theory(DisplayName = "CourseRepository - FindFirst - Should return the first course matching the predicate (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void FindFirst_ShouldReturnFirstMatchingCourse(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Act
        var result = _courseFixture.RepositoryImpl.FindFirst(c => c.Title == courses[0].Title);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(courses[0].Title, result.Title);
    }

    [Theory(DisplayName = "CourseRepository - FindFirstAsync - Should return a single course matching the predicate")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task FindFirstAsync_ShouldReturnSingleMatchingCourse(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Pick a course to search for
        var expected = courses.FirstOrDefault(c => c.Title != null);
        if (expected == null)
            return; // No valid course to test

        // Act
        var result = await _courseFixture.RepositoryImpl.FindFirstAsync(c => c.Title == expected.Title);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Title, result.Title);
        Assert.Equal(expected.Id, result.Id);
    }

    [Theory(DisplayName = "CourseRepository - GetAll - Should return all courses (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void GetAll_ShouldReturnAllCourses(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Act
        var results = _courseFixture.RepositoryImpl.GetAll();

        // Assert
        Assert.All(courses, course => Assert.Contains(results, c => c.Title == course.Title));
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
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        // Act
        var results = await _courseFixture.RepositoryImpl.GetAllAsync();

        // Assert
        Assert.NotEmpty(results);
        Assert.Equal(courses.Count, results.Count);
    }

    [Theory(DisplayName = "CourseRepository - GetById - Should return the correct course (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void GetById_ShouldReturnCourse(Course course)
    {
        // Arrange
        _courseFixture.RepositoryImpl.Add(course);
        _courseFixture.Context.SaveChanges();

        // Act
        var result = _courseFixture.RepositoryImpl.GetById(course.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Title, result.Title);
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

    [Theory(DisplayName = "CourseRepository - Remove - Should remove the course (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void Remove_ShouldRemoveCourse(Course course)
    {
        // Arrange
        _courseFixture.RepositoryImpl.Add(course);
        _courseFixture.Context.SaveChanges();

        // Act
        _courseFixture.RepositoryImpl.Remove(course.Id);
        _courseFixture.Context.SaveChanges();

        // Assert
        var result = _courseFixture.RepositoryImpl.GetById(course.Id);
        Assert.Null(result);
    }

    [Theory(DisplayName = "CourseRepository - RemoveAndCommit - Should remove and persist the course (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void RemoveAndCommit_ShouldRemoveAndPersistCourse(Course course)
    {
        // Arrange
        _courseFixture.RepositoryImpl.Add(course);
        _courseFixture.Context.SaveChanges();

        // Act
        _courseFixture.RepositoryImpl.RemoveAndCommit(course.Id);

        // Assert
        var result = _courseFixture.RepositoryImpl.GetById(course.Id);
        Assert.Null(result);
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

    [Theory(DisplayName = "CourseRepository - RemoveRange - Should remove all courses (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void RemoveRange_ShouldRemoveAllCourses(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Act
        _courseFixture.RepositoryImpl.RemoveRange(courses.Select(c => c.Id).ToArray());
        _courseFixture.Context.SaveChanges();

        // Assert
        var results = _courseFixture.RepositoryImpl.FindAll(c => courses.Select(x => x.Id).Contains(c.Id));
        Assert.Empty(results);
    }

    [Theory(DisplayName = "CourseRepository - RemoveRangeAndCommit - Should remove all courses and persist (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void RemoveRangeAndCommit_ShouldRemoveAllCoursesAndPersist(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Act
        _courseFixture.RepositoryImpl.RemoveRangeAndCommit(courses.Select(c => c.Id).ToArray());

        // Assert
        var results = _courseFixture.RepositoryImpl.FindAll(c => courses.Select(x => x.Id).Contains(c.Id));
        Assert.Empty(results);
    }

    [Theory(DisplayName = "CourseRepository - RemoveRangeAndCommitAsync - Should remove all courses and persist the removal of the courses")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task RemoveRangeAndCommitAsync_ShouldRemoveAllCoursesAndPersist(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAsync(courses);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Act
        await _courseFixture.RepositoryImpl.RemoveRangeAndCommitAsync(courses.Select(c => c.Id).ToArray());

        // Assert
        var results = await _courseFixture.RepositoryImpl.FindAllAsync(c => courses.Select(c => c.Id).Contains(c.Id));
        Assert.Empty(results);
    }

    [Theory(DisplayName = "CourseRepository - RemoveRangeAsync - Should remove all courses after commited")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task RemoveRangeAsync_ShouldRemoveAllCoursesAndPersist(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAsync(courses);
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Act
        await _courseFixture.RepositoryImpl.RemoveRangeAsync(courses.Select(c => c.Id).ToArray());
        await _courseFixture.RepositoryImpl.CommitAsync();

        // Assert
        var results = await _courseFixture.RepositoryImpl.FindAllAsync(c => courses.Select(c => c.Id).Contains(c.Id));
        Assert.Empty(results);
    }

    [Theory(DisplayName = "CourseRepository - SearchWithPagination - Should return paginated and filtered results (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void SearchWithPagination_ShouldReturnPaginatedAndFilteredResults(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        var page = 1;
        var pageSize = 2;
        var fields = CourseSelects.BasicFields;

        var queryFilter = new QueryFilterBuilder<Course>()
            .WithPage(page)
            .WithPageSize(pageSize)
            .WithFields(fields)
            .Build();

        // Act
        var result = _courseFixture.RepositoryImpl.SearchWithPagination(queryFilter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(courses.Count, result.TotalRecords);
        Assert.NotNull(result.Result);
        Assert.True(result.Result.Count <= pageSize);
        Assert.All(result.Result, course =>
        {
            Assert.NotEqual(0, course.Id);
            Assert.False(string.IsNullOrWhiteSpace(course.Title));
        });
    }

    [Theory(DisplayName = "CourseRepository - SearchWithPaginationAsync - Should return paginated and filtered results")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public async Task SearchWithPaginationAsync_ShouldReturnPaginatedAndFilteredResults(List<Course> courses)
    {
        // Arrange
        await _courseFixture.RepositoryImpl.AddRangeAndCommitAsync(courses);

        var page = 1;
        var pageSize = 2;
        var fields = CourseSelects.BasicFields;

        var queryFilter = new QueryFilterBuilder<Course>()
            .WithPage(page)
            .WithPageSize(pageSize)
            .WithFields(fields)
            .Build();

        // Act
        var result = await _courseFixture.RepositoryImpl.SearchWithPaginationAsync(queryFilter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(courses.Count, result.TotalRecords);
        Assert.NotNull(result.Result);
        Assert.True(result.Result.Count <= pageSize);

        // Verifica se os campos retornados são os esperados
        Assert.All(result.Result, course =>
        {
            Assert.NotEqual(0, course.Id);
            Assert.False(string.IsNullOrWhiteSpace(course.Title));
            Assert.Null(course.InstructorId);
            Assert.Null(course.Instructor);
            Assert.Null(course.Modules);
        });
    }

    [Theory(DisplayName = "CourseRepository - Update - Should update the course (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void Update_ShouldUpdateCourse(Course course)
    {
        // Arrange
        _courseFixture.RepositoryImpl.Add(course);
        _courseFixture.Context.SaveChanges();

        // Act
        course.Title = "Updated Sync";
        _courseFixture.RepositoryImpl.Update(course);
        _courseFixture.Context.SaveChanges();

        // Assert
        var result = _courseFixture.RepositoryImpl.GetById(course.Id);
        Assert.Equal("Updated Sync", result.Title);
    }

    [Theory(DisplayName = "CourseRepository - UpdateAndCommit - Should update and persist the course (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void UpdateAndCommit_ShouldUpdateAndPersistCourse(Course course)
    {
        // Arrange
        _courseFixture.RepositoryImpl.Add(course);
        _courseFixture.Context.SaveChanges();

        // Act
        course.Title = "UpdatedAndCommitted Sync";
        var updated = _courseFixture.RepositoryImpl.UpdateAndCommit(course);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("UpdatedAndCommitted Sync", updated.Title);
        var persisted = _courseFixture.RepositoryImpl.GetById(course.Id);
        Assert.Equal("UpdatedAndCommitted Sync", persisted.Title);
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

    [Theory(DisplayName = "CourseRepository - UpdateRange - Should update multiple courses (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void UpdateRange_ShouldUpdateMultipleCourses(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Act
        foreach (var course in courses)
            course.Title += " Updated";
        _courseFixture.RepositoryImpl.UpdateRange(courses);
        _courseFixture.Context.SaveChanges();

        // Assert
        var results = _courseFixture.RepositoryImpl.GetAll();
        Assert.All(courses, course => Assert.Contains(results, c => c.Title == course.Title));
    }

    [Theory(DisplayName = "CourseRepository - UpdateRangeAndCommit - Should update and persist multiple courses (sync)")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void UpdateRangeAndCommit_ShouldUpdateAndPersistCourses(List<Course> courses)
    {
        // Arrange
        _courseFixture.RepositoryImpl.AddRange(courses);
        _courseFixture.Context.SaveChanges();

        // Act
        foreach (var course in courses)
            course.Title += " Updated";
        var updatedCourses = _courseFixture.RepositoryImpl.UpdateRangeAndCommit(courses);

        // Assert
        Assert.NotNull(updatedCourses);
        Assert.Equal(courses.Count, updatedCourses.Count);
        var results = _courseFixture.RepositoryImpl.GetAll();
        Assert.All(courses, course => Assert.Contains(results, c => c.Title == course.Title));
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
