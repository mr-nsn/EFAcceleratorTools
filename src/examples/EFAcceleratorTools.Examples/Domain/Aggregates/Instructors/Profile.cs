using EFAcceleratorTools.Models;

namespace EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;

public class Profile : Entity
{
    #region Simple Properties

    public long? InstructorId { get; set; }
    public string? Bio { get; set; }
    public string? LinkedInUrl { get; set; }

    #endregion

    #region Constructors

    public Profile()
    {

    }

    #endregion
}
