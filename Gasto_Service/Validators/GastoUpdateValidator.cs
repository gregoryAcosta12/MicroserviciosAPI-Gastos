using FluentValidation;
using API_Gasto_Service.DTOs.Requests;

namespace API_Gasto_Service.Validators
{
    public class GastoUpdateValidator : AbstractValidator<GastoUpdateDTO>
    {
        public GastoUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID inválido");

            RuleFor(x => x.Monto)
                .GreaterThan(0).WithMessage("El monto debe ser mayor a 0")
                .LessThan(999999.99m).WithMessage("El monto no puede ser mayor a 999,999.99");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

            RuleFor(x => x.Fecha)
                .NotEmpty().WithMessage("La fecha es requerida")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("La fecha no puede ser futura");

            RuleFor(x => x.CategoriaId)
                .GreaterThan(0).WithMessage("La categoría es requerida");

            RuleFor(x => x.Estado)
                .Must(e => string.IsNullOrEmpty(e) || new[] { "Pendiente", "Pagado", "Cancelado" }.Contains(e))
                .WithMessage("Estado inválido. Valores permitidos: Pendiente, Pagado, Cancelado");
        }
    }
}