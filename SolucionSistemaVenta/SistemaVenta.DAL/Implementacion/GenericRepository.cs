using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// inyecto dependencias
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Implementacion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

        // metodo constructor para inyectar el contexto de la DB
        private readonly DbventaContext _context;
        public GenericRepository(DbventaContext context)
        {
            _context = context;
        }

        // se implementa la interfaz creada
        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            //throw new NotImplementedException();

            try
            {
                TEntity entidad = await _context.Set<TEntity>()
                    .FirstOrDefaultAsync(filtro);
                return entidad;
            }

            catch
            {

                throw;
            }
        }

        public async Task<TEntity> Crear(TEntity entidad)
        {
            //throw new NotImplementedException();
            try
            {
                _context.Set<TEntity>().Add(entidad);
                await _context.SaveChangesAsync();
                return entidad;
            }

            catch
            {

                throw;
            }
        }

        public async Task<bool> Editar(TEntity entidad)
        {
            //throw new NotImplementedException();

            try
            {
                _context.Update(entidad);
                await _context.SaveChangesAsync();
                return true;
            }

            catch
            {

                throw;
            }

        }

        public async Task<bool> Eliminar(TEntity entidad)
        {
            //throw new NotImplementedException();

            try
            {
                _context.Remove(entidad);
                await _context.SaveChangesAsync();
                return true;
            }

            catch
            {

                throw;
            }

        }

       
        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            //throw new NotImplementedException();

            IQueryable<TEntity> queryEntidad = filtro == null ?
                _context.Set<TEntity>() :
                _context.Set<TEntity>().Where(filtro);
            return queryEntidad;
        }

       

        

       

        
    }
}
