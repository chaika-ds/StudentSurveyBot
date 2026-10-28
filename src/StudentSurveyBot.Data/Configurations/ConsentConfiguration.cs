using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data.Configurations;

public sealed class ConsentConfiguration : IEntityTypeConfiguration<Consent>
{
    public void Configure(EntityTypeBuilder<Consent> builder)
    {
        builder.ToTable("Consents");
        builder.HasKey(x => x.StudentId);
        builder.Property(x => x.Version).HasMaxLength(32);
        builder.Property(x => x.TextHash).HasMaxLength(128);
        builder.Property(x => x.GivenAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamp with time zone");
    }
}
