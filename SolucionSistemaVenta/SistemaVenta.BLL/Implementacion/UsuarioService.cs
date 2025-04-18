using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ref
using Microsoft.EntityFrameworkCore;
using System.Net; 
using System.Text;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        // import services
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFireBaseService _fireBaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;
        
        // const
        public UsuarioService(
            IGenericRepository<Usuario> repositorio,
             IFireBaseService fireBaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService
            )

        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }




        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            // include roles
            return query.Include(rol => rol.IdRolNavigation).ToList();
        }

        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            // verificar si el correo ya existe
            Usuario usuario_existe = await _repositorio.Obtener(x => x.Correo == entidad.Correo);
            // error exception
            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");


            try
            {
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = NombreFoto;
                if (Foto != null)
                {
                    // subir foto a firebase
                    string urlFoto = await _fireBaseService.SubirStorage(Foto,"carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = urlFoto;

                }
              
                Usuario usuario_creado = await _repositorio.Crear(entidad);
                if(usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("Error al crear el usuario");

                // enviar correo
                if(UrlPlantillaCorreo != "")
                {
                   UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuario_creado.Correo).Replace("[clave]",clave_generada);
                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream()) {

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

                IQueryable<Usuario> query = await _repositorio.Consultar(x => x.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(rol => rol.IdRolNavigation).First();

                return usuario_creado;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            // verificar si el correo ya existe
            Usuario usuario_existe = await _repositorio.Obtener(x => x.Correo == entidad.Correo && x.IdUsuario != entidad.IdUsuario);
            // error exception
            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(x => x.IdUsuario == entidad.IdUsuario);
                Usuario usuario_editar = queryUsuario.First();

                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;

                if(usuario_editar.NombreFoto == "")
                    usuario_editar.NombreFoto = NombreFoto;

                if(Foto != null)
                {
                    // subir foto a firebase
                    string urlFoto = await _fireBaseService.SubirStorage(Foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuario_editar);

                if (respuesta == false)
                    throw new TaskCanceledException("Error al editar el usuario");

                Usuario usuario_editado = queryUsuario.Include(rol => rol.IdRolNavigation).First();

                return usuario_editado;

            }
            catch { 
                throw;
            }

        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(x => x.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuario_encontrado);

                if (respuesta)
                   await _fireBaseService.EliminarStorage("carpeta_usuario",nombreFoto);

                return true;

            }
            catch
            {
                throw;
            }


        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = _utilidadesService.ConvertirSha256(clave);

            Usuario usuario_encontrado = await _repositorio.Obtener(x => x.Correo.Equals(correo) 
            && x.Clave.Equals(clave_encriptada));

            return usuario_encontrado;
        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
           IQueryable<Usuario> query = await _repositorio.Consultar(x => x.IdUsuario == idUsuario);
            
            Usuario resultado = query.Include(rol => rol.IdRolNavigation).FirstOrDefault();

            return resultado;
        }

        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(x => x.IdUsuario == entidad.IdUsuario);

                if(usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repositorio.Editar(usuario_encontrado);

                return respuesta;
            }
            catch
            {
                throw;
            }

        }


        public async Task<bool> CambiarClave(int idUsuario, string ClaveActual, string ClaveNueva)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(x => x.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                if(usuario_encontrado.Clave != _utilidadesService.ConvertirSha256(ClaveActual))
                    throw new TaskCanceledException("La clave actual no es correcta");

                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(ClaveNueva);
                bool respuesta = await _repositorio.Editar(usuario_encontrado);

                return respuesta;

            }
            catch(Exception ex)
            {
                throw;
            }
        }
        

        public async Task<bool> RestablecerClave(string Correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(x => x.Correo == Correo);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("No se ha encontrado ningun usuario asociado al Correo");

                string clave_generada = _utilidadesService.GenerarClave();
                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(clave_generada);

                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", clave_generada);
                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
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
                    await _correoService.EnviarCorreo(Correo, "Contraseña Establecida", htmlCorreo);
                
                if(!correo_enviado)
                    throw new TaskCanceledException("Hay un error. Por favor intentar de nuevo mas tarde");

                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
