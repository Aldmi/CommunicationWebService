using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DAL.Abstract.Entities.Device;

namespace DAL.Abstract.Abstract
{
    public interface IGenericDataRepository<T>
    {
        T GetById(int id);
        T GetSingle(Expression<Func<T, bool>> predicate);
        IEnumerable<T> List();
        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entitys); 
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        void Edit(T entity);
        bool IsExist(Expression<Func<T, bool>> predicate);
    }

    //IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties);
    //IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
    //T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
    //void Add(params T[] items);
    //void Update(params T[] items);
    //void Remove(params T[] items);


}