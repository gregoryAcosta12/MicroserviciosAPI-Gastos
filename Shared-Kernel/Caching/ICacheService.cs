namespace Shared.Kernel.Caching
{
    public interface ICacheService
    {
        /// <summary>
        /// Obtiene un valor del caché
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Guarda un valor en el caché
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Elimina un valor del caché
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Verifica si existe una clave en el caché
        /// </summary>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Elimina todas las claves que coinciden con un patrón
        /// </summary>
        Task RemoveByPatternAsync(string pattern);
    }
}