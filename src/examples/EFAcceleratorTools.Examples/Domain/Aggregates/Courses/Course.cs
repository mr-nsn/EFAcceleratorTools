using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Models;

namespace EFAcceleratorTools.Examples.Domain.Aggregates.Courses;

public class Course : Entity
{
    #region Simple Properties

    public long? InstructorId { get; set; }

    public string? Title { get; set; }

    #endregion

    #region Navigation Properties

    public Instructor? Instructor { get; set; }

    #region Collections

    public ICollection<Module>? Modules { get; set; }

    #endregion

    #endregion

    #region Constructors

    public Course()
    {
    
    }

    #endregion
}
