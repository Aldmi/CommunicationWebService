using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.OwnedInstances;
using DAL.Abstract.Entities.Options.Device;
using Exchange.Base;
using Exchange.Base.Model;
using Infrastructure.EventBus.Abstract;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Options;
using InputDataModel.Base;
using Newtonsoft.Json;
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

        private readonly IEventBus _eventBus; //TODO: Not Use

        private readonly ProduserOption _produser4DeviceOption;
        private readonly IProduser _produser;
        private readonly Owned<IProduser> _produserOwner;

        private readonly List<IDisposable> _disposeExchangesEventHandlers = new List<IDisposable>();

        #endregion



        #region prop

        public DeviceOption Option { get;  }
        public List<IExchange<TIn>> Exchanges { get; }

        #endregion




        #region ctor

        public Device(DeviceOption option,
                      IEnumerable<IExchange<TIn>> exchanges,
                      IEventBus eventBus,
                      Func<ProduserOption, Owned<IProduser>> produser4DeviceRespFactory,
                      ProduserOption produser4DeviceOption)
        {
            Option = option;
            Exchanges = exchanges.ToList();
            _eventBus = eventBus;

            _produser4DeviceOption = produser4DeviceOption;
            var produserOwner = produser4DeviceRespFactory(_produser4DeviceOption);
            _produserOwner = produserOwner;                  //можно создать/удалить produser в любое время используя фабрику и Owner 
            _produser = produserOwner.Value;

            SubscrubeOnExchangesEvents();
        }

        #endregion




        #region Methode

        public void SubscrubeOnExchangesEvents()
        {
            Exchanges.ForEach(exch =>
            {
               // _disposeExchangesEventHandlers.Add(exch.IsConnectChangeRx.Subscribe(ConnectChangeRxEventHandler));
               // _disposeExchangesEventHandlers.Add(exch.LastSendDataChangeRx.Subscribe(LastSendDataChangeRxEventHandler));
                _disposeExchangesEventHandlers.Add(exch.IsOpenChangeTransportRx.Subscribe(OpenChangeTransportRxEventHandler));
                _disposeExchangesEventHandlers.Add(exch.ResponseChangeRx.Subscribe(TransportResponseChangeRxEventHandler));
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
                _produser.ProduceAsync(Option.Name, $"Обмен не найденн для этого ус-ва {keyExchange}");
                return;
            }
            SendDataOrCommand(exchange, dataAction, inData, command4Device);  
        }


        /// <summary>
        /// Отправить данные или команду.
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="dataAction"></param>
        /// <param name="inData"></param>
        /// <param name="command4Device"></param>
        private void SendDataOrCommand(IExchange<TIn> exchange, DataAction dataAction, List<TIn> inData,Command4Device command4Device = Command4Device.None)
        {
            if (!exchange.IsStartedTransportBg)
            {
                _produser.ProduceAsync(Option.Name, $"Отправка данных НЕ удачна, Бекграунд обмена {exchange.KeyExchange} НЕ ЗАПЦУЩЕН");
            }
            if (!exchange.IsOpen)
            {
                _produser.ProduceAsync(Option.Name, $"Отправка данных НЕ удачна, соединение транспорта для обмена {exchange.KeyExchange} НЕ ОТКРЫТО");
            }
            switch (dataAction)
            {
                case DataAction.OneTimeAction:
                    exchange?.SendOneTimeData(inData);
                    break;

                case DataAction.CycleAction:
                    if (!exchange.IsStartedCycleExchange)
                    {
                        _produser.ProduceAsync(Option.Name, $"Отправка данных НЕ удачна, Цикл. обмен для обмена {exchange.KeyExchange} НЕ ЗАПУЩЕН");
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
            _produser.ProduceAsync(Option.Name, $"Connect = {exchange.IsConnect} для обмена {exchange.KeyExchange}");
        }


        private void LastSendDataChangeRxEventHandler(IExchange<TIn> exchange)
        {
            _produser.ProduceAsync(Option.Name, $"LastSendData = {exchange.LastSendData} для обмена {exchange.KeyExchange}");
        }

        //TODO: События транспорта нужно перевыбрасывать в обмене, чтобы добавить к событию иноформацию об обмене.
        private void OpenChangeTransportRxEventHandler(IsOpenChangeRxModel isOpenChangeRxModel)
        {
            //_eventBus.Publish(isOpenChangeRxModel);  //Публикуем событие на общую шину данных
            Console.WriteLine($"isOpenChangeRxModel {isOpenChangeRxModel.TransportName}   {isOpenChangeRxModel.IsOpen} START>>>>>>>");//DEBUG
            //TODO: produser.Send(Option.Name, $"LastSendData = {exchange.LastSendData} для обмена {exchange.KeyExchange}")

        }


        private void TransportResponseChangeRxEventHandler(OutResponseDataWrapper<TIn> outResponseDataWrapper)
        {
            var settings= new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,             //Отступы дочерних элементов 
                NullValueHandling = NullValueHandling.Ignore  //Игнорировать пустые теги
            };
            var jsonResp = JsonConvert.SerializeObject(outResponseDataWrapper, settings);
            _produser.ProduceAsync(Option.Name, $"ResponseDataWrapper = {jsonResp}");   
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            UnsubscrubeOnExchangesEvents();
            _produserOwner.Dispose();
        }

        #endregion
    }
}