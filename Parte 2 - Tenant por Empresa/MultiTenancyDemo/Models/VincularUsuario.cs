namespace MultiTenancyDemo.Models
{
    public class VincularUsuario
    {
        public Guid EmpresaId { get; set; }
        public string NombreEmpresa { get; set; } = null!;
        public string EmailUsuario { get; set; } = null!;
    }
}
