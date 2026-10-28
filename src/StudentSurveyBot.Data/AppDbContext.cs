using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Consent> Consents => Set<Consent>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<StudentInterest> StudentInterests => Set<StudentInterest>();
    public DbSet<StudentPreference> StudentPreferences => Set<StudentPreference>();
    public DbSet<AdditionalInfo> AdditionalInfos => Set<AdditionalInfo>();
    public DbSet<Community> Communities => Set<Community>();
    public DbSet<CommunityJoinIntent> CommunityJoinIntents => Set<CommunityJoinIntent>();
    public DbSet<SurveySession> SurveySessions => Set<SurveySession>();
    public DbSet<ConsentAudit> ConsentAudits => Set<ConsentAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
