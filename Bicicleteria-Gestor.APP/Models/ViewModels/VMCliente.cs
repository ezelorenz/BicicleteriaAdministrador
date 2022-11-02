namespace Bicicleteria_Gestor.APP.Models.ViewModels
{
    public class VMCliente
    {
        public int IdCliente { get; set; }
        public string? Documento { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Ciudad { get; set; }
        public string? Direccion { get; set; }
        public int? Deudor { get; set; }
    }
}
