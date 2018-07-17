using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Exchange;

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


        public IEnumerable<ExchangeOption> List()
        {
            return ExchangeOptions;
        }


        public IEnumerable<ExchangeOption> List(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return ExchangeOptions.Where(predicate.Compile());
        }


        public void Add(ExchangeOption entity)
        {
            ExchangeOptions.Add(entity);
        }


        public void AddRange(IEnumerable<ExchangeOption> entitys)
        {
            ExchangeOptions.AddRange(entitys);
        }


        public void Delete(ExchangeOption entity)
        {
            ExchangeOptions.Remove(entity);
        }


        public void Delete(Expression<Func<ExchangeOption, bool>> predicate)
        {
            ExchangeOptions.RemoveAll(predicate.Compile().Invoke);
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

        #endregion
    }
}