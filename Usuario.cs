using System.ComponentModel.DataAnnotations;

namespace SistemaGestionCitas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [StringLength(100)]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(200)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [RegularExpression("Administrador|Usuario", ErrorMessage = "Rol inválido")]
        [StringLength(50)]
        [Display(Name = "Rol")]
        public string Rol { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public ICollection<Cita>? Citas { get; set; }
    }
}