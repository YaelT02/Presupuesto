using Microsoft.AspNetCore.Mvc;
using Presupuesto.Validations;
using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Models
{
    public class TipoCuenta : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo (0) es requerido")]
        [StringLength(maximumLength:50,MinimumLength = 3,ErrorMessage = "la lonfgitud del campo (0) debe estar entre (2) y (1)")]
        [Display(Name = "Nombre de el tipo de la cuenta")]
        //[ValidarMayuscula]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        [Required(ErrorMessage = "El campo debe ser un correo electronico valido")]
        [EmailAddress(ErrorMessage ="")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Nombre != null && Nombre.Length > 0)
            {
                var firstLatter = Nombre[0].ToString();
                
                if (firstLatter != firstLatter.ToUpper())
                {
                    yield return new ValidationResult("La primer letra debe ser mayúscula",
                        new[] { nameof(Nombre) });
                }
            }
        }
    }
}
