using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nomina.Shared.Attributes;

namespace Nomina.Shared.Models;

public class Empleado
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    [Display(Name = "Nombre Completo")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El salario es obligatorio")]
    [Column(TypeName = "decimal(12,2)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El salario debe ser mayor a 0")]
    [Display(Name = "Salario")]
    public decimal Salario { get; set; }

    [Required(ErrorMessage = "El cargo es obligatorio")]
    [StringLength(50, ErrorMessage = "El cargo no puede exceder los 50 caracteres")]
    public string Cargo { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El departamento no puede exceder los 50 caracteres")]
    [Display(Name = "Departamento")]
    public string Departamento { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [Required(ErrorMessage = "El correo electrónico es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Formato de correo inválido (ejemplo: usuario@dominio.com)")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
    [StringLength(15, ErrorMessage = "El teléfono no puede exceder los 15 caracteres")]
    [RegularExpression(@"^(\+)?[0-9\s-]*$",
        ErrorMessage = "Solo números, espacios, guiones y signo +")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
    [Display(Name = "Dirección")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Nacimiento")]
    [Range(typeof(DateTime), "1/1/1900", "1/1/2100", ErrorMessage = "Ingrese una fecha entre 1900 y 2100")]
    [MayorDeEdad(18, ErrorMessage = "El empleado debe ser mayor de edad (mínimo 18 años)")]
    public DateTime FechaNacimiento { get; set; }
}