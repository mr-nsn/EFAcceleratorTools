using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Mappings.Aggregates.Courses;

public class LessonMap : EntityTypeConfiguration<Lesson>
{
    public override void Map(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("TB_LESSON");

        builder.HasKey(x => x.Id)
               .HasName("SQ_LESSON");

        builder.Property(x => x.Id)
               .HasColumnName("SQ_LESSON")
               .HasColumnType("bigint")
               .UseIdentityColumn();

        builder.Property(x => x.ModuleId)
               .HasColumnName("SQ_MODULE")
               .HasColumnType("bigint");

        builder.Property(x => x.Title)
               .HasColumnName("TX_TITLE")
               .HasColumnType("nvarchar(100)");

        builder.Property(x => x.Duration)
               .HasColumnName("DT_DURATION")
               .HasColumnType("time");

        builder.Property(x => x.CreatedAt)
               .HasColumnName("DT_CREATION")
               .HasColumnType("datetime");
    }
}
