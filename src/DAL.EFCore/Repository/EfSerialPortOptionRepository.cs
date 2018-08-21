using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;
using DAL.EFCore.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DAL.EFCore.Repository
{
    public class EfSerialPortOptionRepository : ISerialPortOptionRepository, IDisposable
    {
        private readonly Context _context;


        #region ctor

        public EfSerialPortOptionRepository(string connStr)
        {
            _context = new Context(connStr);
        }

        #endregion



        #region CRUD

        public SerialOption GetById(int id)
        {
           var efSpOption= _context.EfSerialPortOptions.Find(id);
            //map 2 spOption
            return null;
        }


        public async Task<SerialOption> GetByIdAsync(int id)
        {
            var efSpOption = await _context.EfSerialPortOptions.FindAsync(id);
            //map 2 spOption
            return null;
        }


        public SerialOption GetSingle(Expression<Func<SerialOption, bool>> predicate)
        {
            var efPredicate = predicate.Compile() as Func<EfSerialPortOptionRepository, bool>;
            //var efSpOption = _context.EfSerialPortOptions.SingleOrDefault(efPredicate);
            //map 2 spOption
            return null;
        }


        public Task<SerialOption> GetSingleAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<SerialOption> List()
        {
           var efSpOptions= _context.EfSerialPortOptions.ToList();
            //map 2 spOption
            return null;
        }


        public  IEnumerable<SerialOption> List(Expression<Func<SerialOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }



        public async Task<IEnumerable<SerialOption>> ListAsync()
        {
            var efSpOptions = await _context.EfSerialPortOptions.ToListAsync();
            //map 2 spOption
            return null;
        }


        public Task<IEnumerable<SerialOption>> ListAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }


        public void Add(SerialOption entity)
        {
            //map 2 EfSpOption
            //_context.EfSerialPortOptions.Add(entity);
        }


        public Task AddAsync(SerialOption entity)
        {
            throw new NotImplementedException();
        }


        public void AddRange(IEnumerable<SerialOption> entitys)
        {
            throw new NotImplementedException();
        }


        public Task AddRangeAsync(IEnumerable<SerialOption> entitys)
        {
            throw new NotImplementedException();
        }

        public void Delete(SerialOption entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Expression<Func<SerialOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(SerialOption entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Edit(SerialOption entity)
        {
            throw new NotImplementedException();
        }

        public Task EditAsync(SerialOption entity)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(Expression<Func<SerialOption, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistAsync(Expression<Func<SerialOption, bool>> predicate)
        {
            throw new NotImplementedException();
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