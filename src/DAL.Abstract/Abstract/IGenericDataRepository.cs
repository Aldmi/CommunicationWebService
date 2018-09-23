using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.Abstract.Abstract
{
    public interface IGenericDataRepository<T>
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);

        T GetSingle(Expression<Func<T, bool>> predicate);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetWithInclude(params Expression<Func<T, object>>[] includeProperties); //?????

        IEnumerable<T> List();
        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> ListAsync();
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate);

        int Count(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        void Add(T entity);
        Task AddAsync(T entity);

        void AddRange(IEnumerable<T> entitys); 
        Task AddRangeAsync(IEnumerable<T> entitys); 

        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        Task DeleteAsync(T entity);
        Task DeleteAsync(Expression<Func<T, bool>> predicate);

        void Edit(T entity);
        Task EditAsync(T entity);

        bool IsExist(Expression<Func<T, bool>> predicate);
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate);
    }
}