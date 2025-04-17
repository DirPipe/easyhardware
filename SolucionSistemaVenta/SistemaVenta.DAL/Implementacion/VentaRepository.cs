using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//references
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        // implemento context
        private readonly DbventaContext _context;

        // constructor
        public VentaRepository(DbventaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            //throw new NotImplementedException();

            Venta ventaGenerada = new Venta();

            using (var transaction = _context.Database.BeginTransaction())
            { 

                try
                {
                    // iterar productos 
                    foreach(DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto producto_encontrado = _context.Productos
                            .Where(x => x.IdProducto == dv.IdProducto)
                            .First();

                        //disminuir stock
                        producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                        _context.Productos.Update(producto_encontrado);
                    }

                    // guardar venta
                    await _context.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _context.NumeroCorrelativos.Where(x => x.Gestion == "venta")
                        .First();

                    // manipulo correlativo de DB
                    correlativo.UltimoNumero = correlativo. UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _context.NumeroCorrelativos.Update(correlativo);
                    await _context.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta;

                    await _context.Venta.AddAsync(entidad);
                    await _context.SaveChangesAsync();

                    ventaGenerada = entidad;

                    // commit operaciones
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

            
            }

            //devuelvo respuesta generada
            return ventaGenerada;

        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            //throw new NotImplementedException();

            List<DetalleVenta> listaResumen = await _context.DetalleVenta
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date &&
                dv.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date).ToListAsync();

            return listaResumen;
        }
    }
}
