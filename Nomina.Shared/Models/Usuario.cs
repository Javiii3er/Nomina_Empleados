using System.ComponentModel.DataAnnotations;

namespace Nomina.Shared.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } = "Empleado";

        public int? EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }
    }
}