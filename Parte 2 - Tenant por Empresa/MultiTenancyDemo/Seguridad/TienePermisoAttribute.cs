using Microsoft.AspNetCore.Authorization;
using MultiTenancyDemo.Entidades;
using MultiTenancyDemo.Servicios;

namespace MultiTenancyDemo.Seguridad
{
    public class TienePermisoAttribute: AuthorizeAttribute
    {
        public TienePermisoAttribute(Permisos permisos)
        {
            Permisos = permisos;
        }

        public Permisos Permisos
        {
            get
            {
                // TienePermisoProductos_Crear
                if (Enum.TryParse(typeof(Permisos), 
                    Policy!.Substring(Constantes.PrefijoPolitica.Length), 
                    ignoreCase: true, out var permiso))
                {
                    return (Permisos)permiso!;
                }

                return Permisos.Nulo;
            }
            set
            {
                Policy = $"{Constantes.PrefijoPolitica}{value.ToString()}";
            }
        }

    }
}
