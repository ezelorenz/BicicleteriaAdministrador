namespace Bicicleteria_Gestor.APP.Models.ViewModels
{
    public class VMProducto
    {
        public int IdProducto { get; set; }
        public string? CodigoBarra { get; set; }
        public string? Marca { get; set; }
        public string? Descripcion { get; set; }
        public int? IdCategoria { get; set; }
        public string? NombreCategoria { get; set; } //hacer referencia que viene de idCategoriaNavigation 
        public int? Stock { get; set; }
        public string? UrlImagen { get; set; }
        public string? Precio { get; set; } //convertir a int
        public int? EsActivo { get; set; } //convertir a bool
    }
}
