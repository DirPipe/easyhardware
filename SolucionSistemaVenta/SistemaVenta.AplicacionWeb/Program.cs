using SistemaVenta.AplicacionWeb.Utilidades.Automapper;
using SistemaVenta.IOC;
using System.Text.Json.Serialization;
using SistemaVenta.AplicacionWeb.Utilidades.Extensiones;
using DinkToPdf;
using DinkToPdf.Contracts;

using Microsoft.AspNetCore.Authentication.Cookies;


namespace SistemaVenta.AplicacionWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Agrega los servicios y configura las opciones de serialización JSON para evitar bug de ciclo perpetuo 500
            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configuración de autenticación
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = "/Acceso/Login";
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                });

            // Configuración de la cadena de conexión
            builder.Services.InyectarDependencia(builder.Configuration);

            // Configuración de AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            var context = new CustomAssemblyLoadContext();
            //context.LoadUnmanagedLibrary(Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.dll"));
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Utilidades/LibreriaPDF/libwkhtmltox.dll"));
            //builder.Services.AddSingleton<IConverter, SynchronizedConverter>(provider => new SynchronizedConverter(new PdfTools()));
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Acceso}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
