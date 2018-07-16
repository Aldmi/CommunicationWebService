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
        private List<SerialOption> Serials { get;  } = new List<SerialOption>();

  


        #region ctor

        public InMemorySerialPortOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion




        #region Methode

        public SerialOption GetById(int id)
        {
            var findItem = Serials.FirstOrDefault(t => t.Id == id);
            return findItem;
        }


        public IEnumerable<SerialOption> List()
        {
            return Serials;
        }


        public IEnumerable<SerialOption> List(Expression<Func<SerialOption, bool>> predicate)
        {
            return Serials.Where(predicate.Compile());
        }


        public void Add(SerialOption entity)
        {
            Serials.Add(entity);
        }


        public void AddRange(IEnumerable<SerialOption> entitys)
        {
            Serials.AddRange(entitys);
        }


        public void Delete(SerialOption entity)
        {
            Serials.Remove(entity);
        }


        public void Delete(Expression<Func<SerialOption, bool>> predicate)
        {
            Serials.RemoveAll(predicate.Compile().Invoke);
        }


        public void Edit(SerialOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = Serials.IndexOf(findItem);
                Serials[index] = entity;
            }
        }

        #endregion
    }
}