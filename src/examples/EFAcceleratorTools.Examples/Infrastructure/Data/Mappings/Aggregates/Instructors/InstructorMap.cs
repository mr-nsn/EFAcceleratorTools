using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Mappings.Aggregates.Instructors;

public class InstructorMap : EntityTypeConfiguration<Instructor>
{
    public override void Map(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("TB_INSTRUCTOR");

        builder.HasKey(x => x.Id)
               .HasName("SQ_INSTRUCTOR");

        builder.Property(x => x.Id)
               .HasColumnName("SQ_INSTRUCTOR")
               .HasColumnType("bigint")
               .UseIdentityColumn();

        builder.Property(x => x.FullName)
               .HasColumnName("TX_FULL_NAME")
               .HasColumnType("nvarchar(100)");

        builder.Property(x => x.CreatedAt)
               .HasColumnName("DT_CREATION")
               .HasColumnType("datetime");

        builder.HasOne(x => x.Profile)
               .WithOne()
               .HasForeignKey<Profile>(x => x.InstructorId);
    }
}

