using Microsoft.AspNetCore.Identity;

namespace PlataformaCursos.Domain;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    // El Email ya viene incluido en IdentityUser

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}