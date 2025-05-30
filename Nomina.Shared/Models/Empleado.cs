using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nomina.Shared.Models;

public class Empleado
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal Salario { get; set; }

    [Required]
    [StringLength(50)]
    public string Cargo { get; set; } = string.Empty;

    [StringLength(50)]
    public string Departamento { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string Telefono { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }
}
