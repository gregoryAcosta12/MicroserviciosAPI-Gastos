namespace Shared.Kernel.Common.Enums
{
    public enum MetodoPago
    {
        Efectivo = 1,
        TarjetaCredito = 2,
        TarjetaDebito = 3,
        Transferencia = 4,
        PayPal = 5,
        Otro = 6
    }

    public static class MetodoPagoExtensions
    {
        public static string ToDisplayString(this MetodoPago metodo)
        {
            return metodo switch
            {
                MetodoPago.Efectivo => "Efectivo",
                MetodoPago.TarjetaCredito => "Tarjeta de Crédito",
                MetodoPago.TarjetaDebito => "Tarjeta de Débito",
                MetodoPago.Transferencia => "Transferencia Bancaria",
                MetodoPago.PayPal => "PayPal",
                MetodoPago.Otro => "Otro",
                _ => metodo.ToString()
            };
        }

        public static MetodoPago FromString(string metodo)
        {
            return metodo.ToLower() switch
            {
                "efectivo" => MetodoPago.Efectivo,
                "tarjetacredito" or "tarjeta credito" => MetodoPago.TarjetaCredito,
                "tarjetadebito" or "tarjeta debito" => MetodoPago.TarjetaDebito,
                "transferencia" => MetodoPago.Transferencia,
                "paypal" => MetodoPago.PayPal,
                _ => MetodoPago.Otro
            };
        }
    }
}