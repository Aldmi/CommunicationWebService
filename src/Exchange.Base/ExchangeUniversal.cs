﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Exchange;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using InputDataModel.Base;
using Shared.Types;
using Transport.Base.Abstract;
using Transport.Base.RxModel;
using Worker.Background.Abstarct;


namespace Exchange.Base
{
    public class ExchangeUniversal<TIn> : IExchange<TIn>
    {
        #region field
        protected readonly ExchangeOption ExchangeOption;
        private readonly ITransport _transport;
        private readonly ITransportBackground _transportBackground;
        private readonly IExchangeDataProvider<TIn, TransportResponse> _dataProvider;
        #endregion



        #region prop

 
        public string KeyExchange => ExchangeOption.Key;
        public bool AutoStart => ExchangeOption.AutoStartCycleFunc;
        public KeyTransport KeyTransport => ExchangeOption.KeyTransport;
        public bool IsOpen => _transport.IsOpen;
        public bool IsConnect { get; }
        public InDataWrapper<TIn> LastSendData { get; private set; }
        public IEnumerable<string> GetRuleNames => new List<string>(); //TODO: сейчас в ExchangeMasterSpOption только 1 ExchangeRule, должно быть список.
        public bool IsStartedCycleExchange { get; set; }
        protected ConcurrentQueue<InDataWrapper<TIn>> InDataQueue { get; set; } = new ConcurrentQueue<InDataWrapper<TIn>>(); //Очередь данных для SendOneTimeData().
        protected InDataWrapper<TIn> Data4CycleFunc { get; set; }                                                           //Данные для Цикл. функции.

        #endregion



        #region ctor

        public ExchangeUniversal(ExchangeOption exchangeOption, ITransport transport, ITransportBackground transportBackground, IExchangeDataProvider<TIn, TransportResponse> dataProvider)
        {
            ExchangeOption = exchangeOption;

            _transport = transport;
            _transportBackground = transportBackground;
            _dataProvider = dataProvider;
        }

        #endregion



        #region RxEvent

        public ISubject<IExchange<TIn>> IsDataExchangeSuccessChangeRx { get; } //TODO: Добавить событие обмена
        public ISubject<IExchange<TIn>> IsConnectChangeRx { get; }
        public ISubject<IExchange<TIn>> LastSendDataChangeRx { get; }

        public ISubject<IsOpenChangeRxModel> IsOpenChangeTransportRx => _transport.IsOpenChangeRx;
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeTransportRx => _transport.StatusDataExchangeChangeRx;
        public ISubject<StatusStringChangeRxModel> StatusStringChangeTransportRx => _transport.StatusStringChangeRx;

        ISubject<IExchange<TIn>> IExchange<TIn>.IsDataExchangeSuccessChangeRx => throw new NotImplementedException();

        ISubject<IExchange<TIn>> IExchange<TIn>.IsConnectChangeRx => throw new NotImplementedException();

        ISubject<IExchange<TIn>> IExchange<TIn>.LastSendDataChangeRx => throw new NotImplementedException();

        #endregion




        #region Methode

        #region ReOpen

        /// <summary>
        /// Циклическое 
        /// </summary>
        private CancellationTokenSource _cycleReOpenedCts;
        public async Task CycleReOpened()
        {
            if (_transport != null)
            {
                _cycleReOpenedCts?.Cancel();
                _cycleReOpenedCts = new CancellationTokenSource();
                await Task.Factory.StartNew(async () =>
                {
                    if (await _transport.CycleReOpened())
                    {
                        StartCycleExchange();
                    }
                }, _cycleReOpenedCts.Token);
            }
        }


        public void CycleReOpenedCancelation()
        {
            if (!IsOpen)
            {
                _transport.CycleReOpenedCancelation();
                _cycleReOpenedCts?.Cancel();
            }
        }

        #endregion


        #region CycleExchange

        /// <summary>
        /// Добавление ЦИКЛ. функций на БГ
        /// </summary>
        public void StartCycleExchange()
        {
            _transportBackground.AddCycleAction(CycleTimeActionAsync);
            IsStartedCycleExchange = true;
        }


        /// <summary>
        /// Удаление ЦИКЛ. функций из БГ
        /// </summary>
        public void StopCycleExchange()
        {
            _transportBackground.RemoveCycleFunc(CycleTimeActionAsync);
            IsStartedCycleExchange = false;
        }

        #endregion


        #region SendData

        /// <summary>
        /// Отправить команду. аналог однократно выставляемой функции.
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(Command4Device command)
        {
            if (command != Command4Device.None)
            {
                var dataWrapper = new InDataWrapper<TIn> { Command = command};
                InDataQueue.Enqueue(dataWrapper);
                _transportBackground.AddOneTimeAction(OneTimeActionAsync);
            }
        }


        /// <summary>
        /// Отправить данные для однократно выставляемой функции.
        /// Функция выставляется на БГ.
        /// </summary>
        public void SendOneTimeData(IEnumerable<TIn> inData)
        {
            if (inData != null)
            {
                var dataWrapper= new InDataWrapper<TIn>{Datas = inData.ToList()};
                InDataQueue.Enqueue(dataWrapper);
                _transportBackground.AddOneTimeAction(OneTimeActionAsync);
            }
        }


        /// <summary>
        /// Выставить данные для цикл. функции.
        /// </summary>
        public void SendCycleTimeData(IEnumerable<TIn> inData)
        {
            if (inData != null)
            {
                var dataWrapper = new InDataWrapper<TIn> { Datas = inData.ToList() };
                Data4CycleFunc = dataWrapper;
            }
        }

        #endregion



        #region Actions

        /// <summary>
        /// Однократно вызываемая функция.
        /// </summary>
        protected async Task OneTimeActionAsync(CancellationToken ct)
        {
            if (InDataQueue.TryDequeue(out var inData))
            {
               _dataProvider.InputData = inData;
               var timeRespone = 1000;
               var dataExchangeSuccess = await _transport.DataExchangeAsync(timeRespone, _dataProvider, ct);
               LastSendData = _dataProvider.InputData;

                var valid = _dataProvider.IsOutDataValid;
                var output = _dataProvider.OutputData;
            }
            await Task.Delay(1000, ct);
        }


        /// <summary>
        /// Обработка отправки цикл. даных.
        /// </summary>
        protected async Task CycleTimeActionAsync(CancellationToken ct)
        {
            await Task.Delay(2000, ct);//DEBUG
            //Console.WriteLine($"ExchangeOption.KeyTransport.Key=  {ExchangeOption.KeyTransport.Key}");//DEBUG
        }

        #endregion

        #endregion




        #region Disposable

        public void Dispose()
        {

        }

        #endregion
    }
}