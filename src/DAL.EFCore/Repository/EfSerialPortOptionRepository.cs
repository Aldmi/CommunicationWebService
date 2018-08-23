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
        private readonly Context _context;


        #region ctor

        public EfSerialPortOptionRepository(string connectionString)
        {
            _context = new Context(connectionString);
        }

        #endregion



        #region CRUD

        public SerialOption GetById(int id)
        {
           var efSpOption= _context.EfSerialPortOptions.Find(id);
           var spOptions = AutoMapperConfig.Mapper.Map<SerialOption>(efSpOption);
           return spOptions;
        }


        public async Task<SerialOption> GetByIdAsync(int id)
        {
            var efSpOption = await _context.EfSerialPortOptions.FindAsync(id);
            var spOptions = AutoMapperConfig.Mapper.Map<SerialOption>(efSpOption);
            return spOptions;
        }


        public SerialOption GetSingle(Expression<Func<SerialOption, bool>> predicate)
        {
            try
            {
                var efPredicate = AutoMapperConfig.Mapper.MapExpression<Expression<Func<EfSerialOption, bool>>>(predicate);
                var efSpOption = _context.EfSerialPortOptions.SingleOrDefault(efPredicate);
                var spOption= AutoMapperConfig.Mapper.Map<SerialOption>(efSpOption);
                return spOption;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
            var efSpOptions=  AutoMapperConfig.Mapper.Map<IEnumerable<EfSerialOption>>(entitys);
            _context.AddRange(efSpOptions);
            _context.SaveChanges();
        }


        public async Task AddRangeAsync(IEnumerable<SerialOption> entitys)
        {
            var efSpOptions=  AutoMapperConfig.Mapper.Map<IEnumerable<EfSerialOption>>(entitys);
            await _context.AddRangeAsync(efSpOptions);
            await _context.SaveChangesAsync();
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
            _context.EfSerialPortOptions.AnyAsync(); //TODO: добавить AnyAsync в репозиторий 

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