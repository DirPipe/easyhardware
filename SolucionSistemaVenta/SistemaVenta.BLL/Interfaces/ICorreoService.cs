using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ICorreoService
    {

        // funcion para enviar correo
        Task<bool> EnviarCorreo(string destinatario, string asunto, string mensaje);    

    }
}
