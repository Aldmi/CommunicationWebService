using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Transport;

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


        public IEnumerable<TcpIpOption> List()
        {
            return TcpIpOptions;
        }


        public IEnumerable<TcpIpOption> List(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return TcpIpOptions.Where(predicate.Compile());
        }


        public void Add(TcpIpOption entity)
        {
            TcpIpOptions.Add(entity);
        }


        public void AddRange(IEnumerable<TcpIpOption> entitys)
        {
            TcpIpOptions.AddRange(entitys);
        }


        public void Delete(TcpIpOption entity)
        {
            TcpIpOptions.Remove(entity);
        }


        public void Delete(Expression<Func<TcpIpOption, bool>> predicate)
        {
            TcpIpOptions.RemoveAll(predicate.Compile().Invoke);
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

        #endregion
    }
}