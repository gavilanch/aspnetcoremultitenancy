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
    public class VinculacionesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuario servicioUsuario;
        private readonly IServicioTenant servicioTenant;

        public VinculacionesController(ApplicationDbContext context,
            IServicioUsuario servicioUsuario, IServicioTenant servicioTenant)
        {
            this.context = context;
            this.servicioUsuario = servicioUsuario;
            this.servicioTenant = servicioTenant;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            return await RetornarVinculacionesPendientes(usuarioId);
        }

        private async Task<IActionResult> RetornarVinculacionesPendientes(string usuarioId)
        {
            var vinculacionesPendientes = await context.Vinculaciones
                .Include(x => x.Empresa)
                .Where(x => x.Estatus == VinculacionEstatus.Pendiente
                && x.UsuarioId == usuarioId).ToListAsync();

            return View(vinculacionesPendientes);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Guid empresaId,
            VinculacionEstatus vinculacionEstatus)
        {
            var usuarioId = servicioUsuario.ObtenerUsuarioId();
            var vinculacion = await context.Vinculaciones
                    .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId
                    && x.EmpresaId == empresaId
                    && x.Estatus == VinculacionEstatus.Pendiente);

            if (vinculacion is null)
            {
                ModelState.AddModelError("", "Ha habido un error: vinculación no encontrada");
                return await RetornarVinculacionesPendientes(usuarioId);
            }

            if (vinculacionEstatus == VinculacionEstatus.Aceptada)
            {
                var permisoNulo = new EmpresaUsuarioPermiso()
                {
                    Permiso = Permisos.Nulo,
                    EmpresaId = empresaId,
                    UsuarioId = usuarioId
                };

                context.Add(permisoNulo);
            }

            vinculacion.Estatus = vinculacionEstatus;
            await context.SaveChangesAsync();

            return RedirectToAction("Cambiar", "Empresas");
        }

        public async Task<IActionResult> Vincular()
        {
            var empresaId = servicioTenant.ObtenerTenant();

            if (string.IsNullOrEmpty(empresaId))
            {
                return RedirectToAction("Index", "Home");
            }

            var empresaIdGuid = new Guid(empresaId);

            var empresa = await context
                .Empresas.FirstOrDefaultAsync(x => x.Id == empresaIdGuid);

            if (empresa is null)
            {
                return RedirectToAction("Index", "Home");
            }

            var modelo = new VincularUsuario
            {
                EmpresaId = empresa.Id,
                NombreEmpresa = empresa.Nombre
            };

            return View(modelo);


        }

        [HttpPost]
        public async Task<IActionResult> Vincular(VincularUsuario modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuarioAVincular = await context.Users
                .FirstOrDefaultAsync(x => x.Email == modelo.EmailUsuario);

            if (usuarioAVincular is null)
            {
                ModelState.AddModelError(nameof(modelo.EmailUsuario), "No existe un usuario con ese email");
                return View(modelo);
            }

            var vinculacion = new Vinculacion
            {
                EmpresaId = modelo.EmpresaId,
                UsuarioId = usuarioAVincular.Id,
                Estatus = VinculacionEstatus.Pendiente,
                FechaCreacion = DateTime.UtcNow
            };

            context.Add(vinculacion);
            await context.SaveChangesAsync();
            return RedirectToAction("UsuarioVinculado");
        }

        public IActionResult UsuarioVinculado()
        {
            return View();
        }

    }
}
