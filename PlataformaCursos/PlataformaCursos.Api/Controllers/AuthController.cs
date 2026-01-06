using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlataformaCursos.Application.DTOs;
using PlataformaCursos.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlataformaCursos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        // 1. Validar que el rol sea permitido
        if (model.Role != "Tutor" && model.Role != "Student")
        {
            return BadRequest(new { message = "Invalid role. Must be 'Tutor' or 'Student'." });
        }

        // 2. Verificar si el correo ya existe
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null) 
        {
            return BadRequest(new { message = "Email already registered." });
        }

        // 3. Crear la instancia del usuario con los nuevos campos
        var user = new ApplicationUser 
        { 
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.Email, 
            Email = model.Email 
        };

        // 4. Guardar el usuario
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) 
        {
            return BadRequest(result.Errors);
        }

        // 5. Asignar el rol al usuario
        await _userManager.AddToRoleAsync(user, model.Role);

        return Ok(new { message = $"User registered successfully as {model.Role}." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            // Obtener los roles del usuario para incluirlos en el Token
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Añadir los roles a los claims
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Generar la llave desde la configuración
            var jwtKey = _configuration["JwtSettings:Key"] ?? "Clave_Super_Secreta_De_Respaldo_32_Caracteres";
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            // Crear el objeto del Token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user = new {
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    Roles = userRoles
                }
            });
        }

        return Unauthorized(new { message = "Invalid email or password." });
    }
}