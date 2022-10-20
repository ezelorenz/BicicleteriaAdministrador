using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using System.Net;
using System.Net.Mail;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> repo;
        public CorreoService(IGenericRepository<Configuracion> repositorio)
        {
            repo = repositorio;
        }
        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string MensajeCorreo)
        {
            try
            {
                //Se puede realizar todo esto sin utilizar los datos de la Base De Datos y ponerlos manualmente.

                //Obtener las credenciales
                IQueryable<Configuracion> query = await repo.Consultar(c => c.Recurso.Equals("Servicio_Correo"));

                //Dictionary permite asociar llaves y valores este caso la columna PROPIEDAD con la columna VALOR de la tabla Servicio_Correo

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c=> c.Propiedad, elementSelector: c=> c.Valor);

                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = Asunto,
                    Body = MensajeCorreo,
                    IsBodyHtml = true
                };

                correo.To.Add(new MailAddress(CorreoDestino));

                var clienteServidor = new SmtpClient()
                {
                    Credentials = credenciales,
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };
                clienteServidor.Send(correo);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
