using Microsoft.AspNetCore.Identity;

namespace MultiTenancyDemo.Entidades
{
    public class EmpresaUsuarioPermiso: IEntidadComún
    {
        public string UsuarioId { get; set; } = null!;
        public Guid EmpresaId { get; set; }
        public Permisos Permiso { get; set; }
        public IdentityUser? Usuario { get; set; }
        public Empresa? Empresa { get; set; }

    }
}
