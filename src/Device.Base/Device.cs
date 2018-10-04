﻿using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Abstract.Entities.Options.Device;
using Exchange.Base;
using Infrastructure.EventBus.Abstract;
using InputDataModel.Base;
using Transport.Base.RxModel;

namespace DeviceForExchange
{
    /// <summary>
    /// Устройство.
    /// Содержит список обменов.
    /// Ответы о своей работе каждое ус-во выставляет самостоятельно на шину данных
    /// </summary>
    public class Device<TIn> : IDisposable
    {
        #region field

        private readonly IEventBus _eventBus;
        private readonly List<IDisposable> _disposeExchangesEventHandlers = new List<IDisposable>();

        #endregion



        #region prop

        public DeviceOption Option { get;  }
        public List<IExchange<TIn>> Exchanges { get; }

        #endregion




        #region ctor

        public Device(DeviceOption option, IEnumerable<IExchange<TIn>> exchanges, IEventBus eventBus)
        {
            Option = option;
            Exchanges = exchanges.ToList();
            _eventBus = eventBus;
        }

        #endregion




        #region Methode

        public void SubscrubeOnExchangesEvents()
        {
            Exchanges.ForEach(exch =>
            {
                _disposeExchangesEventHandlers.Add(exch.IsConnectChangeRx.Subscribe(ConnectChangeRxEventHandler));
                _disposeExchangesEventHandlers.Add(exch.LastSendDataChangeRx.Subscribe(LastSendDataChangeRxEventHandler));
                _disposeExchangesEventHandlers.Add(exch.IsOpenChangeTransportRx.Subscribe(OpenChangeTransportRxEventHandler));
            });
        }


        public void UnsubscrubeOnExchangesEvents()
        {
            _disposeExchangesEventHandlers.ForEach(d=>d.Dispose());
        }


        /// <summary>
        /// Отправить данные или команду на все обмены.
        /// </summary>
        /// <param name="dataAction"></param>
        /// <param name="inData"></param>
        /// <param name="command4Device"></param>
        public void Send2AllExchanges(DataAction dataAction, List<TIn> inData, Command4Device command4Device = Command4Device.None)
        {
            foreach (var exchange in Exchanges)
            {
                SendDataOrCommand(exchange, dataAction, inData, command4Device);
            }  
        }


        /// <summary>
        /// Отправить данные или команду на выбранный обмен.
        /// </summary>
        /// <param name="keyExchange"></param>
        /// <param name="dataAction"></param>
        /// <param name="inData"></param>
        /// <param name="command4Device"></param>
        public void Send2ConcreteExchanges(string keyExchange, DataAction dataAction, List<TIn> inData, Command4Device command4Device = Command4Device.None)
        {
            var exchange = Exchanges.FirstOrDefault(exch=> exch.KeyExchange == keyExchange);
            if (exchange == null)
            {
                //TODO: produser.Send(Option.Name, $"Обмен не найденн для этого ус-ва {keyExchange}")
            }

            SendDataOrCommand(exchange, dataAction, inData, command4Device);  
        }


        private void SendDataOrCommand(IExchange<TIn> exchange, DataAction dataAction, List<TIn> inData,Command4Device command4Device = Command4Device.None)
        {
            if (!exchange.IsStartedTransportBg)
            {
                //TODO: produser.Send(Option.Name, $"Отправка данных НЕ удачна, Бекграунд обмена {exchange.Name} НЕ ЗАПЦУЩЕН")
            }
            if (!exchange.IsOpen)
            {
                //TODO: produser.Send(Option.Name, $"Отправка данных НЕ удачна, соединение транспорта для обмена {exchange.Name} НЕ ОТКРЫТО")
            }

            switch (dataAction)
            {
                case DataAction.OneTimeAction:
                    exchange?.SendOneTimeData(inData);
                    break;

                case DataAction.CycleAction:
                    if (!exchange.IsStartedCycleExchange)
                    {
                        //TODO: produser.Send(Option.Name, $"Отправка данных НЕ удачна, Цикл. обмен для обмена {exchange.Name} НЕ ЗАПЦУЩЕН")
                    }
                    exchange?.SendCycleTimeData(inData);
                    break;

                case DataAction.CommandAction:
                    exchange?.SendCommand(command4Device);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dataAction), dataAction, null);
            }
        }

 
        #endregion




        #region RxEventHandler 

        private void ConnectChangeRxEventHandler(IExchange<TIn> exchange)
        {
            var connect = exchange.IsConnect;
            
        }


        private void LastSendDataChangeRxEventHandler(IExchange<TIn> exchange)
        {
            var lastSendData = exchange.LastSendData;
        }


        private void OpenChangeTransportRxEventHandler(IsOpenChangeRxModel isOpenChangeRxModel)
        {
            _eventBus.Publish(isOpenChangeRxModel);  //Публикуем событие на общую шину данных
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            UnsubscrubeOnExchangesEvents();
        }

        #endregion
    }
}