namespace MultiTenancyDemo.Models
{
    public class IndexPermisosDTO
    {
        public string NombreEmpresa { get; set; } = null!;
        public IEnumerable<UsuarioDTO> Empleados { get; set; } = null!;
    }
}
