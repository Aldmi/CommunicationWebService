using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Shared.Enums;

namespace BL.Services.Storages
{
    public class StorageServiceBase<TKey, TValue> where TKey : IEquatable<TKey>
                                                  where TValue : class, IDisposable
    {
        #region prop

        private ConcurrentDictionary<TKey, TValue> Storage { get; } = new ConcurrentDictionary<TKey, TValue>();
        public IEnumerable<TValue> Values => Storage.Values;

        #endregion



        #region Methode

        public DictionaryCrudResult AddNew(TKey key, TValue value)
        {
            if (Storage.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }

            return (Storage.TryAdd(key, value)) ? DictionaryCrudResult.Added : DictionaryCrudResult.None;
        }


        public DictionaryCrudResult Remove(TKey key)
        {
            if (!Storage.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyNotExist;
            }
            var sp = Storage[key];
            sp.Dispose();
            return (Storage.TryRemove(key, out sp)) ? DictionaryCrudResult.Removed : DictionaryCrudResult.None; 
        }


        public TValue Get(TKey key)
        {
            if (!Storage.ContainsKey(key))
            {
                return null;
            }
            return Storage.TryGetValue(key, out var res) ? res : null;
        }


        public IEnumerable<TValue> GetMany(IEnumerable<TKey> keys)
        {
            return Storage.Where(item => keys.FirstOrDefault(key => key.Equals(item.Key)) != null).Select(pair => pair.Value);
        }


        public bool IsExist(TKey key)
        {
           return Storage.ContainsKey(key);
        }

        #endregion
    }
}