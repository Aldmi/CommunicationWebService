using System;
using System.IO;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange.ProvidersOption;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using InputDataModel.Autodictor.Model;

namespace InputDataModel.Autodictor.DataProviders.ManualDataProviders
{
    public class VidorBinaryDataProvider : BaseDataProvider, IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>
    {
        #region field

        private readonly ManualProviderOption _providerOption;

        #endregion




        #region ctor

        public VidorBinaryDataProvider(ProviderOption providerOption)
        {
            _providerOption = providerOption.ManualProviderOption;
            if(_providerOption == null)
                throw new ArgumentNullException(providerOption.Name);
        }

        #endregion


        //public VidorBinaryDataProvider()
        //{
        //    ProviderName = "VidorBinaryDataProvider";
        //}

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
        public Subject<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>> RaiseSendDataRx { get; }

        public void SendDataIsCompleted()
        {
            throw new NotImplementedException();
        }

        public InDataWrapper<AdInputType> InputData { get; set; }
        public ResponseDataItem<AdInputType> OutputData { get; set; }
        public bool IsOutDataValid { get; }
        public Subject<ResponseDataItem<AdInputType>> OutputDataChangeRx { get; }
        public string ProviderName { get; set; }
        public StringBuilder StatusString { get; set; }
        public string Message { get; }


        public Task<int> StartExchangePipeline(InDataWrapper<AdInputType> inData)
        {
            throw new NotImplementedException();
        }

        public int TimeRespone { get; }
        public CancellationTokenSource Cts { get; set; }


    }
}