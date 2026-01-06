using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlataformaCursos.Domain; // Asegúrate de tener las entidades Course y Lesson en Domain

namespace PlataformaCursos.Infrastructure
{
    // Heredamos de IdentityDbContext para tener las tablas de usuarios listas
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>  
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Filtro Global para Soft Delete
            modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);

            // Regla: Orden único por curso
            modelBuilder.Entity<Lesson>()
                .HasIndex(l => new { l.CourseId, l.Order })
                .IsUnique();

            // Enrollment: clave primaria compuesta y relaciones
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.CourseId, e.UserId });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId);
        }
    }
}