namespace Shared.Kernel.Common.Enums
{
    public enum TipoGasto
    {
        Fijo = 1,
        Variable = 2,
        Extraordinario = 3
    }

    public static class TipoGastoExtensions
    {
        public static string ToDisplayString(this TipoGasto tipo)
        {
            return tipo switch
            {
                TipoGasto.Fijo => "Fijo",
                TipoGasto.Variable => "Variable",
                TipoGasto.Extraordinario => "Extraordinario",
                _ => tipo.ToString()
            };
        }

        public static TipoGasto FromString(string tipo)
        {
            return tipo.ToLower() switch
            {
                "fijo" => TipoGasto.Fijo,
                "variable" => TipoGasto.Variable,
                "extraordinario" => TipoGasto.Extraordinario,
                _ => TipoGasto.Variable
            };
        }
    }
}