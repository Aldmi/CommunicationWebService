using System;
using System.Collections.Generic;
using Shared.Enums;
using Shared.Types;

namespace BL.Services.Storage
{
    public class StorageService<TKey, TValue>  where TValue : class , IDisposable 
    {
        #region prop

        private Dictionary<TKey, TValue> Storage { get;  } = new Dictionary<TKey, TValue>();
        public IEnumerable<TValue> Values => Storage.Values;

        #endregion



        #region Methode

        public DictionaryCrudResult AddNew(TKey key, TValue value)
        {
            if (Storage.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }
            Storage.Add(key, value);
            return DictionaryCrudResult.Added;
        }


        public DictionaryCrudResult Remove(TKey key)
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


        public TValue Get(TKey keyTransport)
        {
            return Storage[keyTransport];
        }

        #endregion

    }
}