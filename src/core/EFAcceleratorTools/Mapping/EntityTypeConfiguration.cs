using EFAcceleratorTools.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFAcceleratorTools.Mapping;

public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration where TEntity : class, IAggregateRoot
{
    public abstract void Map(EntityTypeBuilder<TEntity> builder);
}
