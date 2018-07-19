using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Device;

namespace DAL.InMemory.Repository
{
    public class InMemoryDeviceOptionRepository : IDeviceOptionRepository
    {
        private readonly string _connectionString;
        private List<DeviceOption> DeviceOptions { get; } = new List<DeviceOption>();



        #region ctor

        public InMemoryDeviceOptionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion




        #region Methode

        public DeviceOption GetById(int id)
        {
            var findItem = DeviceOptions.FirstOrDefault(t => t.Id == id);
            return findItem;
        }


        public IEnumerable<DeviceOption> List()
        {
            return DeviceOptions;
        }


        public IEnumerable<DeviceOption> List(Expression<Func<DeviceOption, bool>> predicate)
        {
            return DeviceOptions.Where(predicate.Compile());
        }


        public void Add(DeviceOption entity)
        {
            DeviceOptions.Add(entity);
        }


        public void AddRange(IEnumerable<DeviceOption> entitys)
        {
            DeviceOptions.AddRange(entitys);
        }


        public void Delete(DeviceOption entity)
        {
            DeviceOptions.Remove(entity);
        }


        public void Delete(Expression<Func<DeviceOption, bool>> predicate)
        {
            DeviceOptions.RemoveAll(predicate.Compile().Invoke);
        }


        public void Edit(DeviceOption entity)
        {
            var findItem = GetById(entity.Id);
            if (findItem != null)
            {
                var index = DeviceOptions.IndexOf(findItem);
                DeviceOptions[index] = entity;
            }
        }

        #endregion
    }
}