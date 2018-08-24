using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.Extensions.ExpressionMapping;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;
using DAL.EFCore.DbContext;
using DAL.EFCore.Entities;
using DAL.EFCore.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DAL.EFCore.Repository
{
    public class EfSerialPortOptionRepository : EfBaseRepository, ISerialPortOptionRepository, IDisposable
    {
        #region field

        private readonly Context _context;
        private readonly DbSet<EfSerialOption> _dbSet;

        #endregion




        #region ctor

        public EfSerialPortOptionRepository(string connectionString)
        {
            _context = new Context(connectionString);
            _dbSet= _context.Set<EfSerialOption>();
        }

        #endregion





        #region CRUD

        public SerialOption GetById(int id)
        {
           var efSpOption= _dbSet.Find(id);
           var spOptions = AutoMapperConfig.Mapper.Map<SerialOption>(efSpOption);
           return spOptions;
        }


        public async Task<SerialOption> GetByIdAsync(int id)
        {
            var efSpOption = await _dbSet.FindAsync(id);
            var spOptions = AutoMapperConfig.Mapper.Map<SerialOption>(efSpOption);
            return spOptions;
        }


        public SerialOption GetSingle(Expression<Func<SerialOption, bool>> predicate)
        {         
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            var efSpOption = _dbSet.SingleOrDefault(efPredicate);
            var spOption= AutoMapperConfig.Mapper.Map<SerialOption>(efSpOption);
            return spOption;
        }


        public async Task<SerialOption> GetSingleAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            var efSpOption = await _dbSet.SingleOrDefaultAsync(efPredicate);
            var spOption= AutoMapperConfig.Mapper.Map<SerialOption>(efSpOption);
            return spOption;
        }


        public IEnumerable<SerialOption> List()
        {
           var efSpOptions= _dbSet.ToList();
           var spOptions= AutoMapperConfig.Mapper.Map<IEnumerable<SerialOption>>(efSpOptions);
           return spOptions;
        }


        public  IEnumerable<SerialOption> List(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            var efSpOptions= _dbSet.Where(efPredicate).ToList();
            var spOptions= AutoMapperConfig.Mapper.Map<IEnumerable<SerialOption>>(efSpOptions);
            return spOptions;
        }


        public async Task<IEnumerable<SerialOption>> ListAsync()
        {
            var efSpOptions= await _dbSet.ToListAsync();
            var spOptions= AutoMapperConfig.Mapper.Map<IEnumerable<SerialOption>>(efSpOptions);
            return spOptions;
        }


        public async Task<IEnumerable<SerialOption>> ListAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            var efSpOptions= await _dbSet.Where(efPredicate).ToListAsync();
            var spOptions= AutoMapperConfig.Mapper.Map<IEnumerable<SerialOption>>(efSpOptions);
            return spOptions;
        }


        public int Count(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            return _dbSet.Count(efPredicate);
        }


        public async Task<int> CountAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            return await _dbSet.CountAsync(efPredicate);
        }


        public void Add(SerialOption entity)
        {
            var efSpOptions= AutoMapperConfig.Mapper.Map<EfSerialOption>(entity);
            _dbSet.Add(efSpOptions);
            _context.SaveChanges();
        }


        public async Task AddAsync(SerialOption entity)
        {
            var efSpOptions= AutoMapperConfig.Mapper.Map<EfSerialOption>(entity);
            await _dbSet.AddAsync(efSpOptions);
            await _context.SaveChangesAsync();
        }


        public void AddRange(IEnumerable<SerialOption> entitys)
        {
            var efSpOptions=  AutoMapperConfig.Mapper.Map<IEnumerable<EfSerialOption>>(entitys);
            _dbSet.AddRange(efSpOptions);
            _context.SaveChanges();
        }


        public async Task AddRangeAsync(IEnumerable<SerialOption> entitys)
        {
            var efSpOptions=  AutoMapperConfig.Mapper.Map<IEnumerable<EfSerialOption>>(entitys);
            await _dbSet.AddRangeAsync(efSpOptions);
            await _context.SaveChangesAsync();
        }


        public void Delete(SerialOption entity)
        {
            var efSpOptions= AutoMapperConfig.Mapper.Map<EfSerialOption>(entity);
            _dbSet.Remove(efSpOptions);
            _context.SaveChanges();
        }


        public void Delete(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            var efSpOptions= _dbSet.Where(efPredicate).ToList();
            _dbSet.RemoveRange(efSpOptions);
            _context.SaveChanges();
        }


        public async Task DeleteAsync(SerialOption entity)
        {
           var efSpOptions= AutoMapperConfig.Mapper.Map<EfSerialOption>(entity);
           _dbSet.Remove(efSpOptions);
           await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            var efSpOptions= await _dbSet.Where(efPredicate).ToListAsync();
            _dbSet.RemoveRange(efSpOptions);
            await _context.SaveChangesAsync();
        }


        public void Edit(SerialOption entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }


        public async Task EditAsync(SerialOption entity)
        {
           _context.Entry(entity).State = EntityState.Modified;
           await _context.SaveChangesAsync();
        }


        public bool IsExist(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            return _dbSet.Any(efPredicate); 
        }


        public async Task<bool> IsExistAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
            return await _dbSet.AnyAsync(efPredicate); 
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            _context?.Dispose();
        }

        #endregion
    }
}