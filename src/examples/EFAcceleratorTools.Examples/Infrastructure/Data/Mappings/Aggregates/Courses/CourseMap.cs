using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Mappings.Aggregates.Courses;

public class CourseMap : EntityTypeConfiguration<Course>
{
    public override void Map(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("TB_COURSE");

        builder.HasKey(x => x.Id)
               .HasName("SQ_COURSE");

        builder.Property(x => x.Id)
               .HasColumnName("SQ_COURSE")
               .HasColumnType("bigint")
               .UseIdentityColumn();

        builder.Property(x => x.InstructorId)
               .HasColumnName("SQ_INSTRUCTOR")
               .HasColumnType("bigint");

        builder.Property(x => x.Title)
               .HasColumnName("TX_TITLE")
               .HasColumnType("nvarchar(100)");

        builder.Property(x => x.CreatedAt)
               .HasColumnName("DT_CREATION")
               .HasColumnType("nvarchar(100)");

        builder.HasOne(x => x.Instructor)
               .WithMany()
               .HasForeignKey(x => x.InstructorId);

        builder.HasMany(x => x.Modules)
               .WithOne()
               .HasForeignKey(x => x.CourseId);
    }
}
