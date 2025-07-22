using EFAcceleratorTools.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFAcceleratorTools.Mapping
{
    /// <summary>
    /// Provides extension methods for registering entity type configurations with the Entity Framework Core <see cref="ModelBuilder"/>.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Adds a specific entity type configuration to the <see cref="ModelBuilder"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity being configured. Must implement <see cref="IAggregateRoot"/>.</typeparam>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance to configure.</param>
        /// <param name="configuration">The entity type configuration to apply.</param>
        public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder, EntityTypeConfiguration<TEntity> configuration) where TEntity : class, IAggregateRoot
        {
            configuration.Map(modelBuilder.Entity<TEntity>());
        }

        /// <summary>
        /// Registers multiple entity type configurations with the <see cref="ModelBuilder"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> instance to configure.</param>
        /// <param name="mappings">
        /// A dictionary mapping entity types to their corresponding <see cref="IEntityTypeConfiguration"/> implementations.
        /// </param>
        public static void RegisterModelsMapping(this ModelBuilder modelBuilder, Dictionary<Type, IEntityTypeConfiguration> mappings)
        {
            foreach (var mapping in mappings)
            {
                var entityType = mapping.Key;
                var genericMethod = typeof(ModelBuilderExtensions).GetMethod(nameof(ModelBuilderExtensions.AddConfiguration));
                var method = genericMethod!.MakeGenericMethod(entityType);
                method!.Invoke(null, new object[] { modelBuilder, mapping.Value });
            }
        }
    }
}
