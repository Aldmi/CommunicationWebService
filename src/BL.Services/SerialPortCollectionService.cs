using System.Collections.Generic;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Abstract;

namespace BL.Services
{
    /// <summary>
    /// Сервис содержит словарь портов и словарь бекграунда для них. Ключ словаря - TransportType
    /// Сервис предоставдяет методы для добавления/удаления портов и связанного с ним бекграунда, также запуск/останов бекграунда
    /// </summary>
    public class SerialPortCollectionService
    {
        #region prop

        private Dictionary<KeyTransport, ISerailPort> SerialDict { get;  } = new Dictionary<KeyTransport, ISerailPort>();


        #endregion






        #region Methode

        public DictionaryCrudResult AddNew(KeyTransport key, ISerailPort serailPort)
        {
            if (SerialDict.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }
            SerialDict.Add(key, serailPort);
            return DictionaryCrudResult.Added;
        }


        public DictionaryCrudResult Remove(KeyTransport key)
        {
            if (SerialDict.ContainsKey(key))
            {
                return DictionaryCrudResult.KeyAlredyExist;
            }
            var sp = SerialDict[key];
            sp.Dispose();
            SerialDict.Remove(key);             
            return DictionaryCrudResult.Removed;
        }

        #endregion
    }



}