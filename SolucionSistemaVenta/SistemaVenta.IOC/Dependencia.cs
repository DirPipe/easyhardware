using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.DAL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.BLL.Implementacion;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración de la cadena de conexión
            services.AddDbContext<DbventaContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CadenaSQL")));

            // inyeccion de los repositorios genéricos
            services.AddTransient(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IVentaRepository, VentaRepository>();
            
            // inyeccion funciones corre
            services.AddScoped<ICorreoService, CorreoService>();
            // inyeccion firebase
            services.AddScoped<IFireBaseService,FireBaseService>();
            // inyeccion de las utilidades
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            // inyeccion roles           
            services.AddScoped<IRolService, RolService>();
            //inyeccion de usuarios
            services.AddScoped<IUsuarioService, UsuarioService>();

            // inyeccion de negocio
            services.AddScoped<INegocioService, NegocioService>();
            // inyeccion categoria
            services.AddScoped<ICategoriaService, CategoriaService>();



            // Inyección de dependencias para los repositorios y servicios
            //services.AddScoped<IProductoRepository, ProductoRepository>();
            //services.AddScoped<IProductoService, ProductoService>();
            //services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            //services.AddScoped<ICategoriaService, CategoriaService>();
        }
    }
}
