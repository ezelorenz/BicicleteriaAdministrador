namespace Bicicleteria_Gestor.APP.Models.ViewModels
{
    public class VMDashBoard
    {
        public int TotalVentas { get; set; }
        public string? TotalIngresos { get; set; }
        public int TotalProductos { get; set; }

        public List<VMVentasSemana> VentasUltimaSemana { get; set; }
    }
}
