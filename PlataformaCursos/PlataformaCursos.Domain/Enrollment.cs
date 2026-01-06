// ...new file...
namespace PlataformaCursos.Domain;

public class Enrollment
{
    public Guid CourseId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Course? Course { get; set; }
    public ApplicationUser? User { get; set; }
}

