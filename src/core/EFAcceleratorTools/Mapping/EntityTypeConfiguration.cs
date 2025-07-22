using EFAcceleratorTools.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFAcceleratorTools.Mapping;

/// <summary>
/// Abstract base class for configuring entity types in the Entity Framework Core model.
/// Implement this class to define mapping and configuration for a specific aggregate root entity.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity being configured. Must be a class implementing <see cref="IAggregateRoot"/>.
/// </typeparam>
public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration where TEntity : class, IAggregateRoot
{
    /// <summary>
    /// Configures the entity type using the provided <see cref="EntityTypeBuilder{TEntity}"/>.
    /// Implement this method to define property mappings, relationships, and constraints.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public abstract void Map(EntityTypeBuilder<TEntity> builder);
}
