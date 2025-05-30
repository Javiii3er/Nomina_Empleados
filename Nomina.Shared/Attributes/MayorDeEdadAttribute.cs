using System;
using System.ComponentModel.DataAnnotations;

namespace Nomina.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MayorDeEdadAttribute : ValidationAttribute
    {
        private readonly int _edadMinima;

        public MayorDeEdadAttribute(int edadMinima = 18)
        {
            _edadMinima = edadMinima;
            ErrorMessage = $"El empleado debe tener al menos {_edadMinima} años";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime fechaNacimiento)
            {
                return ValidationResult.Success;
            }

            var edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

            if (edad < _edadMinima)
            {
                return new ValidationResult(
                    ErrorMessage ?? $"El empleado debe tener al menos {_edadMinima} años",
                    memberNames: [validationContext.MemberName ?? validationContext.DisplayName]);
            }

            return ValidationResult.Success;
        }
    }
}