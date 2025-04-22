using SistemaVenta.AplicacionWeb.Utilidades.Automapper;
using SistemaVenta.IOC;
using System.Text.Json.Serialization;
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

            // Configuración de la cadena de conexión
            builder.Services.InyectarDependencia(builder.Configuration);

            // Configuración de AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));



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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
