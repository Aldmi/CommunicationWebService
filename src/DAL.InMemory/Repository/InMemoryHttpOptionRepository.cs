using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.InMemory.Repository
{
    public class InMemoryHttpOptionRepository : IHttpOptionRepository
    {
        private readonly string _connectionString;
        private List<HttpOption> HttpOptions { get;  } = new List<HttpOption>();

  


        #region ctor

        public InMemoryHttpOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion



        #region Methode

        public HttpOption GetById(int id)
        {
            var findItem = HttpOptions.FirstOrDefault(t => t.Id == id);
            return findItem;
        }


        public async Task<HttpOption> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return GetById(id);
        }


        public HttpOption GetSingle(Expression<Func<HttpOption, bool>> predicate)
        {
            var findItem = HttpOptions.FirstOrDefault(predicate.Compile());
            return findItem;
        }

        public async Task<HttpOption> GetSingleAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return GetSingle(predicate);
        }

        public IEnumerable<HttpOption> GetWithInclude(params Expression<Func<HttpOption, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<HttpOption> List()
        {
            return HttpOptions;
        }


        public IEnumerable<HttpOption> List(Expression<Func<HttpOption, bool>> predicate)
        {
            return HttpOptions.Where(predicate.Compile());
        }


        public async Task<IEnumerable<HttpOption>> ListAsync()
        {
           await Task.CompletedTask;
           return List();
        }


        public async Task<IEnumerable<HttpOption>> ListAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return List(predicate);
        }


        public int Count(Expression<Func<HttpOption, bool>> predicate)
        {
            return HttpOptions.Count(predicate.Compile());
        }


        public async Task<int> CountAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return HttpOptions.Count(predicate.Compile());
        }


        public void Add(HttpOption entity)
        {
            entity.Id = CalcMaxId() + 1;
            HttpOptions.Add(entity);
        }


        public async Task AddAsync(HttpOption entity)
        {
            await Task.CompletedTask;
            Add(entity);
        }


        public void AddRange(IEnumerable<HttpOption> entitys)
        {
            var maxId = CalcMaxId();
            foreach (var entity in entitys)
            {
                entity.Id = ++maxId;
            }
            HttpOptions.AddRange(entitys);
        }


        public async Task AddRangeAsync(IEnumerable<HttpOption> entitys)
        {
            await Task.CompletedTask;;
            AddRange(entitys);
        }


        public void Delete(HttpOption entity)
        {
            HttpOptions.Remove(entity);
        }


        public void Delete(Expression<Func<HttpOption, bool>> predicate)
        {
            HttpOptions.RemoveAll(predicate.Compile().Invoke);
        }


        public async Task DeleteAsync(HttpOption entity)
        {
            await Task.CompletedTask;
            Delete(entity);
        }


        public async Task DeleteAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            Delete(predicate);
        }


        public void Edit(HttpOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = HttpOptions.IndexOf(findItem);
                HttpOptions[index] = entity;
            }
        }


        public async Task EditAsync(HttpOption entity)
        {
            await Task.CompletedTask;
            Edit(entity);
        }


        public bool IsExist(Expression<Func<HttpOption, bool>> predicate)
        {
            return HttpOptions.FirstOrDefault(predicate.Compile()) != null;
        }


        public async Task<bool> IsExistAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return IsExist(predicate);
        }


        private int CalcMaxId()
        {
            var maxId = HttpOptions.Any() ? HttpOptions.Max(d => d.Id) : 0;
            return maxId;
        }
        #endregion
    }
}