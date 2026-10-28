using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data.Configurations;

public sealed class CommunityConfiguration : IEntityTypeConfiguration<Community>
{
    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.ToTable("Communities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(600).IsRequired();
        builder.Property(x => x.TelegramUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.InterestCodes).HasColumnType("text[]").IsRequired();
    }
}
