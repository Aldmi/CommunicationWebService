using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Enums;
using Shared.Types;

namespace BL.Services.Storage
{
    public class StorageService<TKey, TValue> where TKey : IEquatable<TKey>
                                              where TValue : class, IDisposable                                        
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


        public TValue Get(TKey key)
        {
            if (!Storage.ContainsKey(key))
            {
                return null;
            }

            return Storage[key];
        }


        public IEnumerable<TValue> GetMany(IEnumerable<TKey> keys)
        {
            return Storage.Where(item => keys.FirstOrDefault(key => key.Equals(item.Key)) != null).Select(pair=> pair.Value);  
        }

        #endregion

    }
}