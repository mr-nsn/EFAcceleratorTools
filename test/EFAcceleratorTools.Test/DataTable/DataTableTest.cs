using Apparatus.AOT.Reflection;
using EFAcceleratorTools.DataTables;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Test.Customization;
using EFAcceleratorTools.Test.Fixtures.Courses;

namespace EFAcceleratorTools.Test.DataTable;

[Collection(nameof(CourseCollection))]
public class DataTableTest
{
    private readonly CourseFixture _courseFixture;

    public DataTableTest(CourseFixture courseFixture)
    {
        _courseFixture = courseFixture;
        _courseFixture.Reset();
    }

    [Theory(DisplayName = "ToDataTable - Should create an ordered datatable")]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    [InlineAutoDataCustom()]
    public void ToDataTable_ShouldCreateAnOrderedDataTable(List<Course> courses)
    {
        // Act
        var columnsOrder = new Dictionary<string, int>()
        {
            { "Id", 0 },
            { "InstructorId", 1 },
            { "Title", 2 }
        };

        var dataTable = courses.ToDataTable(columnsOrder, _courseFixture.Context);

        // Assert
        Assert.NotNull(dataTable);
        Assert.Equal(courses.Count, dataTable.Rows.Count);

        for (int i = 0; i < courses.Count; i++)
        {
            var course = courses[i];
            var row = dataTable.Rows[i];
            Assert.Equal(course.Id, row.ItemArray[0]);
            Assert.Equal(course.InstructorId, row.ItemArray[1]);
            Assert.Equal(course.Title, row.ItemArray[2]);
        }
    }
}
