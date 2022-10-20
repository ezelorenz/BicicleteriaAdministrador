using Bicicleteria_Gestor.BLL.Interfaces;
using System.Text;
using XSystem.Security.Cryptography;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        public string GenerarClave()
        {
            //retorna una cadena de texto aleatoria de numeros y letras de una longitud de 6 caracteres.
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);
            return clave;
        }

        // el siguiente metodo encipta la clave que el usuario va a ingresar, para que no se pueda entender en la base de datos
        public string CovertirSha256(string texto)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            Encoding enc = Encoding.UTF8;

            byte[] result = sha1.ComputeHash(enc.GetBytes(texto));  //Convierte el texto en un array de bytes
            StringBuilder sb = new StringBuilder();
            //recorremos cada uno de los elementos dentro de result

            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2")); //Concatenar cada uno de los resultados a mi bariable Sb
                                             //que la convierta a string y el doble de largo
            }
            return sb.ToString();
        }

        /*public string CovertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(texto));  //Convierte el texto en un array de bytes

                //recorremos cada uno de los elementos dentro de result
                
                foreach(byte b in result)
                {
                    sb.Append(b.ToString("x2")); //Concatenar cada uno de los resultados a mi bariable Sb
                                                    //que la convierta a string y el doble de largo
                }
            }
            return sb.ToString(); 
        }*/
    }
}
