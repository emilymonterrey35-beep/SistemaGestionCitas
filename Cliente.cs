using System.ComponentModel.DataAnnotations;

namespace SistemaGestionCitas.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [Display(Name = "Correo")]
        public string Correo { get; set; }
    }
}