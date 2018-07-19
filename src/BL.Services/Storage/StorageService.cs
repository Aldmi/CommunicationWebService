using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Abstract.Entities;
using Shared.Enums;
using Shared.Types;

namespace BL.Services.Storage
{
    public class StorageService<T>  where T : class , IDisposable 
    {
        #region prop

        private Dictionary<KeyTransport, T> Storage { get;  } = new Dictionary<KeyTransport, T>();
        public IEnumerable<T> Values => Storage.Values;

        #endregion



        #region Methode

        public DictionaryCrudResult AddNew(KeyTransport key, T value)
        {
            if (Storage.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }
            Storage.Add(key, value);
            return DictionaryCrudResult.Added;
        }


        public DictionaryCrudResult Remove(KeyTransport key)
        {
            if (Storage.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }
            var sp = Storage[key];
            sp.Dispose();
            Storage.Remove(key);             
            return DictionaryCrudResult.Removed;
        }


        public T Get(KeyTransport keyTransport)
        {
            return Storage[keyTransport];
        }

        //public T Get(int id)
        //{
        //    return Storage.Values.FirstOrDefault(v=>v.Id == id);
        //}

        #endregion

    }
}