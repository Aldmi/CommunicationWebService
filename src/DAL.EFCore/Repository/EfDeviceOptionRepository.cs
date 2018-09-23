using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Device;
using DAL.EFCore.Entities.Device;

namespace DAL.EFCore.Repository
{
    public class EfDeviceOptionRepository : EfBaseRepository<EfDeviceOption, DeviceOption>, IDeviceOptionRepository
    {
        #region ctor

        public EfDeviceOptionRepository(string connectionString) : base(connectionString)
        {
        }

        #endregion



        #region CRUD

        public new DeviceOption GetById(int id)
        {
            return base.GetById(id);
        }


        public new async Task<DeviceOption> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }


        public new DeviceOption GetSingle(Expression<Func<DeviceOption, bool>> predicate)
        {
            return base.GetSingle(predicate);
        }


        public new async Task<DeviceOption> GetSingleAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            return await base.GetSingleAsync(predicate);
        }


        public new IEnumerable<DeviceOption> GetWithInclude(params Expression<Func<DeviceOption, object>>[] includeProperties)
        {
            return base.GetWithInclude(includeProperties);
        }


        public new IEnumerable<DeviceOption> List()
        {
            return base.List();
        }


        public new IEnumerable<DeviceOption> List(Expression<Func<DeviceOption, bool>> predicate)
        {
            return base.List(predicate);
        }


        public new async Task<IEnumerable<DeviceOption>> ListAsync()
        {
            return await base.ListAsync();
        }


        public new async Task<IEnumerable<DeviceOption>> ListAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            return await base.ListAsync(predicate);
        }


        public new int Count(Expression<Func<DeviceOption, bool>> predicate)
        {
            return base.Count(predicate);
        }


        public new async Task<int> CountAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            return await base.CountAsync(predicate);
        }


        public new void Add(DeviceOption entity)
        {
            base.Add(entity);
        }


        public new async Task AddAsync(DeviceOption entity)
        {
            await base.AddAsync(entity);
        }


        public new void AddRange(IEnumerable<DeviceOption> entitys)
        {
            base.AddRange(entitys);
        }


        public new async Task AddRangeAsync(IEnumerable<DeviceOption> entitys)
        {
            await base.AddRangeAsync(entitys);
        }


        public new void Delete(DeviceOption entity)
        {
            base.Delete(entity);
        }


        public new void Delete(Expression<Func<DeviceOption, bool>> predicate)
        {
            base.Delete(predicate);
        }


        public new async Task DeleteAsync(DeviceOption entity)
        {
            await base.DeleteAsync(entity);
        }


        public new async Task DeleteAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            await base.DeleteAsync(predicate);
        }


        public new void Edit(DeviceOption entity)
        {
            base.Edit(entity);
        }


        public new async Task EditAsync(DeviceOption entity)
        {
            await base.EditAsync(entity);
        }


        public new bool IsExist(Expression<Func<DeviceOption, bool>> predicate)
        {
            return base.IsExist(predicate);
        }


        public new async Task<bool> IsExistAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            return await base.IsExistAsync(predicate);
        }

        #endregion
    }
}