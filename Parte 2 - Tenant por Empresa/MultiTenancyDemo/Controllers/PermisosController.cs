using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancyDemo.Data;
using MultiTenancyDemo.Entidades;
using MultiTenancyDemo.Models;
using MultiTenancyDemo.Seguridad;
using MultiTenancyDemo.Servicios;
using System.ComponentModel.DataAnnotations;

namespace MultiTenancyDemo.Controllers
{
    [Authorize]
    public class PermisosController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioTenant servicioTenant;

        public PermisosController(ApplicationDbContext context,
            IServicioTenant servicioTenant)
        {
            this.context = context;
            this.servicioTenant = servicioTenant;
        }

        [TienePermiso(Permisos.Permisos_Leer)]
        public async Task<ActionResult> Index()
        {
            var tenantId = new Guid(servicioTenant.ObtenerTenant());
            var modelo = await context.Empresas.
                Include(x => x.EmpresaUsuariosPermisos).ThenInclude(x => x.Usuario)
                .Where(x => x.Id == tenantId)
                .Select(x => new IndexPermisosDTO
                {
                    NombreEmpresa = x.Nombre,
                    Empleados = x.EmpresaUsuariosPermisos.Select(y => new UsuarioDTO
                    {
                        Email = y.Usuario!.Email
                    }).Distinct()
                }).FirstOrDefaultAsync();

            return View(modelo);

        }

        [TienePermiso(Permisos.Permisos_Leer)]
        public async Task<IActionResult> Administrar(string email)
        {
            var tenantId = new Guid(servicioTenant.ObtenerTenant());
            var usuarioId = await context.Users
                .Where(x => x.Email == email).Select(x => x.Id).FirstOrDefaultAsync();

            if (usuarioId is null)
            {
                return RedirectToAction("Index", "Permisos");
            }

            var permisos = await context.EmpresasUsuariosPermisos
    .Where(x => x.EmpresaId == tenantId && x.UsuarioId == usuarioId
    && x.Permiso != Permisos.Nulo).ToListAsync();

            var permisosUsuarioDiccionario = permisos.ToDictionary(x => x.Permiso);

            var modelo = new AdministrarPermisosDTO();
            modelo.UsuarioId = usuarioId;
            modelo.Email = email;

            foreach (var permiso in Enum.GetValues<Permisos>())
            {
                var campo = typeof(Permisos).GetField(permiso.ToString())!;
                var esconder = campo.IsDefined(typeof(EsconderAttribute), false);

                if (esconder)
                {
                    continue;
                }

                var descripcion = permiso.ToString();

                if (campo.IsDefined(typeof(DisplayAttribute), false))
                {
                    var displayAttr = (DisplayAttribute)Attribute
                        .GetCustomAttribute(campo, typeof(DisplayAttribute))!;
                    descripcion = displayAttr.Description;
                }

                modelo.Permisos.Add(new PermisoUsuarioDTO()
                {
                    Descripcion = descripcion,
                    Permiso = permiso,
                    LoTiene = permisosUsuarioDiccionario.ContainsKey(permiso)
                });

            }

            return View(modelo);
                
        }

        [TienePermiso(Permisos.Permisos_Modificar)]
        [HttpPost]
        public async Task<IActionResult> Administrar(AdministrarPermisosDTO modelo)
        {
            var tenantId = new Guid(servicioTenant.ObtenerTenant());

            // Siempre agregamos el permiso por defecto.
            modelo.Permisos.Add(new PermisoUsuarioDTO() 
            { LoTiene = true, Permiso = Permisos.Nulo });

            // Borramos todos los permisos de la persona
            await context.Database
                .ExecuteSqlInterpolatedAsync($@"DELETE EmpresasUsuariosPermisos 
                   WHERE UsuarioId = {modelo.UsuarioId} AND EmpresaId = {tenantId}");

            // Filtramos los permisos a agregar
            var permisosFiltrados = modelo.Permisos
                .Where(x => x.LoTiene).Select(x => new EmpresaUsuarioPermiso
            {
                EmpresaId = tenantId,
                UsuarioId = modelo.UsuarioId,
                Permiso = x.Permiso
            });

            // Agregamos los permisos a la tabla
            context.AddRange(permisosFiltrados);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
