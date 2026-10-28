using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data.Configurations;

public sealed class StudentPreferenceConfiguration : IEntityTypeConfiguration<StudentPreference>
{
    public void Configure(EntityTypeBuilder<StudentPreference> builder)
    {
        builder.ToTable("StudentPreferences");
        builder.HasKey(x => x.StudentId);

        builder.Property(x => x.DesiredVibe).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.ContactStyle).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.GroupSize).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.VisitFrequency).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.PreferredTime).HasConversion<string>().HasMaxLength(256);
        builder.Property(x => x.Budget).HasConversion<string>().HasMaxLength(64);
    }
}
