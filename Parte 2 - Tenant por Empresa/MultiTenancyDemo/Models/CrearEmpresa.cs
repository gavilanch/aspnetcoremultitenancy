using System.ComponentModel.DataAnnotations;

namespace MultiTenancyDemo.Models
{
    public class CrearEmpresa
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; } = null!;
    }
}
