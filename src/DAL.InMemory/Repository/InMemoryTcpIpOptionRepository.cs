using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.InMemory.Repository
{
    public class InMemoryTcpIpOptionRepository : ITcpIpOptionRepository
    {
        private readonly string _connectionString;
        private List<TcpIpOption> TcpIpOptions { get;  } = new List<TcpIpOption>();

  


        #region ctor

        public InMemoryTcpIpOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion




        #region Methode

        public TcpIpOption GetById(int id)
        {
            var findItem = TcpIpOptions.FirstOrDefault(t => t.Id == id);
            return findItem;
        }


        public async Task<TcpIpOption> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return GetById(id);
        }


        public TcpIpOption GetSingle(Expression<Func<TcpIpOption, bool>> predicate)
        {
            var findItem = TcpIpOptions.FirstOrDefault(predicate.Compile());
            return findItem;
        }


        public async Task<TcpIpOption> GetSingleAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return GetSingle(predicate);
        }

        public IEnumerable<TcpIpOption> GetWithInclude(params Expression<Func<TcpIpOption, object>>[] includeProperties)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<TcpIpOption> List()
        {
            return TcpIpOptions;
        }


        public IEnumerable<TcpIpOption> List(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return TcpIpOptions.Where(predicate.Compile());
        }


        public async Task<IEnumerable<TcpIpOption>> ListAsync()
        {
            await Task.CompletedTask;
            return List();
        }


        public async Task<IEnumerable<TcpIpOption>> ListAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return List(predicate);
        }


        public int Count(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return TcpIpOptions.Count(predicate.Compile());
        }


        public async Task<int> CountAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return TcpIpOptions.Count(predicate.Compile());
        }


        public void Add(TcpIpOption entity)
        {
            entity.Id = CalcMaxId() + 1;
            TcpIpOptions.Add(entity);
        }


        public async Task AddAsync(TcpIpOption entity)
        {
            await Task.CompletedTask;
            Add(entity);
        }


        public void AddRange(IEnumerable<TcpIpOption> entitys)
        {
            var maxId = CalcMaxId();
            foreach (var entity in entitys)
            {
                entity.Id = ++maxId;
            }
            TcpIpOptions.AddRange(entitys);
        }


        public async Task AddRangeAsync(IEnumerable<TcpIpOption> entitys)
        {
            await Task.CompletedTask;
            AddRange(entitys);
        }


        public void Delete(TcpIpOption entity)
        {
            TcpIpOptions.Remove(entity);
        }


        public void Delete(Expression<Func<TcpIpOption, bool>> predicate)
        {
            TcpIpOptions.RemoveAll(predicate.Compile().Invoke);
        }


        public async Task DeleteAsync(TcpIpOption entity)
        {
            await Task.CompletedTask;
            Delete(entity);
        }


        public async Task DeleteAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            Delete(predicate);
        }


        public void Edit(TcpIpOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = TcpIpOptions.IndexOf(findItem);
                TcpIpOptions[index] = entity;
            }
        }


        public async Task EditAsync(TcpIpOption entity)
        {
            await Task.CompletedTask;
            Edit(entity);
        }


        public bool IsExist(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return TcpIpOptions.FirstOrDefault(predicate.Compile()) != null;
        }


        public async Task<bool> IsExistAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            await Task.CompletedTask;
            return IsExist(predicate);
        }


        private int CalcMaxId()
        {
            var maxId = TcpIpOptions.Any() ? TcpIpOptions.Max(d => d.Id) : 0;
            return maxId;
        }

        #endregion
    }
}