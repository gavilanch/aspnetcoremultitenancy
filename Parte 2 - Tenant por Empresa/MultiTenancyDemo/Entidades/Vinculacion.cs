using Microsoft.AspNetCore.Identity;

namespace MultiTenancyDemo.Entidades
{
    public class Vinculacion: IEntidadComún
    {
        public int Id { get; set; }
        public Guid EmpresaId { get; set; }
        public string UsuarioId { get; set; } = null!;
        public VinculacionEstatus Estatus { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Empresa Empresa { get; set; } = null!;
        public IdentityUser Usuario { get; set; } = null!;

    }
}
