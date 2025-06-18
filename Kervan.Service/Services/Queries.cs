using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Kervan.Core.CoreServiceFolder;
using Kervan.Core.ICoreServiceFolder;
using Kervan.Model.KervanDbModelFolder;
using Kervan.Model.TablesOfKervan;
using Microsoft.EntityFrameworkCore;

namespace Kervan.Service.Services
{
    public class Queries<T> : ICoreService<T> where T : CoreService
    {
        private readonly KervanDbContext _kervanModel;

        public Queries(KervanDbContext kervanDb)
        {
            _kervanModel = kervanDb;
        }

        public bool Create(T entity)
        {
            try
            {
                _kervanModel.Set<T>().Add(entity);
                return Save();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateAsync(T entity)
        {
            try
            {
                await _kervanModel.Set<T>().AddAsync(entity);
                return await SaveAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                _kervanModel.Set<T>().Remove(entity);
                return Save();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _kervanModel.Set<T>().Remove(entity);
                return await SaveAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<T> GetAll()
        {
            return _kervanModel.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _kervanModel.Set<T>().ToListAsync();
        }

        public T GetById(int id)
        {
            return _kervanModel.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _kervanModel.Set<T>().FindAsync(id);
        }

        public bool Save()
        {
            return _kervanModel.SaveChanges() > 0;
        }

        public async Task<bool> SaveAsync()
        {
            return await _kervanModel.SaveChangesAsync() > 0;
        }

        public bool Update(T entity)
        {
            try
            {
                _kervanModel.Set<T>().Update(entity);
                return Save();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _kervanModel.Set<T>().Update(entity);
                return await SaveAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }
    

    }
}