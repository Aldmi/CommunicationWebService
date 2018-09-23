using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.InMemory.Repository
{
    public class InMemorySerialPortOptionRepository : ISerialPortOptionRepository
    {
        private readonly string _connectionString;
        private List<SerialOption> SerialOptions { get;  } = new List<SerialOption>();

  


        #region ctor

        public InMemorySerialPortOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion




        #region Methode

        public SerialOption GetById(int id)
        {
            var findItem = SerialOptions.FirstOrDefault(t => t.Id == id);
            return findItem;
        }


        public async Task<SerialOption> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return GetById(id);
        }


        public SerialOption GetSingle(Expression<Func<SerialOption, bool>> predicate)
        {
            var findItem = SerialOptions.FirstOrDefault(predicate.Compile());
            return findItem;
        }


        public async Task<SerialOption> GetSingleAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return GetSingle(predicate);
        }

        public IEnumerable<SerialOption> GetWithInclude(params Expression<Func<SerialOption, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<SerialOption> List()
        {
            return SerialOptions;
        }


        public IEnumerable<SerialOption> List(Expression<Func<SerialOption, bool>> predicate)
        {
            return SerialOptions.Where(predicate.Compile());
        }


        public async Task<IEnumerable<SerialOption>> ListAsync()
        {
            await Task.CompletedTask;
            return List();
        }


        public async Task<IEnumerable<SerialOption>> ListAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return List(predicate);
        }


        public int Count(Expression<Func<SerialOption, bool>> predicate)
        {
            return SerialOptions.Count(predicate.Compile());
        }


        public async Task<int> CountAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return SerialOptions.Count(predicate.Compile());
        }


        public void Add(SerialOption entity)
        {
            entity.Id = CalcMaxId() + 1;
            SerialOptions.Add(entity);
        }


        public async Task AddAsync(SerialOption entity)
        {
            await Task.CompletedTask;
            Add(entity);
        }


        public void AddRange(IEnumerable<SerialOption> entitys)
        {
            var maxId = CalcMaxId();
            foreach (var entity in entitys)
            {
                entity.Id = ++maxId;
            }
            SerialOptions.AddRange(entitys);
        }


        public async Task AddRangeAsync(IEnumerable<SerialOption> entitys)
        {
            await Task.CompletedTask;
            AddRange(entitys);
        }


        public void Delete(SerialOption entity)
        {
            SerialOptions.Remove(entity);
        }


        public void Delete(Expression<Func<SerialOption, bool>> predicate)
        {
            SerialOptions.RemoveAll(predicate.Compile().Invoke);
        }


        public async Task DeleteAsync(SerialOption entity)
        {
            await Task.CompletedTask;
            Delete(entity);
        }

        public async Task DeleteAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            await Task.CompletedTask;
            Delete(predicate);
        }


        public void Edit(SerialOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = SerialOptions.IndexOf(findItem);
                SerialOptions[index] = entity;
            }
        }


        public async Task EditAsync(SerialOption entity)
        {
            await Task.CompletedTask;
            Edit(entity);
        }


        public bool IsExist(Expression<Func<SerialOption, bool>> predicate)
        {
            return SerialOptions.FirstOrDefault(predicate.Compile()) != null;
        }


        public async Task<bool> IsExistAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return IsExist(predicate);
        }


        private int CalcMaxId()
        {
            var maxId = SerialOptions.Any() ? SerialOptions.Max(d => d.Id) : 0;
            return maxId;
        }

        #endregion
    }
}