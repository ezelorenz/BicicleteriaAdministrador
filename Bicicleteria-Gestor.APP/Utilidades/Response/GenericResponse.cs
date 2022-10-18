namespace Bicicleteria_Gestor.APP.Utilidades.Response
{
    public class GenericResponse<TObject>
    {
        //Clase de respuesta para todas las solicitudes web

        public bool Estado { get; set; }
        public string? Mensaje { get; set; }
        public TObject? Objeto { get; set; }
        public List<TObject>? ListaObjeto { get; set; }
    }
}
