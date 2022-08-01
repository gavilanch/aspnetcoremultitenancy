using System.Security.Claims;

namespace MultiTenancyDemo.Servicios
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly HttpContext httpContext;

        public ServicioUsuario(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }


        public string ObtenerUsuarioId()
        {
            if (httpContext.User.Identity!.IsAuthenticated)
            {
                var idClaim = httpContext.User
                        .Claims.Where(x => x.Type == ClaimTypes.NameIdentifier)
                        .FirstOrDefault();

                if (idClaim is null)
                {
                    throw new ApplicationException("El usuario no tiene el claim del ID");
                }

                return idClaim.Value;
            }
            else
            {
                throw new ApplicationException("El usuario no está autenticado");
            }

        }
    }
}
