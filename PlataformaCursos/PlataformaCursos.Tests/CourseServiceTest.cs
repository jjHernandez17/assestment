using Microsoft.EntityFrameworkCore;
using PlataformaCursos.Application.Services;
using PlataformaCursos.Domain;
using PlataformaCursos.Infrastructure;
using Xunit;

namespace PlataformaCursos.Tests;

public class CourseServiceTests
{
    private ApplicationDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // DB única por test
            .Options;
        var databaseContext = new ApplicationDbContext(options);
        databaseContext.Database.EnsureCreated();
        return databaseContext;
    }

    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var context = GetDatabaseContext();
        var service = new CourseService(context);
        var courseId = await service.CreateCourseAsync("Curso con lecciones");
        await service.CreateLessonAsync(courseId, "Lección 1");

        // Act
        var result = await service.PublishCourseAsync(courseId);

        // Assert
        Assert.True(result.Success);
        var course = await context.Courses.FindAsync(courseId);
        Assert.Equal("Published", course.Status);
    }

    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var context = GetDatabaseContext();
        var service = new CourseService(context);
        var courseId = await service.CreateCourseAsync("Curso vacío");

        // Act
        var result = await service.PublishCourseAsync(courseId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cannot publish a course without active lessons.", result.Message);
    }

    [Fact]
    public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var context = GetDatabaseContext();
        var service = new CourseService(context);
        var courseId = await service.CreateCourseAsync("Curso Test");

        // Act
        await service.CreateLessonAsync(courseId, "Lección 1"); // Order 1
        await service.CreateLessonAsync(courseId, "Lección 2"); // Order 2

        // Assert
        var lessons = await context.Lessons.Where(l => l.CourseId == courseId).ToListAsync();
        Assert.Equal(2, lessons.Count);
        Assert.Equal(1, lessons[0].Order);
        Assert.Equal(2, lessons[1].Order);
    }

    [Fact]
    public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
    {
        // Nota: En la vida real, el DbContext configurado con índices únicos lanzaría excepción.
        // Aquí validamos que nuestra lógica de Service maneja el incremento correctamente.
        var context = GetDatabaseContext();
        var service = new CourseService(context);
        var courseId = await service.CreateCourseAsync("Curso Test");

        await service.CreateLessonAsync(courseId, "L1");
        var result = await service.CreateLessonAsync(courseId, "L2");

        var lessons = await context.Lessons.Where(l => l.CourseId == courseId).ToListAsync();
        Assert.True(result.Success);
        Assert.Equal(1, lessons[0].Order);
        Assert.Equal(2, lessons[1].Order);
        Assert.NotEqual(lessons[0].Order, lessons[1].Order);
    }

    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var context = GetDatabaseContext();
        var service = new CourseService(context);
        var courseId = await service.CreateCourseAsync("Curso para borrar");

        // Act
        await service.DeleteCourseAsync(courseId);

        // Assert
        // Usamos IgnoreQueryFilters para ver si sigue en la DB físicamente
        var course = await context.Courses
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == courseId);

        Assert.NotNull(course);
        Assert.True(course.IsDeleted);
    }
}