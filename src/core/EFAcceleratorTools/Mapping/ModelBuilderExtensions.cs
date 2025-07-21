using EFAcceleratorTools.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EFAcceleratorTools.Mapping
{
    public static class ModelBuilderExtensions
    {
        public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder, EntityTypeConfiguration<TEntity> configuration) where TEntity : class, IAggregateRoot
        {
            configuration.Map(modelBuilder.Entity<TEntity>());
        }

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
