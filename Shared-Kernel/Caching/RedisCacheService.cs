using System.Text.Json;
using StackExchange.Redis;

namespace Shared.Kernel.Caching
{
    public class RedisCacheService : ICacheService, IDisposable
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private bool _disposed;

        public RedisCacheService(string connectionString, ILogger<RedisCacheService>? logger = null)
        {
            _logger = logger ?? new LoggerFactory().CreateLogger<RedisCacheService>();

            try
            {
                _connection = ConnectionMultiplexer.Connect(connectionString);
                _database = _connection.GetDatabase();
                _logger.LogInformation("✅ Conexión a Redis establecida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al conectar con Redis");
                throw;
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener cache: {key}");
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, json, expiration ?? TimeSpan.FromMinutes(30));
                _logger.LogInformation($"✅ Cache guardado: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al guardar cache: {key}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
                _logger.LogInformation($"🗑️ Cache eliminado: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar cache: {key}");
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar cache: {key}");
                return false;
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var server = _connection.GetServer(_connection.GetEndPoints().First());
                var keys = server.Keys(pattern: $"*{pattern}*").ToArray();

                if (keys.Any())
                {
                    await _database.KeyDeleteAsync(keys);
                    _logger.LogInformation($"🗑️ Caches eliminados por patrón: {pattern} ({keys.Length} elementos)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar cache por patrón: {pattern}");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _connection?.Close();
            _connection?.Dispose();

            _disposed = true;
            _logger.LogInformation("🔌 Conexión a Redis cerrada");
        }
    }
}