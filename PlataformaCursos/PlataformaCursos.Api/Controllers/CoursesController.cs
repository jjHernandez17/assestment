using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursos.Application.DTOs;
using PlataformaCursos.Application.Services;
using System.Security.Claims;

namespace PlataformaCursos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Todos los endpoints requieren Token JWT según el requerimiento
public class CoursesController : ControllerBase
{
    private readonly CourseService _courseService;

    public CoursesController(CourseService courseService)
    {
        _courseService = courseService;
    }

    // GET /api/courses/search?q=&status=&page=&pageSize=
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q, 
        [FromQuery] string? status, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var result = await _courseService.SearchCoursesAsync(q, status, page, pageSize);
        return Ok(new { 
            items = result.Items, 
            total = result.Total, 
            page, 
            pageSize 
        });
    }

    // GET /api/courses/{id}/summary
    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        var summary = await _courseService.GetSummaryAsync(id);
        if (summary == null) return NotFound(new { message = "Course not found" });
        return Ok(summary);
    }

    // POST /api/courses
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title)) 
            return BadRequest("Title is required");

        var courseId = await _courseService.CreateCourseAsync(request.Title);
        return CreatedAtAction(nameof(GetSummary), new { id = courseId }, new { id = courseId, title = request.Title });
    }

    // PATCH /api/courses/{id}/publish
    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result = await _courseService.PublishCourseAsync(id);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message });
    }

    // PATCH /api/courses/{id}/unpublish
    [HttpPatch("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        var result = await _courseService.UnpublishCourseAsync(id);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message });
    }

    // DELETE /api/courses/{id} (Soft Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _courseService.DeleteCourseAsync(id);
        return Ok(new { message = "Course deleted logically (soft delete)." });
    }

    // POST /api/courses/{id}/join  --> Nuevo endpoint
    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinCourse(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _courseService.JoinCourseAsync(id, userId);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message });
    }

    // GET /api/courses/{id}/students  --> Opcional: listar usuarios inscritos (IDs)
    [HttpGet("{id}/students")]
    public async Task<IActionResult> GetStudents(Guid id)
    {
        var students = await _courseService.GetEnrollmentsByCourseAsync(id);
        return Ok(students);
    }

    // --- ENDPOINTS DE LECCIONES ---

    // GET /api/courses/{id}/lessons
    [HttpGet("{id}/lessons")]
    public async Task<IActionResult> GetLessons(Guid id)
    {
        var lessons = await _courseService.GetLessonsByCourseAsync(id);
        return Ok(lessons);
    }

    // POST /api/courses/{id}/lessons
    [HttpPost("{id}/lessons")]
    public async Task<IActionResult> AddLesson(Guid id, [FromBody] CreateLessonRequest request)
    {
        var result = await _courseService.CreateLessonAsync(id, request.Title);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = "Lesson added successfully." });
    }

    // PATCH /api/courses/lessons/{lessonId}/reorder?moveUp=true
    [HttpPatch("lessons/{lessonId}/reorder")]
    public async Task<IActionResult> Reorder(Guid lessonId, [FromQuery] bool moveUp)
    {
        await _courseService.ReorderLessonAsync(lessonId, moveUp);
        return Ok(new { message = "Order updated." });
    }
}

// DTOs simples para los cuerpos de las peticiones (puedes moverlos a la capa Application.DTOs)
public class CreateCourseRequest { public string Title { get; set; } = string.Empty; }
public class CreateLessonRequest { public string Title { get; set; } = string.Empty; }