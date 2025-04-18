using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// referencias
using SistemaVenta.BLL.Interfaces;
using System.Security.Cryptography;


namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        public string GenerarClave()
        {
            // aleatorizar
            string clave = Guid.NewGuid().ToString("N").Substring(0,6);
            return clave;
        }

        public string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                // Convertir el texto a un arreglo de bytes y calcular el hash
                byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                // Convertir el hash a una cadena hexadecimal
                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            return sb.ToString();


        }

       
    }
}
