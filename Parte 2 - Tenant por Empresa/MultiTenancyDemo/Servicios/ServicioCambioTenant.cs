using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenancyDemo.Data;
using System.Security.Claims;

namespace MultiTenancyDemo.Servicios
{
    public class ServicioCambioTenant: IServicioCambioTenant
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;

        public ServicioCambioTenant(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        public async Task ReemplazarTenant(Guid idEmpresa, string usuarioId)
        {
            var usuario = await userManager.FindByIdAsync(usuarioId);

            var claimTenantExistente = await context.UserClaims
    .FirstOrDefaultAsync(x => x.ClaimType == Constantes.ClaimTenantId 
    && x.UserId == usuarioId);

            if (claimTenantExistente is not null)
            {
                context.Remove(claimTenantExistente);
            }

            var claimTenantNuevo = new Claim(Constantes.ClaimTenantId, 
                idEmpresa.ToString());

            await userManager.AddClaimAsync(usuario, claimTenantNuevo);

            await signInManager.SignInAsync(usuario, isPersistent: true);
        }
    }
}
