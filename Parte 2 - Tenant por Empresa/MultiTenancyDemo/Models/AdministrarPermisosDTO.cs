namespace MultiTenancyDemo.Models
{
    public class AdministrarPermisosDTO
    {
        public string UsuarioId { get; set; } = null!;
        public string? Email { get; set; }
        public List<PermisoUsuarioDTO> Permisos { get; set; } = 
            new List<PermisoUsuarioDTO>();
    }
}
