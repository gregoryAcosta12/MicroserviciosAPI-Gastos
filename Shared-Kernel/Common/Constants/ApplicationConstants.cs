namespace Shared_Kernel.Common.Constants
{
    public static class ApplicationConstants
    {
        // ========================================
        // NOMBRES DE SERVICIOS
        // ========================================
        public const string GatewayService = "API-Gateway";
        public const string GastoService = "Gasto-Service";
        public const string CategoriaService = "Categoria-Service";
        public const string UsuarioService = "Usuario-Service";

        // ========================================
        // NOMBRES DE COLAS RABBITMQ
        // ========================================
        public const string QueueGastoCreado = "gasto-creado-queue";
        public const string QueueGastoActualizado = "gasto-actualizado-queue";
        public const string QueueGastoEliminado = "gasto-eliminado-queue";
        public const string QueueCategoriaCreada = "categoria-creada-queue";
        public const string QueueCategoriaActualizada = "categoria-actualizada-queue";
        public const string QueueCategoriaEliminada = "categoria-eliminada-queue";
        public const string QueueUsuarioCreado = "usuario-creado-queue";

        // ========================================
        // NOMBRES DE EXCHANGES
        // ========================================
        public const string ExchangeGastos = "gastos-exchange";
        public const string ExchangeCategorias = "categorias-exchange";
        public const string ExchangeUsuarios = "usuarios-exchange";

        // ========================================
        // RUTAS DE API
        // ========================================
        public const string ApiGastos = "/api/gastos";
        public const string ApiCategorias = "/api/categorias";
        public const string ApiUsuarios = "/api/usuarios";
        public const string ApiAuth = "/api/auth";

        // ========================================
        // CONFIGURACIONES
        // ========================================
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;
        public const string DefaultDateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public const string DefaultCurrency = "USD";

        // ========================================
        // ROLES
        // ========================================
        public const string RoleAdmin = "Administrador";
        public const string RoleUser = "Usuario";
    }
}