using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudTask.BLL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T item);
        void Delete(T item);
        void Update(T item);

        // GetAll
        Task<IReadOnlyList<T>> GetAllAsync();
        // GEtById
        Task<T?> GetAsync(int id);
        void SaveChanges();

        // GetAll WithSpec

    }
}
