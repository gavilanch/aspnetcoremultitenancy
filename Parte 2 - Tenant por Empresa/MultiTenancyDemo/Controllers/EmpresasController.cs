using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancyDemo.Data;
using MultiTenancyDemo.Entidades;
using MultiTenancyDemo.Models;
using MultiTenancyDemo.Servicios;

namespace MultiTenancyDemo.Controllers
{
    [Authorize]
    public class EmpresasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuario servicioUsuario;
        private readonly IServicioCambioTenant servicioCambioTenant;

        public EmpresasController(ApplicationDbContext context,
            IServicioUsuario servicioUsuario,
            IServicioCambioTenant servicioCambioTenant)
        {
            this.context = context;
            this.servicioUsuario = servicioUsuario;
            this.servicioCambioTenant = servicioCambioTenant;
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearEmpresa crearEmpresa)
        {
            if (!ModelState.IsValid)
            {
                return View(crearEmpresa);
            }

            var empresa = new Empresa
            {
                Nombre = crearEmpresa.Nombre
            };

            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            empresa.UsuarioCreacionId = usuarioId;
            context.Add(empresa);
            await context.SaveChangesAsync();

            // Le damos todos los permisos al usuario que crea la app.
            var usuarioEmpresaPermisos = new List<EmpresaUsuarioPermiso>();

            foreach (var permiso in Enum.GetValues<Permisos>())
            {
                usuarioEmpresaPermisos.Add(new EmpresaUsuarioPermiso
                {
                    EmpresaId = empresa.Id,
                    UsuarioId = usuarioId,
                    Permiso = permiso
                });
            }

            context.AddRange(usuarioEmpresaPermisos);
            await context.SaveChangesAsync();

            await servicioCambioTenant.ReemplazarTenant(empresa.Id, usuarioId);

            // Redirigir al usuario al home.
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Cambiar()
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var empresas = await context.EmpresasUsuariosPermisos
                .Include(x => x.Empresa)
                .Where(x => x.UsuarioId == usuarioId)
                .Select(x => x.Empresa!).Distinct().ToListAsync();

            return View(empresas);
        }

        [HttpPost]
        public async Task<IActionResult> Cambiar(Guid id)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            await servicioCambioTenant.ReemplazarTenant(id, usuarioId);
            return RedirectToAction("Index", "Home");
        }


    }
}
