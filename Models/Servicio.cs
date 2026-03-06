using System.ComponentModel.DataAnnotations;

namespace SistemaGestionCitas.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria")]
        [Display(Name = "Duración (minutos)")]
        public int DuracionMinutos { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio")]
        [Display(Name = "Costo")]
        public decimal Costo { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
