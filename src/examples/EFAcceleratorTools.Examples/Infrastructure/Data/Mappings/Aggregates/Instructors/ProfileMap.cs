using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFAcceleratorTools.Examples.Infrastructure.Data.Mappings.Aggregates.Instructors;

public class ProfileMap : EntityTypeConfiguration<Profile>
{
    public override void Map(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("TB_PROFILE");

        builder.HasKey(x => x.Id)
               .HasName("SQ_PROFILE");

        builder.Property(x => x.Id)
               .HasColumnName("SQ_PROFILE")
               .HasColumnType("bigint")
               .UseIdentityColumn();

        builder.Property(x => x.InstructorId)
               .HasColumnName("SQ_INSTRUCTOR")
               .HasColumnType("bigint");

        builder.Property(x => x.Bio)
               .HasColumnName("TX_BIO")
               .HasColumnType("nvarchar(500)");

        builder.Property(x => x.LinkedInUrl)
               .HasColumnName("TX_LINKEDIN")
               .HasColumnType("nvarchar(250)");

        builder.Property(x => x.CreatedAt)
               .HasColumnName("DT_CREATION")
               .HasColumnType("datetime");
    }
}
