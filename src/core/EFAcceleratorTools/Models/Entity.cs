using EFAcceleratorTools.Interfaces;

namespace EFAcceleratorTools.Models;

public class Entity : IAggregateRoot
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
