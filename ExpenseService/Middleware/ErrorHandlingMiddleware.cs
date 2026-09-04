using System.Text;
using System.Text.Json;

namespace API_Gateway.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log de la petición entrante
            var request = context.Request;
            var requestBody = await ReadRequestBodyAsync(request);
            var requestInfo = new
            {
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = requestBody,
                IP = context.Connection.RemoteIpAddress?.ToString(),
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation($"📥 REQUEST: {JsonSerializer.Serialize(requestInfo)}");

            // Capturar la respuesta
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                // Log de la respuesta
                var responseBodyText = await ReadResponseBodyAsync(responseBody);
                var responseInfo = new
                {
                    StatusCode = context.Response.StatusCode,
                    Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    Body = responseBodyText,
                    Timestamp = DateTime.UtcNow
                };

                _logger.LogInformation($"📤 RESPONSE: {JsonSerializer.Serialize(responseInfo)}");

                // Copiar respuesta al stream original
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private async Task<string> ReadResponseBodyAsync(MemoryStream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBody, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}