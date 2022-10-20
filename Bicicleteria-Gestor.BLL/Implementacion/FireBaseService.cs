using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Firebase.Auth;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> repo;
        public FireBaseService(IGenericRepository<Configuracion> repositorio)
        {
            repo = repositorio;
        }

        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                //Se puede realizar todo esto sin utilizar los datos de la Base De Datos y ponerlos manualmente.

                //Obtener las credenciales
                IQueryable<Configuracion> query = await repo.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                //Dictionary permite asociar llaves y valores este caso la columna PROPIEDAD con la columna VALOR de la tabla Servicio_Correo

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var cred = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancelacion = new CancellationTokenSource();

                //Subimos la imagen en la siguiente "tarea", el servidor retorna una Url con la cual vamos a acceder
                //al archivo que estemos enviando

                var tarea = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(cred.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])  //creacion Carpeta en firebase
                    .Child(NombreArchivo)           //creacion Archivo en firebase
                    .PutAsync(StreamArchivo, cancelacion.Token); //Guardar el archivo que recibe el metodo

                UrlImagen = await tarea;
                //return UrlImagen;
            }
            catch
            {
                return UrlImagen = "";
            }
            return UrlImagen;
        }

        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                //Se puede realizar todo esto sin utilizar los datos de la Base De Datos y ponerlos manualmente.

                //Obtener las credenciales
                IQueryable<Configuracion> query = await repo.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                //Dictionary permite asociar llaves y valores este caso la columna PROPIEDAD con la columna VALOR de la tabla Servicio_Correo

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var cred = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancelacion = new CancellationTokenSource();

                //Subimos la imagen en la siguiente "tarea", el servidor retorna una Url con la cual vamos a acceder
                //al archivo que estemos enviando

                var tarea = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(cred.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])  //creacion Carpeta en firebase
                    .Child(NombreArchivo)           //creacion Archivo en firebase
                    .DeleteAsync();                 //Elimina el archivo

                await tarea;

                return true;
            }
            catch
            {
                return false;
            }
        }

        
    }
}
