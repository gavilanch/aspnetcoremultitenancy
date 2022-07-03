using MultiTenancyDemo.Entidades;

namespace MultiTenancyDemo.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Producto> Productos { get; set; } = new List<Producto>();
        public IEnumerable<País> Países { get; set; } = new List<País>();
    }
}
