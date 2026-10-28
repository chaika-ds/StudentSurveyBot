using Microsoft.EntityFrameworkCore;
using StudentSurveyBot.Core.Entities;
using StudentSurveyBot.Data;

namespace StudentSurveyBot.UnitTests;

public sealed class StudentServiceTests
{
    [Fact]
    public async Task GetOrCreateAsync_DoesNotCreateDuplicateStudent()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var student = new Student
        {
            Id = Guid.NewGuid(),
            TgId = 111L,
            FullName = "Иванов Иван Иванович",
            Direction = "IT",
            Course = "3"
        };

        context.Students.Add(student);
        await context.SaveChangesAsync();

        var service = new StudentSurveyBot.Core.Services.StudentService(context);
        var created = await service.GetOrCreateAsync(111L);

        Assert.Equal(1, await context.Students.CountAsync(x => x.TgId == 111L));
        Assert.Equal(student.Id, created.Id);
    }
}
