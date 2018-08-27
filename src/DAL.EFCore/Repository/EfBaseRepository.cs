using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.Extensions.ExpressionMapping;
using DAL.EFCore.DbContext;
using DAL.EFCore.Entities;
using DAL.EFCore.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DAL.EFCore.Repository
{
    public abstract class EfBaseRepository<TDb, TMap> : IDisposable
                                                        where TDb : class, IEntity
                                                        where TMap : class
    {
        #region field

        protected readonly Context Context;
        protected readonly DbSet<TDb> DbSet;

        #endregion




        #region ctor

        protected EfBaseRepository(string connectionString)
        {
            Context = new Context(connectionString);
            DbSet = Context.Set<TDb>();
        }

        static EfBaseRepository()
        {
            AutoMapperConfig.Register();
        }

        #endregion




        #region CRUD

        protected TMap GetById(int id)
        {
            var efSpOption = DbSet.Find(id);
            var spOptions = AutoMapperConfig.Mapper.Map<TMap>(efSpOption);
            return spOptions;
        }


        public async Task<TMap> GetByIdAsync(int id)
        {
            var efSpOption = await DbSet.FindAsync(id);
            var spOptions = AutoMapperConfig.Mapper.Map<TMap>(efSpOption);
            return spOptions;
        }


        public TMap GetSingle(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efSpOption = DbSet.SingleOrDefault(efPredicate);
            var spOption = AutoMapperConfig.Mapper.Map<TMap>(efSpOption);
            return spOption;
        }


        public async Task<TMap> GetSingleAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efSpOption = await DbSet.SingleOrDefaultAsync(efPredicate);
            var spOption = AutoMapperConfig.Mapper.Map<TMap>(efSpOption);
            return spOption;
        }


        public IEnumerable<TMap> List()
        {
            var efOptions = DbSet.ToList();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        public IEnumerable<TMap> List(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = DbSet.Where(efPredicate).ToList();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        public async Task<IEnumerable<TMap>> ListAsync()
        {
            var efOptions = await DbSet.ToListAsync();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        public async Task<IEnumerable<TMap>> ListAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = await DbSet.Where(efPredicate).ToListAsync();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        public int Count(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return DbSet.Count(efPredicate);
        }


        public async Task<int> CountAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return await DbSet.CountAsync(efPredicate);
        }


        public void Add(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            DbSet.Add(efOptions);
            Context.SaveChanges();
        }


        public async Task AddAsync(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            await DbSet.AddAsync(efOptions);
            await Context.SaveChangesAsync();
        }


        public void AddRange(IEnumerable<TMap> entitys)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TDb>>(entitys);
            DbSet.AddRange(efOptions);
            Context.SaveChanges();
        }


        public async Task AddRangeAsync(IEnumerable<TMap> entitys)
        {
            //DEBUG
            try
            {
                var efOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TDb>>(entitys);
                await DbSet.AddRangeAsync(efOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        
        
            await Context.SaveChangesAsync();
        }


        public void Delete(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            DbSet.Remove(efOptions);
            Context.SaveChanges();
        }


        public void Delete(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = DbSet.Where(efPredicate).ToList();
            DbSet.RemoveRange(efOptions);
            Context.SaveChanges();
        }


        public async Task DeleteAsync(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            DbSet.Remove(efOptions);
            await Context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = await DbSet.Where(efPredicate).ToListAsync();
            DbSet.RemoveRange(efOptions);
            await Context.SaveChangesAsync();
        }


        public void Edit(TMap entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }


        public async Task EditAsync(TMap entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }


        public bool IsExist(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return DbSet.Any(efPredicate);
        }


        public async Task<bool> IsExistAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return await DbSet.AnyAsync(efPredicate);
        }



        #endregion




        #region Disposable

        public void Dispose()
        {
            Context?.Dispose();
        }

        #endregion
    }
}