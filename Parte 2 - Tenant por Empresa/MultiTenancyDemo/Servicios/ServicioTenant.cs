using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace MultiTenancyDemo.Servicios
{
    public class ServicioTenant : IServicioTenant
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServicioTenant(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string ObtenerTenant()
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return string.Empty;
            }

            var authTicket = DecryptAuthCookie(httpContext);

            if (authTicket is null)
            {
                return string.Empty;
            }

            var claimTenant = authTicket.Principal.Claims.FirstOrDefault(x => x.Type == Constantes.ClaimTenantId);

            if (claimTenant is null)
            {
                return string.Empty;
            }

            return claimTenant.Value;
        }

        private static AuthenticationTicket? DecryptAuthCookie(HttpContext httpContext)
        {
            var opt = httpContext.RequestServices
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get("Identity.Application");

            var cookie = opt.CookieManager.GetRequestCookie(httpContext, opt.Cookie.Name!);

            return opt.TicketDataFormat.Unprotect(cookie);
        }

    }
}
