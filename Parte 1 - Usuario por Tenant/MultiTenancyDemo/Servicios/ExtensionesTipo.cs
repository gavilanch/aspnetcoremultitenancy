using Microsoft.AspNetCore.Identity;
using MultiTenancyDemo.Entidades;

namespace MultiTenancyDemo.Servicios
{
    public static class ExtensionesTipo
    {
        public static bool DebeSaltarValidaciónTenant(this Type t)
        {
            var booleanos = new List<bool>()
                        {
                                t.IsAssignableFrom(typeof(IdentityRole)),
                                t.IsAssignableFrom(typeof(IdentityRoleClaim<string>)),
                                t.IsAssignableFrom(typeof(IdentityUser)),
                                t.IsAssignableFrom(typeof(IdentityUserLogin<string>)),
                                t.IsAssignableFrom(typeof(IdentityUserRole<string>)),
                                t.IsAssignableFrom(typeof(IdentityUserToken<string>)),
                                t.IsAssignableFrom(typeof(IdentityUserClaim<string>)),
                                typeof(IEntidadComún).IsAssignableFrom(t)
                        };

            var resultado = booleanos.Aggregate((b1, b2) => b1 || b2);

            return resultado;
        }

    }
}
