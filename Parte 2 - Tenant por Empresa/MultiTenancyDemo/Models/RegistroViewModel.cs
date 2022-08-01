using System.ComponentModel.DataAnnotations;

namespace MultiTenancyDemo.Models
{
    public class RegistroViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }

}
