using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Examples.Infrastructure.Data.Mappings.Aggregates.Courses;
using EFAcceleratorTools.Examples.Infrastructure.Data.Mappings.Aggregates.Instructors;
using EFAcceleratorTools.Interfaces;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Mappings;

public static class MappingsHolder
{
    public static Dictionary<Type, IEntityTypeConfiguration> GetMappings()
    {
        var mappings = new Dictionary<Type, IEntityTypeConfiguration>();

        mappings.Add(typeof(Course), new CourseMap());
        mappings.Add(typeof(Lesson), new LessonMap());
        mappings.Add(typeof(Module), new ModuleMap());
        mappings.Add(typeof(Instructor), new InstructorMap());
        mappings.Add(typeof(Profile), new ProfileMap());

        return mappings;
    }
}
