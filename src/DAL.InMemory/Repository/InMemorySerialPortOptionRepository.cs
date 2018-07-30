using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Transport;

namespace DAL.InMemory.Repository
{
    public class InMemorySerialPortOptionRepository : ISerialPortOptionRepository
    {
        private readonly string _connectionString;
        private List<SerialOption> SerialOptions { get;  } = new List<SerialOption>();

  


        #region ctor

        public InMemorySerialPortOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion




        #region Methode

        public SerialOption GetById(int id)
        {
            var findItem = SerialOptions.FirstOrDefault(t => t.Id == id);
            return findItem;
        }

        public SerialOption GetSingle(Expression<Func<SerialOption, bool>> predicate)
        {
            var findItem = SerialOptions.FirstOrDefault(predicate.Compile());
            return findItem;
        }


        public IEnumerable<SerialOption> List()
        {
            return SerialOptions;
        }


        public IEnumerable<SerialOption> List(Expression<Func<SerialOption, bool>> predicate)
        {
            return SerialOptions.Where(predicate.Compile());
        }


        public void Add(SerialOption entity)
        {
            entity.Id = CalcMaxId() + 1;
            SerialOptions.Add(entity);
        }


        public void AddRange(IEnumerable<SerialOption> entitys)
        {
            var maxId = CalcMaxId();
            foreach (var entity in entitys)
            {
                entity.Id = ++maxId;
            }
            SerialOptions.AddRange(entitys);
        }


        public void Delete(SerialOption entity)
        {
            SerialOptions.Remove(entity);
        }


        public void Delete(Expression<Func<SerialOption, bool>> predicate)
        {
            SerialOptions.RemoveAll(predicate.Compile().Invoke);
        }


        public void Edit(SerialOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = SerialOptions.IndexOf(findItem);
                SerialOptions[index] = entity;
            }
        }


        public bool IsExist(Expression<Func<SerialOption, bool>> predicate)
        {
            return SerialOptions.FirstOrDefault(predicate.Compile()) != null;
        }


        private int CalcMaxId()
        {
            var maxId = SerialOptions.Any() ? SerialOptions.Max(d => d.Id) : 0;
            return maxId;
        }


        #endregion
    }
}