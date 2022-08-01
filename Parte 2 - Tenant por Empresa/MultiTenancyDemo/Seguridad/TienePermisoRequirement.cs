using Microsoft.AspNetCore.Authorization;
using MultiTenancyDemo.Entidades;

namespace MultiTenancyDemo.Seguridad
{
    public class TienePermisoRequirement: IAuthorizationRequirement
    {
        public TienePermisoRequirement(Permisos permisos)
        {
            Permiso = permisos;
        }

        public Permisos Permiso { get; }
    }
}
