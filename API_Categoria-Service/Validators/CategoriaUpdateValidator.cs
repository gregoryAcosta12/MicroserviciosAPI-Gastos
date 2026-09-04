using FluentValidation;
using API_Categoria_Service.DTOs;

namespace API_Categoria_Service.Validators
{
    public class CategoriaUpdateValidator : AbstractValidator<CategoriaUpdateDTO>
    {
        public CategoriaUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID inválido");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.Color)
                .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
                .When(x => !string.IsNullOrEmpty(x.Color))
                .WithMessage("El color debe ser un código hexadecimal válido (ej: #007bff)");

            RuleFor(x => x.Icono)
                .MaximumLength(50).WithMessage("El icono no puede exceder 50 caracteres");
        }
    }
}