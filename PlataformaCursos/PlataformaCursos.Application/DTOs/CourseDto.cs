namespace PlataformaCursos.Application.DTOs;

public class CourseDto {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft | Published
    public DateTime UpdatedAt { get; set; }
}

public class CourseSummaryDto {
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public DateTime LastModified { get; set; }
}