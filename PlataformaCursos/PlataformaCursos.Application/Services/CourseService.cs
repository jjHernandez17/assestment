using Microsoft.EntityFrameworkCore;
using PlataformaCursos.Application.DTOs;
using PlataformaCursos.Domain;
using PlataformaCursos.Infrastructure;

namespace PlataformaCursos.Application.Services;

public class CourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    // --- BÚSQUEDA Y PAGINACIÓN ---
    public async Task<(List<CourseDto> Items, int Total)> SearchCoursesAsync(string? query, string? status, int page, int pageSize)
    {
        var baseQuery = _context.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(query))
            baseQuery = baseQuery.Where(c => c.Title.Contains(query));

        if (!string.IsNullOrEmpty(status))
            baseQuery = baseQuery.Where(c => c.Status == status);

        var total = await baseQuery.CountAsync();
        var items = await baseQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseDto {
                Id = c.Id,
                Title = c.Title,
                Status = c.Status,
                UpdatedAt = c.UpdatedAt
            }).ToListAsync();

        return (items, total);
    }

    // --- CURSOS ---
    public async Task<Guid> CreateCourseAsync(string title)
    {
        var course = new Course { Title = title, Status = "Draft" };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course.Id;
    }

    public async Task<(bool Success, string Message)> PublishCourseAsync(Guid id)
    {
        var course = await _context.Courses
            .Include(c => c.Lessons.Where(l => !l.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return (false, "Course not found.");

        // REGLA: Al menos una lección activa
        if (!course.Lessons.Any())
            return (false, "Cannot publish a course without active lessons.");

        course.Status = "Published";
        course.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Course published successfully.");
    }

    public async Task<(bool Success, string Message)> UnpublishCourseAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return (false, "Course not found.");

        course.Status = "Draft";
        course.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return (true, "Course unpublished successfully.");
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            course.IsDeleted = true;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    // --- LECCIONES ---
    public async Task<(bool Success, string Message)> CreateLessonAsync(Guid courseId, string title)
    {
        // REGLA: Obtener el siguiente Order único
        var lastOrder = await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .MaxAsync(l => (int?)l.Order) ?? 0;

        var lesson = new Lesson {
            CourseId = courseId,
            Title = title,
            Order = lastOrder + 1
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        return (true, "Lesson created.");
    }

    public async Task<List<LessonDto>> GetLessonsByCourseAsync(Guid courseId)
    {
        return await _context.Lessons
            .Where(l => l.CourseId == courseId && !l.IsDeleted)
            .OrderBy(l => l.Order)
            .Select(l => new LessonDto {
                Id = l.Id,
                Title = l.Title,
                Order = l.Order
            }).ToListAsync();
    }

    public async Task ReorderLessonAsync(Guid lessonId, bool moveUp)
    {
        var lesson = await _context.Lessons.FindAsync(lessonId);
        if (lesson == null) return;

        var targetOrder = moveUp ? lesson.Order - 1 : lesson.Order + 1;
        if (targetOrder < 1) return;

        var swapLesson = await _context.Lessons
            .FirstOrDefaultAsync(l => l.CourseId == lesson.CourseId && l.Order == targetOrder);

        if (swapLesson != null)
        {
            swapLesson.Order = lesson.Order;
            lesson.Order = targetOrder;
            await _context.SaveChangesAsync();
        }
    }

    // --- SUMMARY ---
    public async Task<CourseSummaryDto?> GetSummaryAsync(Guid id)
    {
        var course = await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return null;

        return new CourseSummaryDto {
            Title = course.Title,
            Status = course.Status,
            TotalLessons = course.Lessons.Count(l => !l.IsDeleted),
            LastModified = course.UpdatedAt
        };
    }

    // --- ENROLLMENTS (USUARIOS QUE SE UNEN A CURSOS) ---
    public async Task<(bool Success, string Message)> JoinCourseAsync(Guid courseId, string userId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null) return (false, "Course not found.");
        if (course.Status != "Published") return (false, "Cannot join an unpublished course.");

        var existing = await _context.Enrollments.FindAsync(courseId, userId);
        if (existing != null) return (false, "User already enrolled in this course.");

        var enrollment = new Enrollment { CourseId = courseId, UserId = userId };
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return (true, "Enrolled successfully.");
    }

    public async Task<bool> IsUserEnrolledAsync(Guid courseId, string userId)
    {
        var existing = await _context.Enrollments.FindAsync(courseId, userId);
        return existing != null;
    }

    public async Task<List<string>> GetEnrollmentsByCourseAsync(Guid courseId)
    {
        return await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.UserId)
            .ToListAsync();
    }
}