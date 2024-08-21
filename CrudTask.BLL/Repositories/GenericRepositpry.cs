using CrudTask.BLL.Interfaces;
using CrudTask.DAL.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudTask.BLL.Repositories
{
    public class GenericRepositories<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepositories(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public async Task AddAsync(T item) => await _context.Set<T>().AddAsync(item);

        public void Delete(T item) => _context.Set<T>().Remove(item);


        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Update(T item) => _context.Set<T>().Update(item);
    }
}
