using MultiTenancyDemo.Servicios;
using System.ComponentModel.DataAnnotations;

namespace MultiTenancyDemo.Entidades
{
    public enum Permisos
    {
        [Esconder]
        Nulo = 0, // Permiso que todos los usuarios tienen por ser miembros de una empresa. Solo se elimina al desvincular a un usuario de una empresa.
        [Display(Description = "Puede crear productos")]
        Productos_Crear = 1,
        [Display(Description = "Puede leer productos")]
        Productos_Leer = 2,
        Usuarios_Vincular = 3,
        Permisos_Leer = 4,
        Permisos_Modificar = 5,
    }
}
