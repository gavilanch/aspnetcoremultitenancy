using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MultiTenancyDemo.Data;
using MultiTenancyDemo.Servicios;

namespace MultiTenancyDemo.Seguridad
{
    public class TienePermisoHandler: AuthorizationHandler<TienePermisoRequirement>
    {
        private readonly IServicioTenant servicioTenant;
        private readonly IServicioUsuario servicioUsuario;
        private readonly ApplicationDbContext dbContext;

        public TienePermisoHandler(IServicioTenant servicioTenant,
            IServicioUsuario servicioUsuario,
            ApplicationDbContext dbContext)
        {
            this.servicioTenant = servicioTenant;
            this.servicioUsuario = servicioUsuario;
            this.dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
        TienePermisoRequirement requirement)
        {
            var permiso = requirement.Permiso;
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var tenantId = new Guid(servicioTenant.ObtenerTenant());

            var tienePermiso = await dbContext.EmpresasUsuariosPermisos
                .AnyAsync(x => x.UsuarioId == usuarioId 
                && x.EmpresaId == tenantId && x.Permiso == permiso);

            if (tienePermiso)
            {
                context.Succeed(requirement);
            }
        }


    }
}
