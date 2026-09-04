using System.ComponentModel.DataAnnotations;

namespace API_Categoria_Service.DTOs
{
    public class CategoriaUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }

        [MaxLength(50)]
        public string? Icono { get; set; }

        public bool Activo { get; set; } = true;
    }
}