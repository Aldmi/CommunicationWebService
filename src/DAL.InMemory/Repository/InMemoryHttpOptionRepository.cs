using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Transport;

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


        public IEnumerable<HttpOption> List()
        {
            return HttpOptions;
        }


        public IEnumerable<HttpOption> List(Expression<Func<HttpOption, bool>> predicate)
        {
            return HttpOptions.Where(predicate.Compile());
        }


        public void Add(HttpOption entity)
        {
            entity.Id = CalcMaxId() + 1;
            HttpOptions.Add(entity);
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


        public void Delete(HttpOption entity)
        {
            HttpOptions.Remove(entity);
        }

        public void Delete(Expression<Func<HttpOption, bool>> predicate)
        {
            HttpOptions.RemoveAll(predicate.Compile().Invoke);
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


        public bool IsExist(Expression<Func<HttpOption, bool>> predicate)
        {
            return HttpOptions.FirstOrDefault(predicate.Compile()) != null;
        }


        private int CalcMaxId()
        {
            var maxId = HttpOptions.Any() ? HttpOptions.Max(d => d.Id) : 0;
            return maxId;
        }
        #endregion
    }
}