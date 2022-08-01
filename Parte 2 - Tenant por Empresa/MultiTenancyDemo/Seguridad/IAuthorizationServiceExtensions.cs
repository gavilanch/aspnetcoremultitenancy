using Microsoft.AspNetCore.Authorization;
using MultiTenancyDemo.Entidades;
using MultiTenancyDemo.Servicios;
using System.Security.Claims;

namespace MultiTenancyDemo.Seguridad
{
    public static class IAuthorizationServiceExtensions
    {
        public static async Task<bool> 
            TienePermiso(this IAuthorizationService authorizationService, 
            ClaimsPrincipal user, Permisos permiso)
        {
            if (!user.Identity!.IsAuthenticated)
            {
                return false;
            }

            var nombrePolitica = $"{Constantes.PrefijoPolitica}{permiso}";
            var resultado = await authorizationService
                .AuthorizeAsync(user, nombrePolitica);
            return resultado.Succeeded;
        }

    }
}
