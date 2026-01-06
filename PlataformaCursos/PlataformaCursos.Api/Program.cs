using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlataformaCursos.Infrastructure;
using PlataformaCursos.Domain; // Importante para ApplicationUser
using PlataformaCursos.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE BASE DE DATOS (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, 
        b => b.MigrationsAssembly("PlataformaCursos.Infrastructure")));

// 2. IDENTITY (Configurado para usar ApplicationUser y Roles)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.User.RequireUniqueEmail = true; // Email único obligatorio
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. CONFIGURACIÓN DE JWT
var jwtKey = builder.Configuration["JwtSettings:Key"] ?? "Clave_Super_Secreta_De_Respaldo_32_Caracteres_Minimo";
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// 4. SERVICIOS DE API, SWAGGER Y CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plataforma de Cursos API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", p => {
        p.AllowAnyOrigin() // Permite cualquier origen (importante para desarrollo)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// Registrar servicios de aplicación
builder.Services.AddScoped<CourseService>();

var app = builder.Build();

// --- MIGRACIONES AUTOMÁTICAS ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Esto revisa si hay migraciones pendientes y las aplica. 
        // Si la base de datos no existe, la crea.
        context.Database.Migrate(); 
        Console.WriteLine("--> Migraciones aplicadas con éxito.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Error al aplicar migraciones: {ex.Message}");
    }
}

// ... aquí sigue tu código de Seed de usuario (admin@test.com) ...

// --- SEED DE ROLES Y USUARIO DE PRUEBA ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Crear Roles si no existen
        string[] roleNames = { "Tutor", "Student" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Crear Usuario Admin de prueba si no existe
        if (!userManager.Users.Any())
        {
            var adminUser = new ApplicationUser 
            { 
                FirstName = "Admin",
                LastName = "Principal",
                UserName = "admin@test.com", 
                Email = "admin@test.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                // Asignar rol de Tutor por defecto al admin
                await userManager.AddToRoleAsync(adminUser, "Tutor");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error durante el Seed de la base de datos.");
    }
}

// 5. PIPELINE HTTP (MIDDLEWARES)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plataforma Cursos API V1");
    c.RoutePrefix = "swagger"; // Esto hace que la URL sea http://localhost:5276/swagger
});


app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();