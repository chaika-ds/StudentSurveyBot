using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentSurveyBot.Core.Entities;

public sealed class AdditionalInfoConfiguration
    : IEntityTypeConfiguration<AdditionalInfo>
{
    public void Configure(EntityTypeBuilder<AdditionalInfo> builder)
    {
        builder.ToTable("AdditionalInfos");
        builder.HasKey(x => x.StudentId);

        builder.Property(x => x.SpecialWishes)
            .HasMaxLength(2000);

        builder.Property(x => x.ContactTelegramNick)
            .HasMaxLength(100);
    }
}