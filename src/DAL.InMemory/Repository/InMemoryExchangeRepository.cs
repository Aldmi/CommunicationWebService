using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Exchange;

namespace DAL.InMemory.Repository
{
    public class InMemoryExchangeOptionRepository : IExchangeOptionRepository
    {


        public ExchangeOption GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExchangeOption> List()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExchangeOption> List(Expression<Func<ExchangeOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(ExchangeOption entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<ExchangeOption> entitys)
        {
            throw new NotImplementedException();
        }

        public void Delete(ExchangeOption entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<ExchangeOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Edit(ExchangeOption entity)
        {
            throw new NotImplementedException();
        }
    }
}