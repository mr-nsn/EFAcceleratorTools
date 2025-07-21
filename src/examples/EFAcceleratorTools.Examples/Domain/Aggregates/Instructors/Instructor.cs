using EFAcceleratorTools.Models;

namespace EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;

public class Instructor : Entity
{
    #region Simple Properties

    public string? FullName { get; set; }

    #endregion

    #region Navigation Properties

    public Profile? Profile { get; set; }

    #endregion

    #region Constructors

    public Instructor()
    {

    }

    #endregion
}
