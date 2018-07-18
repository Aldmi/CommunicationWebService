using System.Collections.Generic;
using Exchange.Base;
using Shared.Enums;
using Shared.Types;

namespace BL.Services
{
    public class ExchangeCollectionService
    {
        #region prop

        private Dictionary<KeyTransport, IExchange> ExchangeDict { get; } = new Dictionary<KeyTransport, IExchange>();
        public IEnumerable<IExchange> Exchanges => ExchangeDict.Values;

        #endregion




        #region Methode

        public DictionaryCrudResult AddNew(KeyTransport key, IExchange exchange)
        {
            if (ExchangeDict.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }
            ExchangeDict.Add(key, exchange);
            return DictionaryCrudResult.Added;
        }


        public IExchange Get(KeyTransport keyTransport)
        {
            return ExchangeDict[keyTransport];
        }

        #endregion

    }
}