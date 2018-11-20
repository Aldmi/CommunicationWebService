using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.EFCore.Entities.Exchange;
using DAL.EFCore.Mappers;

namespace DAL.EFCore.Repository
{
    public class EfExchangeOptionRepository : EfBaseRepository<EfExchangeOption, ExchangeOption>, IExchangeOptionRepository
    {
        #region ctor

        public EfExchangeOptionRepository(string connectionString) : base(connectionString)
        {
        }

        #endregion



        #region CRUD

        public new ExchangeOption GetById(int id)
        {
            return base.GetById(id);
        }


        public new async Task<ExchangeOption> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }


        public new ExchangeOption GetSingle(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return base.GetSingle(predicate);
        }


        public new async Task<ExchangeOption> GetSingleAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return await base.GetSingleAsync(predicate);
        }


        public new IEnumerable<ExchangeOption> List()
        {
            return base.List();
        }


        public new IEnumerable<ExchangeOption> List(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return base.List(predicate);
        }


        public new async Task<IEnumerable<ExchangeOption>> ListAsync()
        {
            return await base.ListAsync();
        }


        public new async Task<IEnumerable<ExchangeOption>> ListAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return await base.ListAsync(predicate);
        }


        public new int Count(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return base.Count(predicate);
        }


        public new async Task<int> CountAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return await base.CountAsync(predicate);
        }


        public new void Add(ExchangeOption entity)
        {
            base.Add(entity);
        }


        public new async Task AddAsync(ExchangeOption entity)
        {
            await base.AddAsync(entity);
        }


        public new void AddRange(IEnumerable<ExchangeOption> entitys)
        {
            base.AddRange(entitys);
        }


        public new async Task AddRangeAsync(IEnumerable<ExchangeOption> entitys)
        {
            await base.AddRangeAsync(entitys);
        }


        public new void Delete(ExchangeOption entity)
        {
            base.Delete(entity);
        }


        public new void Delete(Expression<Func<ExchangeOption, bool>> predicate)
        {
            base.Delete(predicate);
        }


        public new async Task DeleteAsync(ExchangeOption entity)
        {
            await base.DeleteAsync(entity);
        }


        public new async Task DeleteAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            await base.DeleteAsync(predicate);
        }


        public new void Edit(ExchangeOption entity)
        {
            base.Edit(entity);
        }


        public new async Task EditAsync(ExchangeOption entity)
        {
            await base.EditAsync(entity);
        }


        public new bool IsExist(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return base.IsExist(predicate);
        }


        public new async Task<bool> IsExistAsync(Expression<Func<ExchangeOption, bool>> predicate)
        {
            return await base.IsExistAsync(predicate);
        }

        #endregion
    }
}