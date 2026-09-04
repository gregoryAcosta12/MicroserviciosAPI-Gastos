namespace Shared.Kernel.Common.DTOs
{
    public class BaseResponseDTO
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static BaseResponseDTO Ok(string message = "Operación exitosa")
        {
            return new BaseResponseDTO
            {
                Success = true,
                Message = message
            };
        }

        public static BaseResponseDTO Error(string message, List<string>? errors = null)
        {
            return new BaseResponseDTO
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class BaseResponseDTO<T> : BaseResponseDTO
    {
        public T? Data { get; set; }

        public static BaseResponseDTO<T> Ok(T data, string message = "Operación exitosa")
        {
            return new BaseResponseDTO<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public new static BaseResponseDTO<T> Error(string message, List<string>? errors = null)
        {
            return new BaseResponseDTO<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}