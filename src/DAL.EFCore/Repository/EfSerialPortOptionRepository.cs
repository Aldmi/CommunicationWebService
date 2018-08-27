using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.Extensions.ExpressionMapping;
using DAL.Abstract.Abstract;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;
using DAL.EFCore.Entities.Transport;
using DAL.EFCore.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DAL.EFCore.Repository
{
    public class EfSerialPortOptionRepository : EfBaseRepository<EfSerialOption, SerialOption>, ISerialPortOptionRepository
    {
        #region ctor

        public EfSerialPortOptionRepository(string connectionString) : base(connectionString)
        {
        }

        #endregion




        #region CRUD

        public new SerialOption GetById(int id)
        {
            return base.GetById(id);
        }


        public new async Task<SerialOption> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }


        public new SerialOption GetSingle(Expression<Func<SerialOption, bool>> predicate)
        {
            return base.GetSingle(predicate);
        }


        public new async Task<SerialOption> GetSingleAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            return await base.GetSingleAsync(predicate);
        }


        public new IEnumerable<SerialOption> List()
        {
            return base.List();
        }


        public new IEnumerable<SerialOption> List(Expression<Func<SerialOption, bool>> predicate)
        {
            return base.List(predicate);
        }


        public new async Task<IEnumerable<SerialOption>> ListAsync()
        {
            return await base.ListAsync();
        }


        public new async Task<IEnumerable<SerialOption>> ListAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            return await base.ListAsync(predicate);
        }


        public new int Count(Expression<Func<SerialOption, bool>> predicate)
        {
            return  base.Count(predicate);
        }


        public new async Task<int> CountAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            return await base.CountAsync(predicate);
        }


        public new void Add(SerialOption entity)
        {
            base.Add(entity);
        }


        public new async Task AddAsync(SerialOption entity)
        {
            await base.AddAsync(entity);
        }


        public new void AddRange(IEnumerable<SerialOption> entitys)
        {
            base.AddRange(entitys);
        }


        public new async Task AddRangeAsync(IEnumerable<SerialOption> entitys)
        {
           await base.AddRangeAsync(entitys);
        }


        public new void Delete(SerialOption entity)
        {
            base.Delete(entity);
        }


        public new void Delete(Expression<Func<SerialOption, bool>> predicate)
        {
            base.Delete(predicate);
        }


        public new async Task DeleteAsync(SerialOption entity)
        {
           await base.DeleteAsync(entity);
        }


        public new async Task DeleteAsync(Expression<Func<SerialOption, bool>> predicate)
        {
           await base.DeleteAsync(predicate);
        }


        public new void Edit(SerialOption entity)
        {
            base.Edit(entity);
        }


        public new async Task EditAsync(SerialOption entity)
        {
           await base.EditAsync(entity);
        }


        public new bool IsExist(Expression<Func<SerialOption, bool>> predicate)
        {
            return base.IsExist(predicate);
        }


        public new async Task<bool> IsExistAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            return await base.IsExistAsync(predicate);
        }

        #endregion
    }
}