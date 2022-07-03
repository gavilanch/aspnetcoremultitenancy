namespace MultiTenancyDemo.Entidades
{
    public class Producto: IEntidadTenant
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string TenantId { get; set; } = null!;
    }
}
