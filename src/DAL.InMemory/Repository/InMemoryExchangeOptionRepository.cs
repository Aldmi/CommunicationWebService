using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Exchange;

namespace DAL.InMemory.Repository
{
    public class InMemoryExchangeOptionRepository : IExchangeOptionRepository
    {
        private readonly string _connectionString;
        private List<ExchangeOption> ExchangeOptions { get; } = new List<ExchangeOption>();



        #region ctor

        public InMemoryExchangeOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion



        #region Methode

        public ExchangeOption GetById(int id)
        {
            var findItem = ExchangeOptions.FirstOrDefault(t => t.Id == id);
            return findItem;
        }


        public async Task<ExchangeOption> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return GetById(id);
        }


        public ExchangeOption GetSingle(Expression<Func<ExchangeOption, bool>> predicate)
        {
            var findItem = ExchangeOptions.FirstOrDefault(predicate.Compile());
            return findItem;
        }


        public async Task<ExchangeOption> GetSingleAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return GetSingle(predicate);
        }

        public IEnumerable<ExchangeOption> GetWithInclude(params Expression<Func<ExchangeOption, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<ExchangeOption> List()
        {
            return ExchangeOptions;
        }


        public IEnumerable<ExchangeOption> List(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return ExchangeOptions.Where(predicate.Compile());
        }


        public async Task<IEnumerable<ExchangeOption>> ListAsync()
        {
            await Task.CompletedTask;
            return List();
        }


        public async Task<IEnumerable<ExchangeOption>> ListAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return List(predicate);
        }


        public int Count(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return ExchangeOptions.Count(predicate.Compile());
        }


        public async Task<int> CountAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return ExchangeOptions.Count(predicate.Compile());
        }


        public void Add(ExchangeOption entity)
        {
            entity.Id = CalcMaxId() + 1;
            ExchangeOptions.Add(entity);
        }


        public async Task AddAsync(ExchangeOption entity)
        {
            await Task.CompletedTask;
            Add(entity);
        }


        public void AddRange(IEnumerable<ExchangeOption> entitys)
        {
            var maxId = CalcMaxId();
            foreach (var entity in entitys)
            {
                entity.Id = ++maxId;
            }
            ExchangeOptions.AddRange(entitys);
        }


        public async Task AddRangeAsync(IEnumerable<ExchangeOption> entitys)
        {
            await Task.CompletedTask;
            AddRange(entitys);
        }


        public void Delete(ExchangeOption entity)
        {
            ExchangeOptions.Remove(entity);
        }


        public void Delete(Expression<Func<ExchangeOption, bool>> predicate)
        {
            ExchangeOptions.RemoveAll(predicate.Compile().Invoke);
        }


        public async Task DeleteAsync(ExchangeOption entity)
        {
            await Task.CompletedTask;
            Delete(entity);
        }


        public async Task DeleteAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            await Task.CompletedTask;
            Delete(predicate);
        }


        public void Edit(ExchangeOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = ExchangeOptions.IndexOf(findItem);
                ExchangeOptions[index] = entity;
            }
        }

        public async Task EditAsync(ExchangeOption entity)
        {
            await Task.CompletedTask;
            Edit(entity);
        }


        public bool IsExist(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return ExchangeOptions.FirstOrDefault(predicate.Compile()) != null;
        }


        public async Task<bool> IsExistAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return IsExist(predicate);
        }


        private int CalcMaxId()
        {
            var maxId = ExchangeOptions.Any() ? ExchangeOptions.Max(d => d.Id) : 0;
            return maxId;
        }

        #endregion
    }
}