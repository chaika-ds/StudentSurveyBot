using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TgId).IsUnique();
        builder.Property(x => x.FullName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Direction).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Course).HasMaxLength(50).IsRequired();

        builder.HasOne(x => x.Consent)
            .WithOne(x => x.Student)
            .HasForeignKey<Consent>(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Preference)
            .WithOne(x => x.Student)
            .HasForeignKey<StudentPreference>(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AdditionalInfo)
            .WithOne(x => x.Student)
            .HasForeignKey<AdditionalInfo>(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StudentInterests)
            .WithOne(x => x.Student)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
