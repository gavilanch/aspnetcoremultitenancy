using Microsoft.AspNetCore.Identity;

namespace MultiTenancyDemo.Entidades
{
    public class Empresa : IEntidadComún
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? UsuarioCreacionId { get; set; }
        public IdentityUser UsuarioCreacion { get; set; } = null!;
        public List<EmpresaUsuarioPermiso> EmpresaUsuariosPermisos { get; set; } = null!;
    }
}
