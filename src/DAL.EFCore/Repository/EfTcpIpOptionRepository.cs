using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;
using DAL.EFCore.Entities.Transport;

namespace DAL.EFCore.Repository
{
    public class EfTcpIpOptionRepository : EfBaseRepository<EfTcpIpOption, TcpIpOption>, ITcpIpOptionRepository
    {
        #region ctor

        public EfTcpIpOptionRepository(string connectionString) : base(connectionString)
        {
        }

        #endregion




        #region CRUD

        public new TcpIpOption GetById(int id)
        {
            return base.GetById(id);
        }


        public new async Task<TcpIpOption> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }


        public new TcpIpOption GetSingle(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return base.GetSingle(predicate);
        }


        public new async Task<TcpIpOption> GetSingleAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return await base.GetSingleAsync(predicate);
        }


        public new IEnumerable<TcpIpOption> List()
        {
            return base.List();
        }


        public new IEnumerable<TcpIpOption> List(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return base.List(predicate);
        }


        public new async Task<IEnumerable<TcpIpOption>> ListAsync()
        {
            return await base.ListAsync();
        }


        public new async Task<IEnumerable<TcpIpOption>> ListAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return await base.ListAsync(predicate);
        }


        public new int Count(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return base.Count(predicate);
        }


        public new async Task<int> CountAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return await base.CountAsync(predicate);
        }


        public new void Add(TcpIpOption entity)
        {
            base.Add(entity);
        }


        public new async Task AddAsync(TcpIpOption entity)
        {
            await base.AddAsync(entity);
        }


        public new void AddRange(IEnumerable<TcpIpOption> entitys)
        {
            base.AddRange(entitys);
        }


        public new async Task AddRangeAsync(IEnumerable<TcpIpOption> entitys)
        {
            await base.AddRangeAsync(entitys);
        }


        public new void Delete(TcpIpOption entity)
        {
            base.Delete(entity);
        }


        public new void Delete(Expression<Func<TcpIpOption, bool>> predicate)
        {
            base.Delete(predicate);
        }


        public new async Task DeleteAsync(TcpIpOption entity)
        {
            await base.DeleteAsync(entity);
        }


        public new async Task DeleteAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            await base.DeleteAsync(predicate);
        }


        public new void Edit(TcpIpOption entity)
        {
            base.Edit(entity);
        }


        public new async Task EditAsync(TcpIpOption entity)
        {
            await base.EditAsync(entity);
        }


        public new bool IsExist(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return base.IsExist(predicate);
        }


        public new async Task<bool> IsExistAsync(Expression<Func<TcpIpOption, bool>> predicate)
        {
            return await base.IsExistAsync(predicate);
        }

        #endregion
    }
}