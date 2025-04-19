using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// referencias
using System.Net;
using System.Net.Mail;

using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        //constructor
        public CorreoService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        // implementacion interfaz
        public async Task<bool> EnviarCorreo(string destinatario, string asunto, string mensaje)
        {
            //throw new NotImplementedException();

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c=> c.Recurso.Equals("Servicio_Correo"));
                
                // recupero columna propiedad y valor de la DB y almaceno en diccionario
                Dictionary<string,string> Config = query.ToDictionary(keySelector : c=>c.Propiedad,elementSelector: c=> c.Valor);

                //config credenciales correo 
                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true,
                };

                // configuracion destinatario
                correo.To.Add(new MailAddress(destinatario));

                // configuracion cliente
                var clienteServidor = new SmtpClient
                {
                    Host = Config["host"],
                    Port = Convert.ToInt32(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                    //EnableSsl = Convert.ToBoolean(Config["ssl"]),
                };

                // envio correo
                clienteServidor.Send(correo);
                return true;

            }
            catch 
            {
                return false;
            }


        }
    }
}
