namespace Shared.Kernel.Common.Enums
{
    public enum EstadoGasto
    {
        Pendiente = 1,
        Pagado = 2,
        Cancelado = 3
    }

    public static class EstadoGastoExtensions
    {
        public static string ToDisplayString(this EstadoGasto estado)
        {
            return estado switch
            {
                EstadoGasto.Pendiente => "Pendiente",
                EstadoGasto.Pagado => "Pagado",
                EstadoGasto.Cancelado => "Cancelado",
                _ => estado.ToString()
            };
        }

        public static EstadoGasto FromString(string estado)
        {
            return estado.ToLower() switch
            {
                "pendiente" => EstadoGasto.Pendiente,
                "pagado" => EstadoGasto.Pagado,
                "cancelado" => EstadoGasto.Cancelado,
                _ => EstadoGasto.Pendiente
            };
        }
    }
}