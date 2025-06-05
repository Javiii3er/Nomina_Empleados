using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nomina.API.Data;
using Microsoft.EntityFrameworkCore;
using Nomina.Shared.Models;
using BCrypt.Net;

namespace Nomina.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration config, AppDbContext context) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly AppDbContext _context = context;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username)
                || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Credenciales inválidas");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash))
                return Unauthorized("Credenciales inválidas");

            var token = GenerateJwtToken(usuario);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Usuario o contraseña vacíos");

            var exists = await _context.Usuarios.AnyAsync(u => u.Username == dto.Username);
            if (exists)
                return BadRequest("El usuario ya existe");

            var nuevoUsuario = new Usuario
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User" // Valor por defecto
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Usuario registrado exitosamente",
                Username = nuevoUsuario.Username
            });
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JWT:Key"] ?? throw new Exception("JWT Key no configurada")));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToInt32(_config["JWT:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Endpoint temporal - ELIMINAR después de usar en producción
        [HttpPost("crear-admin-temp")]
        public async Task<IActionResult> CrearAdminTemporal()
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == "admin"))
                return BadRequest("El usuario admin ya existe");

            var admin = new Usuario
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                Role = "Admin"
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Usuario admin creado (ELIMINAR ESTE ENDPOINT DESPUÉS DE USAR)",
                Username = admin.Username,
                Password = "Admin123*"
            });
        }
    }

    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

}