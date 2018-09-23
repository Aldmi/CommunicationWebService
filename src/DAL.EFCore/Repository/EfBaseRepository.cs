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
    /// <summary>
    /// Базовый тип репозитория для EntitiFramework
    /// </summary>
    /// <typeparam name="TDb">Тип в системе хранения</typeparam>
    /// <typeparam name="TMap">Тип в бизнесс логики</typeparam>
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


        protected async Task<TMap> GetByIdAsync(int id)
        {
            var efSpOption = await DbSet.FindAsync(id);
            var spOptions = AutoMapperConfig.Mapper.Map<TMap>(efSpOption);
            return spOptions;
        }


        protected TMap GetSingle(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efSpOption = DbSet.SingleOrDefault(efPredicate);
            var spOption = AutoMapperConfig.Mapper.Map<TMap>(efSpOption);
            return spOption;
        }


        protected async Task<TMap> GetSingleAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efSpOption = await DbSet.SingleOrDefaultAsync(efPredicate);
            var spOption = AutoMapperConfig.Mapper.Map<TMap>(efSpOption);
            return spOption;
        }

        //TODO: Отладить!!!!  using: (IEnumerable<Phone> phones = phoneRepo.GetWithInclude(p=>p.Company);)
        public IEnumerable<TMap> GetWithInclude(params Expression<Func<TMap, object>>[] includeProperties)
        {
            var list = new List<Expression<Func<TDb, object>>>();
            foreach (var includeProperty in includeProperties)
            {
                var efIncludeProperty = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, object>>>(includeProperty);
                list.Add(efIncludeProperty);
            }
            var result = Include(list.ToArray()).ToList();
            return AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(result);
        }


        protected IEnumerable<TMap> List()
        {
            var efOptions = DbSet.ToList();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        protected IEnumerable<TMap> List(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = DbSet.Where(efPredicate).ToList();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        protected async Task<IEnumerable<TMap>> ListAsync()
        {
            var efOptions = await DbSet.ToListAsync();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        protected async Task<IEnumerable<TMap>> ListAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = await DbSet.Where(efPredicate).ToListAsync();
            var spOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TMap>>(efOptions);
            return spOptions;
        }


        protected int Count(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return DbSet.Count(efPredicate);
        }


        protected async Task<int> CountAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return await DbSet.CountAsync(efPredicate);
        }


        protected void Add(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            DbSet.Add(efOptions);
            Context.SaveChanges();
        }


        protected async Task AddAsync(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            await DbSet.AddAsync(efOptions);
            await Context.SaveChangesAsync();
        }


        protected void AddRange(IEnumerable<TMap> entitys)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<IEnumerable<TDb>>(entitys);
            DbSet.AddRange(efOptions);
            Context.SaveChanges();
        }


        protected async Task AddRangeAsync(IEnumerable<TMap> entitys)
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


        protected void Delete(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            DbSet.Remove(efOptions);
            Context.SaveChanges();
        }


        protected void Delete(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = DbSet.Where(efPredicate).ToList();
            DbSet.RemoveRange(efOptions);
            Context.SaveChanges();
        }


        protected async Task DeleteAsync(TMap entity)
        {
            var efOptions = AutoMapperConfig.Mapper.Map<TDb>(entity);
            DbSet.Remove(efOptions);
            await Context.SaveChangesAsync();
        }


        protected async Task DeleteAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            var efOptions = await DbSet.Where(efPredicate).ToListAsync();
            DbSet.RemoveRange(efOptions);
            await Context.SaveChangesAsync();
        }


        protected void Edit(TMap entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }


        protected async Task EditAsync(TMap entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }


        protected bool IsExist(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return DbSet.Any(efPredicate);
        }


        protected async Task<bool> IsExistAsync(Expression<Func<TMap, bool>> predicate)
        {
            var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<TDb, bool>>>(predicate);
            return await DbSet.AnyAsync(efPredicate);
        }

        #endregion




        #region Methode

        private IQueryable<TDb> Include(params Expression<Func<TDb, object>>[] includeProperties)
        {
            IQueryable<TDb> query = DbSet.AsNoTracking();
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
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