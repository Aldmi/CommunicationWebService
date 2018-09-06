﻿using System;
using System.IO;
using System.Reactive.Subjects;
using DAL.Abstract.Entities.Options.Exchange.Providers;
using Exchange.Base.DataProviderAbstract;
using InputDataModel.Autodictor.Model;
using Shared.Types;

namespace InputDataModel.Autodictor.ManualDataProviders
{
    public class VidorBinaryDataProvider : IExchangeDataProvider<AdInputType, TransportResponse>
    {
        private readonly ManualProviderOption _manualProviderOption;



        #region ctor

        public VidorBinaryDataProvider(ProviderOption providerOption)
        {
            _manualProviderOption = providerOption.ManualProviderOption;
            if(_manualProviderOption == null)
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
        public AdInputType InputData { get; set; }
        public TransportResponse OutputData { get; set; }
        public bool IsOutDataValid { get; }
        public Subject<TransportResponse> OutputDataChangeRx { get; }
        public string ProviderName { get; set; }
    }
}