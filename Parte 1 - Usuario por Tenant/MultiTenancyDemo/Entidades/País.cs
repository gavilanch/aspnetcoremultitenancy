namespace MultiTenancyDemo.Entidades
{
    public class País: IEntidadComún
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
