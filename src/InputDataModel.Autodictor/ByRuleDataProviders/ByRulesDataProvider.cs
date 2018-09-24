using System;
using System.IO;
using System.Reactive.Subjects;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using InputDataModel.Autodictor.Model;
using Shared.Types;

namespace InputDataModel.Autodictor.ByRuleDataProviders
{
    public class ByRulesDataProvider : IExchangeDataProvider<AdInputType, TransportResponse>
    {

        #region field

        private readonly ByRulesProviderOption _providerOption;

        #endregion




        #region ctor

        public ByRulesDataProvider(ProviderOption providerOption)
        {
            _providerOption = providerOption.ByRulesProviderOption;
            if(_providerOption == null)
                throw new ArgumentNullException(providerOption.Name);
        }

        #endregion




        public byte[] GetDataByte()
        {
            throw new System.NotImplementedException();
        }

        public bool SetDataByte(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public Stream GetStream()
        {
            throw new System.NotImplementedException();
        }

        public bool SetStream(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public string GetString()
        {
            throw new System.NotImplementedException();
        }

        public bool SetString(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public int CountGetDataByte { get; }
        public int CountSetDataByte { get; }
        public InDataWrapper<AdInputType> InputData { get; set; }
        public TransportResponse OutputData { get; set; }
        public bool IsOutDataValid { get; }
        public Subject<TransportResponse> OutputDataChangeRx { get; }
        public string ProviderName { get; set; }
    }
}