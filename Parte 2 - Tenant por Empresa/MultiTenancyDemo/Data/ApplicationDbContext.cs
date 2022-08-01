using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenancyDemo.Entidades;
using MultiTenancyDemo.Servicios;
using System.Linq.Expressions;
using System.Reflection;

namespace MultiTenancyDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private string tenantId;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IServicioTenant servicioTenant)
            : base(options)
        {
            tenantId = servicioTenant.ObtenerTenant();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added
            && e.Entity is IEntidadTenant))
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    throw new Exception("TenantId no encontrado al momento de crear el registro.");
                }

                var entidad = item.Entity as IEntidadTenant;
                entidad!.TenantId = tenantId;
            }

            return base.SaveChangesAsync(cancellationToken);
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EmpresaUsuarioPermiso>()
                .HasKey(x => new { x.EmpresaId, x.UsuarioId, x.Permiso });

            builder.Entity<País>().HasData(new País[]
                {
                    new País{Id = 1, Nombre = "República Dominicana"},
                    new País{Id = 2, Nombre = "México"},
                    new País{Id = 3, Nombre = "Colombia"}
                });


            foreach (var entidad in builder.Model.GetEntityTypes())
            {
                var tipo = entidad.ClrType;

                if (typeof(IEntidadTenant).IsAssignableFrom(tipo))
                {
                    var método = typeof(ApplicationDbContext)
                        .GetMethod(nameof(ArmarFiltroGlobalTenant),
                        BindingFlags.NonPublic | BindingFlags.Static
                           )?.MakeGenericMethod(tipo);

                    var filtro = método?.Invoke(null, new object[] { this })!;
                    entidad.SetQueryFilter((LambdaExpression)filtro);
                    entidad.AddIndex(entidad.FindProperty(nameof(IEntidadTenant.TenantId))!);
                }
                else if (tipo.DebeSaltarValidaciónTenant())
                {
                    continue;
                }
                else
                {
                    throw new Exception($"La entidad {entidad} no ha sido marcada como tenant o común");
                }
            }
        }

        private static LambdaExpression ArmarFiltroGlobalTenant<TEntidad>(
            ApplicationDbContext context)
            where TEntidad : class, IEntidadTenant
        {
            Expression<Func<TEntidad, bool>> filtro = x => x.TenantId == context.tenantId;
            return filtro;
        }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<País> Paises => Set<País>();
        public DbSet<Empresa> Empresas => Set<Empresa>();
        public DbSet<EmpresaUsuarioPermiso> EmpresasUsuariosPermisos => Set<EmpresaUsuarioPermiso>();
        public DbSet<Vinculacion> Vinculaciones => Set<Vinculacion>();


    }
}