using EFAcceleratorTools.Interfaces;

namespace EFAcceleratorTools.Models;

/// <summary>
/// Represents a base entity with a unique identifier and creation timestamp.
/// Implements <see cref="IAggregateRoot"/> to indicate it is an aggregate root in the domain model.
/// </summary>
public abstract class Entity : IAggregateRoot
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class,
    /// setting <see cref="CreatedAt"/> to the current UTC date and time.
    /// </summary>
    public Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
