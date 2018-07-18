using System.Collections.Generic;
using Shared.Enums;
using Shared.Types;
using Worker.Background.Abstarct;

namespace BL.Services
{
    public class BackgroundCollectionService
    {
        #region prop

        private Dictionary<KeyTransport, IBackground> BackgroundDict { get; } = new Dictionary<KeyTransport, IBackground>();
        public IEnumerable<IBackground> Backgrounds => BackgroundDict.Values;

        #endregion




        #region Methode

        public DictionaryCrudResult AddNew(KeyTransport key, IBackground background)
        {
            if (BackgroundDict.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }
            BackgroundDict.Add(key, background);
            return DictionaryCrudResult.Added;
        }


        public IBackground Get(KeyTransport keyTransport)
        {
            return BackgroundDict[keyTransport];
        }

        #endregion
    }
}