using System.ComponentModel.DataAnnotations;

namespace SistemaGestionCitas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Correo { get; set; }

        [Required]
        public string Contrasena { get; set; }

        [Required]
        public string Rol { get; set; }

        public bool Activo { get; set; } = true;
    }
}
