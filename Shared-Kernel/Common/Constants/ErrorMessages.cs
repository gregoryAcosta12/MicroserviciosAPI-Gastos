namespace Shared_Kernel.Common.Constants
{
    public static class ErrorMessages
    {
        // ========================================
        // ERRORES GENERALES
        // ========================================
        public const string InternalServerError = "Ocurrió un error interno en el servidor";
        public const string BadRequest = "Solicitud inválida";
        public const string NotFound = "Recurso no encontrado";
        public const string Unauthorized = "No autorizado";
        public const string Forbidden = "Acceso denegado";

        // ========================================
        // ERRORES DE GASTOS
        // ========================================
        public const string GastoNotFound = "Gasto no encontrado";
        public const string GastoCreateError = "Error al crear el gasto";
        public const string GastoUpdateError = "Error al actualizar el gasto";
        public const string GastoDeleteError = "Error al eliminar el gasto";
        public const string GastoInvalidMonto = "El monto debe ser mayor a 0";
        public const string GastoInvalidFecha = "La fecha no puede ser futura";

        // ========================================
        // ERRORES DE CATEGORÍAS
        // ========================================
        public const string CategoriaNotFound = "Categoría no encontrada";
        public const string CategoriaExists = "Ya existe una categoría con ese nombre";
        public const string CategoriaHasGastos = "No se puede eliminar la categoría porque tiene gastos asociados";
        public const string CategoriaCreateError = "Error al crear la categoría";
        public const string CategoriaUpdateError = "Error al actualizar la categoría";
        public const string CategoriaDeleteError = "Error al eliminar la categoría";

        // ========================================
        // ERRORES DE USUARIOS
        // ========================================
        public const string UserNotFound = "Usuario no encontrado";
        public const string UserExists = "El email ya está registrado";
        public const string UserInvalidCredentials = "Credenciales inválidas";
        public const string UserInactive = "Usuario inactivo";
        public const string UserCreateError = "Error al crear el usuario";
        public const string UserUpdateError = "Error al actualizar el usuario";
        public const string UserDeleteError = "Error al eliminar el usuario";

        // ========================================
        // ERRORES DE VALIDACIÓN
        // ========================================
        public const string ValidationError = "Error de validación";
        public const string RequiredField = "Campo requerido";
        public const string InvalidEmail = "Email inválido";
        public const string InvalidPassword = "La contraseña debe tener al menos 6 caracteres";
        public const string InvalidPhone = "Teléfono inválido";
        public const string MaxLengthExceeded = "El campo excede la longitud máxima permitida";

        // ========================================
        // ERRORES DE BASE DE DATOS
        // ========================================
        public const string DbConnectionError = "Error de conexión a la base de datos";
        public const string DbSaveError = "Error al guardar los cambios";
        public const string DbConcurrencyError = "Error de concurrencia en la base de datos";

        // ========================================
        // ERRORES DE RABBITMQ
        // ========================================
        public const string RabbitMQConnectionError = "Error de conexión a RabbitMQ";
        public const string RabbitMQPublishError = "Error al publicar el mensaje";
        public const string RabbitMQConsumeError = "Error al consumir el mensaje";
        public const string RabbitMQQueueNotFound = "Cola no encontrada";

        // ========================================
        // ERRORES DE REDIS
        // ========================================
        public const string RedisConnectionError = "Error de conexión a Redis";
        public const string RedisCacheError = "Error al operar con el caché";
    }
}