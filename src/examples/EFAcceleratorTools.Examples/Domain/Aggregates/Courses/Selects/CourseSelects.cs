using Apparatus.AOT.Reflection;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;

namespace EFAcceleratorTools.Examples.Domain.Aggregates.Courses.Selects
{
    public static class CourseSelects
    {
        public static KeyOf<Course>[] BasicFields =>
        [
            string.Format("{0}", nameof(Course.Id)),
            string.Format("{0}", nameof(Course.Title)),
            string.Format("{0}", nameof(Course.CreatedAt)),
        ];

        public static KeyOf<Course>[] AllRelationships =>
        [
            string.Format("{0}", nameof(Course.Id)),
            string.Format("{0}", nameof(Course.InstructorId)),
            string.Format("{0}", nameof(Course.Title)),
            string.Format("{0}", nameof(Course.CreatedAt)),
            string.Format("{0}.{1}", nameof(Course.Instructor), nameof(Course.Instructor.Id)),
            string.Format("{0}.{1}", nameof(Course.Instructor), nameof(Course.Instructor.FullName)),
            string.Format("{0}.{1}", nameof(Course.Instructor), nameof(Course.Instructor.CreatedAt)),
            string.Format("{0}.{1}.{2}", nameof(Course.Instructor), nameof(Instructor.Profile), nameof(Profile.Id)),
            string.Format("{0}.{1}.{2}", nameof(Course.Instructor), nameof(Instructor.Profile), nameof(Profile.InstructorId)),
            string.Format("{0}.{1}.{2}", nameof(Course.Instructor), nameof(Instructor.Profile), nameof(Profile.Bio)),
            string.Format("{0}.{1}.{2}", nameof(Course.Instructor), nameof(Instructor.Profile), nameof(Profile.LinkedInUrl)),
            string.Format("{0}.{1}.{2}", nameof(Course.Instructor), nameof(Instructor.Profile), nameof(Profile.CreatedAt)),
            string.Format("{0}.{1}", nameof(Course.Modules), nameof(Module.Id)),
            string.Format("{0}.{1}", nameof(Course.Modules), nameof(Module.CourseId)),
            string.Format("{0}.{1}", nameof(Course.Modules), nameof(Module.Name)),
            string.Format("{0}.{1}", nameof(Course.Modules), nameof(Module.CreatedAt)),
            string.Format("{0}.{1}.{2}", nameof(Course.Modules), nameof(Module.Lessons), nameof(Lesson.Id)),
            string.Format("{0}.{1}.{2}", nameof(Course.Modules), nameof(Module.Lessons), nameof(Lesson.ModuleId)),
            string.Format("{0}.{1}.{2}", nameof(Course.Modules), nameof(Module.Lessons), nameof(Lesson.Title)),
            string.Format("{0}.{1}.{2}", nameof(Course.Modules), nameof(Module.Lessons), nameof(Lesson.Duration)),
            string.Format("{0}.{1}.{2}", nameof(Course.Modules), nameof(Module.Lessons), nameof(Lesson.CreatedAt))
        ];
    }
}
