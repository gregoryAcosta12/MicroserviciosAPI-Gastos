namespace API_Gateway.Interfaces
{
    public interface IGatewayService
    {
        /// <summary>
        /// Obtiene el estado de todos los servicios
        /// </summary>
        Task<Dictionary<string, string>> GetServicesStatusAsync();

        /// <summary>
        /// Obtiene las rutas configuradas
        /// </summary>
        object GetConfiguredRoutes();

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        Task<bool> ValidateTokenAsync(string token);

        /// <summary>
        /// Obtiene información del usuario desde el token
        /// </summary>
        int? GetUserIdFromToken(string token);
    }
}