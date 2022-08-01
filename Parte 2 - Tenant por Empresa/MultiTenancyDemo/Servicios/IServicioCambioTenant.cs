namespace MultiTenancyDemo.Servicios
{
    public interface IServicioCambioTenant
    {
        Task ReemplazarTenant(Guid idEmpresa, string usuarioId);
    }
}
