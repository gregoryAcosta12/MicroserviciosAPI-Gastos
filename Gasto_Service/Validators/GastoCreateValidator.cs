using FluentValidation;
using API_Gasto_Service.DTOs.Requests;

namespace API_Gasto_Service.Validators
{
    public class GastoCreateValidator : AbstractValidator<GastoCreateDTO>
    {
        public GastoCreateValidator()
        {
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

            RuleForEach(x => x.Detalles)
                .ChildRules(detalle =>
                {
                    detalle.RuleFor(d => d.Campo)
                        .NotEmpty().WithMessage("El campo es requerido")
                        .MaximumLength(100).WithMessage("El campo no puede exceder 100 caracteres");

                    detalle.RuleFor(d => d.Valor)
                        .NotEmpty().WithMessage("El valor es requerido")
                        .MaximumLength(500).WithMessage("El valor no puede exceder 500 caracteres");
                });
        }
    }
}