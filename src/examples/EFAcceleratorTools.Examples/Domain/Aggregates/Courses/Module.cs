using EFAcceleratorTools.Models;

namespace EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
public class Module : Entity
{
    #region Simple Properties

    public long? CourseId { get; set; }

    public string? Name { get; set; }

    #endregion

    #region Navigation Properties

    #region Collections

    public ICollection<Lesson>? Lessons { get; set; }

    #endregion

    #endregion

    #region Constructors

    public Module()
    {

    }

    #endregion
}
