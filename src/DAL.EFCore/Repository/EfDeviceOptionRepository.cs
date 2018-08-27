using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.Extensions.ExpressionMapping;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Device;
using DAL.EFCore.DbContext;
using DAL.EFCore.Entities.Device;
using DAL.EFCore.Entities.Transport;
using DAL.EFCore.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DAL.EFCore.Repository
{
    public class EfDeviceOptionRepository : EfBaseRepository, IDeviceOptionRepository
    {

        #region field

        private readonly Context _context;
        private readonly DbSet<EfDeviceOption> _dbSet;

        #endregion




        #region ctor

        public EfDeviceOptionRepository(string connectionString)
        {
            _context = new Context(connectionString);
            _dbSet = _context.Set<EfDeviceOption>();
        }

        #endregion




        #region CRUD

        public DeviceOption GetById(int id)
        {
            throw new NotImplementedException();
        }


        public Task<DeviceOption> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


        public DeviceOption GetSingle(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }


        public Task<DeviceOption> GetSingleAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeviceOption> List()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeviceOption> List(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceOption>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceOption>> ListAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }


        public async Task<int> CountAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfDeviceOption, bool>>>(predicate);
            return await _dbSet.CountAsync(efPredicate);
        }


        public void Add(DeviceOption entity)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(DeviceOption entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<DeviceOption> entitys)
        {
            throw new NotImplementedException();
        }


        public async Task AddRangeAsync(IEnumerable<DeviceOption> entitys)
        {
            var efDeviceOptions = AutoMapperConfig.Mapper.Map<IEnumerable<EfDeviceOption>>(entitys);
            await _dbSet.AddRangeAsync(efDeviceOptions);
            await _context.SaveChangesAsync();
        }


        public void Delete(DeviceOption entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(DeviceOption entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Edit(DeviceOption entity)
        {
            throw new NotImplementedException();
        }

        public Task EditAsync(DeviceOption entity)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistAsync(Expression<Func<DeviceOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}