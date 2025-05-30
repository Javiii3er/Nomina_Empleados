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
}