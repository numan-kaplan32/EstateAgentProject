using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kervan.Core.CoreServiceFolder;

namespace Kervan.Core.ICoreServiceFolder
{
   public  interface ICoreService <T> where T : CoreService
    {
        /*Senkron Metodlar*/

        public bool Create(T entity);
        public bool Update(T entity);
        public bool Delete(T entity);
        public T GetById(int id);
        public IEnumerable<T> GetAll();
        public bool Save();

        /*Asenkron Metodlar*/

        public Task<bool> CreateAsync(T entity);
        public Task<bool> UpdateAsync(T entity);
        public Task<bool> DeleteAsync(T entity);
        public Task<T> GetByIdAsync(int id);
        public Task<IEnumerable<T>> GetAllAsync();
        

        public Task<bool> SaveAsync();

    }
}
