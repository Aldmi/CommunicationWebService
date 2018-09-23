using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Device;

namespace DAL.InMemory.Repository
{
    public class InMemoryDeviceOptionRepository : IDeviceOptionRepository
    {
        private readonly string _connectionString;
        private List<DeviceOption> DeviceOptions { get; } = new List<DeviceOption>();



        #region ctor

        public InMemoryDeviceOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion




        #region Methode

        public DeviceOption GetById(int id)
        {
            var findItem = DeviceOptions.FirstOrDefault(t => t.Id == id);
            return findItem;
        }

    
        public async Task<DeviceOption> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return GetById(id);
        }


        public DeviceOption GetSingle(Expression<Func<DeviceOption, bool>> predicate)
        {
            var findItem = DeviceOptions.FirstOrDefault(predicate.Compile());
            return findItem;
        }


        public async Task<DeviceOption> GetSingleAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return GetSingle(predicate);
        }

        public IEnumerable<DeviceOption> GetWithInclude(params Expression<Func<DeviceOption, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<DeviceOption> List()
        {
            return DeviceOptions;
        }


        public IEnumerable<DeviceOption> List(Expression<Func<DeviceOption, bool>> predicate)
        {
            return DeviceOptions.Where(predicate.Compile());
        }


        public async Task<IEnumerable<DeviceOption>> ListAsync()
        {
            await Task.CompletedTask;
            return List();
        }


        public async Task<IEnumerable<DeviceOption>> ListAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return List(predicate);
        }


        public int Count(Expression<Func<DeviceOption, bool>> predicate)
        {
            return DeviceOptions.Count(predicate.Compile());
        }


        public async Task<int> CountAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return DeviceOptions.Count(predicate.Compile());
        }


        public void Add(DeviceOption entity)
        {
            entity.Id = CalcMaxId() + 1;
            DeviceOptions.Add(entity);
        }

        public async Task AddAsync(DeviceOption entity)
        {
            await Task.CompletedTask;
            Add(entity);
        }


        public void AddRange(IEnumerable<DeviceOption> entitys)
        {
            var maxId = CalcMaxId();
            foreach (var entity in entitys)
            {
                entity.Id = ++maxId;
            }
            DeviceOptions.AddRange(entitys);
        }


        public async Task AddRangeAsync(IEnumerable<DeviceOption> entitys)
        {
            await Task.CompletedTask;
            AddRange(entitys);
        }


        public void Delete(DeviceOption entity)
        {
            DeviceOptions.Remove(entity);
        }


        public void Delete(Expression<Func<DeviceOption, bool>> predicate)
        {
            DeviceOptions.RemoveAll(predicate.Compile().Invoke);
        }


        public async Task DeleteAsync(DeviceOption entity)
        {
            await Task.CompletedTask;
            Delete(entity);
        }


        public async Task DeleteAsync(Expression<Func<DeviceOption, bool>> predicate)
        {    
            await Task.CompletedTask;
            Delete(predicate);
        }


        public void Edit(DeviceOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = DeviceOptions.IndexOf(findItem);
                DeviceOptions[index] = entity;
            }
        }


        public async Task EditAsync(DeviceOption entity)
        {
            await Task.CompletedTask;
            Edit(entity);
        }


        public bool IsExist(Expression<Func<DeviceOption, bool>> predicate)
        {
            return DeviceOptions.FirstOrDefault(predicate.Compile()) != null;
        }


        public async Task<bool> IsExistAsync(Expression<Func<DeviceOption, bool>> predicate)
        {   
            await Task.CompletedTask;
            return IsExist(predicate);
        }


        private int CalcMaxId()
        {
            var maxId =  DeviceOptions.Any() ? DeviceOptions.Max(d => d.Id) : 0;
            return maxId;
        }

        #endregion
    }
}