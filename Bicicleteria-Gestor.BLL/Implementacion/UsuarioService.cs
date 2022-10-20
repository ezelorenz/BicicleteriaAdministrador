using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> repo;
        private readonly IFireBaseService _fireBaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;
        public UsuarioService(
            IGenericRepository<Usuario> repositorio, 
            IFireBaseService fireBaseService, 
            IUtilidadesService utilidadesService,
            ICorreoService correoService
            )
        {
            repo = repositorio;
            _fireBaseService = fireBaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await repo.Consultar();
            return query.Include(rol => rol.IdRolNavigation).ToList();
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = _utilidadesService.CovertirSha256(clave);

            Usuario usuario_encontrado = await repo.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(clave));
            return usuario_encontrado;
        }

        public async Task<Usuario> ObtenerProId(int idUsuario)
        {
            IQueryable<Usuario> query = await repo.Consultar(u => u.IdUsuario == idUsuario);

            //Incluir el rol a ese Usuario.
            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();
            return resultado;

        }
        public async Task<Usuario> Crear(Usuario entidad, Stream foto = null, string nombreFoto = "", string urlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await repo.Obtener(u => u.Correo == entidad.Correo);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");

            try
            {

                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.CovertirSha256(clave_generada);
                entidad.NombreFoto = nombreFoto;

                if (foto != null) {
                    string urlFoto = await _fireBaseService.SubirStorage(foto, "carpeta_usuario", nombreFoto);
                    entidad.UrlFoto = urlFoto;
                }

                Usuario usuario_creado = await repo.Crear(entidad);

                //Si el usuario no se creo, cancela las tareas anteriores y dispara el mensaje de error.
                if(usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el Usuario");

                //Mas adelante vamos a ver el funcionamiento de lo siguiente.
                if(urlPlantillaCorreo != "")
                {
                    urlPlantillaCorreo = urlPlantillaCorreo.Replace("[correo]", usuario_creado.Correo)
                                                           .Replace("[clave]", clave_generada);

                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        using(Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                                readerStream = new StreamReader(dataStream);
                            else
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = readerStream.ReadToEnd();

                            response.Close();
                            readerStream.Close();
                        }
                    }
                    if (htmlCorreo != "")
                        await _correoService.EnviarCorreo(usuario_creado.Correo, "Cuenta Creada", htmlCorreo);
                }
                IQueryable<Usuario> query = await repo.Consultar(u => u.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(rol => rol.IdRolNavigation).First();

                return usuario_creado;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Usuario> Editar(Usuario entidad, Stream foto = null, string nombreFoto = "")
        {
            Usuario usuario_existe = await repo.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                IQueryable<Usuario> queryUsuario = await repo.Consultar(u => u.IdUsuario == entidad.IdUsuario);

                Usuario usuario_editar = queryUsuario.First();
                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;
                usuario_editar.EsActivo = entidad.EsActivo;

                if (usuario_editar.NombreFoto == "")
                    usuario_editar.NombreFoto = nombreFoto;

                if(foto != null)
                {
                    string urlFoto = await _fireBaseService.SubirStorage(foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlFoto;
                }

                bool respuesta = await repo.Editar(usuario_editar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el Usuario");

                Usuario usuario_editado = queryUsuario.Include(rol => rol.IdRolNavigation).First();
                return usuario_editado;
            }
            catch
            {
                throw;
            }

        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await repo.Obtener(u => u.IdUsuario == idUsuario);

                if(usuario_encontrado == null)
                    throw new TaskCanceledException("El Usuario no existe");

                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await repo.Eliminar(usuario_encontrado);

                //si el metodo de eliminar el usuario devuelve true, se procede a eliminar la foto de FireBase
                if (respuesta)
                    await _fireBaseService.EliminarStorage("carpeta_usuario", nombreFoto);

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try 
            {
                Usuario usuario_encontrado = await repo.Obtener(u => u.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El Usuario no existe");

                if (usuario_encontrado.Clave != _utilidadesService.CovertirSha256(claveActual))
                    throw new TaskCanceledException("La contraseña ingresada no corresponde con la que tiene asignada actualmente");

                usuario_encontrado.Clave = _utilidadesService.CovertirSha256(claveNueva);

                bool respuesta = await repo.Editar(usuario_encontrado);
                return respuesta;
            }
            catch(Exception ex)
            {
                throw;
            }
            

        }

        public async Task<bool> RestablecerClave(string correo, string urlPlantillaCorreo)
        {
            try
            {
                Usuario usuario_encontrado = await repo.Obtener(u => u.Correo == correo);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("No hay un Usuario asociado al correo");

                string clave_generada = _utilidadesService.GenerarClave();
                usuario_encontrado.Clave = _utilidadesService.CovertirSha256(clave_generada);


                //Enviar plantilla de correo unicamente con la clave generada
                urlPlantillaCorreo = urlPlantillaCorreo.Replace("[clave]", clave_generada);

                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;

                        if (response.CharacterSet == null)
                            readerStream = new StreamReader(dataStream);
                        else
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                        htmlCorreo = readerStream.ReadToEnd();

                        response.Close();
                        readerStream.Close();
                    }
                }

                bool correo_enviado = false;

                if (htmlCorreo != "")
                    correo_enviado = await _correoService.EnviarCorreo(correo, "Contraseña Restablecida", htmlCorreo);

                if(!correo_enviado)
                    throw new TaskCanceledException("No se pudo enviar el correo, intentalo nuevamente mas tarde");

                bool respuesta = await repo.Editar(usuario_encontrado);
                return respuesta;

            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await repo.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if(usuario_encontrado == null)
                    throw new TaskCanceledException("El Usuario no existe");

                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;

                bool respuesta = await repo.Editar(usuario_encontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

    }
}
