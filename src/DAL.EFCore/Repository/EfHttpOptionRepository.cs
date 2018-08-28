using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;
using DAL.EFCore.Entities.Transport;

namespace DAL.EFCore.Repository
{
    public class EfHttpOptionRepository : EfBaseRepository<EfHttpOption, HttpOption>, IHttpOptionRepository
    {
        #region ctor

        public EfHttpOptionRepository(string connectionString) : base(connectionString)
        {
        }

        #endregion




        #region CRUD

        public new HttpOption GetById(int id)
        {
            return base.GetById(id);
        }


        public new async Task<HttpOption> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }


        public new HttpOption GetSingle(Expression<Func<HttpOption, bool>> predicate)
        {
            return base.GetSingle(predicate);
        }


        public new async Task<HttpOption> GetSingleAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            return await base.GetSingleAsync(predicate);
        }


        public new IEnumerable<HttpOption> List()
        {
            return base.List();
        }


        public new IEnumerable<HttpOption> List(Expression<Func<HttpOption, bool>> predicate)
        {
            return base.List(predicate);
        }


        public new async Task<IEnumerable<HttpOption>> ListAsync()
        {
            return await base.ListAsync();
        }


        public new async Task<IEnumerable<HttpOption>> ListAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            return await base.ListAsync(predicate);
        }


        public new int Count(Expression<Func<HttpOption, bool>> predicate)
        {
            return base.Count(predicate);
        }


        public new async Task<int> CountAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            return await base.CountAsync(predicate);
        }


        public new void Add(HttpOption entity)
        {
            base.Add(entity);
        }


        public new async Task AddAsync(HttpOption entity)
        {
            await base.AddAsync(entity);
        }


        public new void AddRange(IEnumerable<HttpOption> entitys)
        {
            base.AddRange(entitys);
        }


        public new async Task AddRangeAsync(IEnumerable<HttpOption> entitys)
        {
            await base.AddRangeAsync(entitys);
        }


        public new void Delete(HttpOption entity)
        {
            base.Delete(entity);
        }


        public new void Delete(Expression<Func<HttpOption, bool>> predicate)
        {
            base.Delete(predicate);
        }


        public new async Task DeleteAsync(HttpOption entity)
        {
            await base.DeleteAsync(entity);
        }


        public new async Task DeleteAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            await base.DeleteAsync(predicate);
        }


        public new void Edit(HttpOption entity)
        {
            base.Edit(entity);
        }


        public new async Task EditAsync(HttpOption entity)
        {
            await base.EditAsync(entity);
        }


        public new bool IsExist(Expression<Func<HttpOption, bool>> predicate)
        {
            return base.IsExist(predicate);
        }


        public new async Task<bool> IsExistAsync(Expression<Func<HttpOption, bool>> predicate)
        {
            return await base.IsExistAsync(predicate);
        }

        #endregion
    }
}