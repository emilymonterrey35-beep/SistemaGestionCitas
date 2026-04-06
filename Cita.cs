using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionCitas.Models
{
    public class Cita : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un servicio")]
        [Display(Name = "Servicio")]
        public int ServicioId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora")]
        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "Debe existir un usuario responsable")]
        [Display(Name = "Usuario Responsable")]
        public int UsuarioId { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Programada"; // Programada / Cancelada

        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        [ForeignKey("ServicioId")]
        public Servicio? Servicio { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var fechaHoraCita = Fecha.Date + Hora;

            if (fechaHoraCita < DateTime.Now)
            {
                yield return new ValidationResult(
                    "No se pueden registrar citas en fechas u horas pasadas.",
                    new[] { nameof(Fecha), nameof(Hora) });
            }

            if (Estado != "Programada" && Estado != "Cancelada")
            {
                yield return new ValidationResult(
                    "El estado de la cita debe ser Programada o Cancelada.",
                    new[] { nameof(Estado) });
            }
        }
    }
}