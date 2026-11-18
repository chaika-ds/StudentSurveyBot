using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data.Configurations;

public sealed class StudentInterestConfiguration
    : IEntityTypeConfiguration<StudentInterest>
{
    public void Configure(EntityTypeBuilder<StudentInterest> builder)
    {
        builder.ToTable("StudentInterests");

        builder.HasKey(x => new
        {
            x.StudentId,
            x.InterestId
        });

        builder.HasOne(x => x.Student)
            .WithMany(x => x.StudentInterests)
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Interest)
            .WithMany(x => x.StudentInterests)
            .HasForeignKey(x => x.InterestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}