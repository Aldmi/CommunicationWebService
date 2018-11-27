using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.OwnedInstances;
using Confluent.Kafka;
using DAL.Abstract.Entities.Options.Device;
using Exchange.Base;
using Exchange.Base.Model;
using Exchange.Base.RxModel;
using Infrastructure.EventBus.Abstract;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Options;
using InputDataModel.Base;
using Newtonsoft.Json;
using Serilog;
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
        private readonly ILogger _logger;
        private readonly IProduser _produser;
        private readonly Owned<IProduser> _produserOwner;
        private readonly List<IDisposable> _disposeExchangesEventHandlers = new List<IDisposable>();

        #endregion



        #region prop

        public DeviceOption Option { get;  }
        public List<IExchange<TIn>> Exchanges { get; }
        public string TopicName4MessageBroker { get; set; }

        #endregion




        #region ctor

        public Device(DeviceOption option,
                      IEnumerable<IExchange<TIn>> exchanges,
                      IEventBus eventBus,
                      Func<ProduserOption, Owned<IProduser>> produser4DeviceRespFactory,
                      ProduserOption produser4DeviceOption,
                      ILogger logger)
        {
            Option = option;
            Exchanges = exchanges.ToList();
            _eventBus = eventBus;
            _logger = logger;

            var produserOwner = produser4DeviceRespFactory(produser4DeviceOption);
            _produserOwner = produserOwner;                  //можно создать/удалить produser в любое время используя фабрику и Owner 
            _produser = produserOwner.Value;
            TopicName4MessageBroker = null;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Подписка на публикацию событий устройства на ВНЕШНЮЮ ШИНУ ДАННЫХ
        /// </summary>
        /// <param name="topicName4MessageBroker">Имя топика, если == null, то берется из настроек</param>
        public bool SubscrubeOnExchangesEvents(string topicName4MessageBroker = null)
        {
            //Подписка уже есть
            if (!string.IsNullOrEmpty(TopicName4MessageBroker))
                return false;

            //Топик не указан
            TopicName4MessageBroker = topicName4MessageBroker ?? Option.TopicName4MessageBroker;
            if (string.IsNullOrEmpty(TopicName4MessageBroker))
                return false;

            Exchanges.ForEach(exch =>
            {
                _disposeExchangesEventHandlers.Add(exch.IsConnectChangeRx.Subscribe(ConnectChangeRxEventHandler));
                //_disposeExchangesEventHandlers.Add(exch.LastSendDataChangeRx.Subscribe(LastSendDataChangeRxEventHandler));
                _disposeExchangesEventHandlers.Add(exch.IsOpenChangeTransportRx.Subscribe(OpenChangeTransportRxEventHandler));
                _disposeExchangesEventHandlers.Add(exch.ResponseChangeRx.Subscribe(TransportResponseChangeRxEventHandler));
            });
            return true;
        }

 


        public void UnsubscrubeOnExchangesEvents()
        {
            TopicName4MessageBroker = null;
            _disposeExchangesEventHandlers.ForEach(d=>d.Dispose());
        }


        /// <summary>
        /// Отправить данные или команду на все обмены.
        /// </summary>
        /// <param name="dataAction"></param>
        /// <param name="inData"></param>
        /// <param name="command4Device"></param>
        public async Task Send2AllExchanges(DataAction dataAction, List<TIn> inData, Command4Device command4Device = Command4Device.None)
        {
            var tasks= new List<Task>();
            foreach (var exchange in Exchanges)
            {
                tasks.Add(SendDataOrCommand(exchange, dataAction, inData, command4Device));
            }
            await Task.WhenAll(tasks);
        }


        /// <summary>
        /// Отправить данные или команду на выбранный обмен.
        /// </summary>
        /// <param name="keyExchange"></param>
        /// <param name="dataAction"></param>
        /// <param name="inData"></param>
        /// <param name="command4Device"></param>
        public async Task Send2ConcreteExchanges(string keyExchange, DataAction dataAction, List<TIn> inData, Command4Device command4Device = Command4Device.None)
        {
            var exchange = Exchanges.FirstOrDefault(exch=> exch.KeyExchange == keyExchange);
            if (exchange == null)
            {
                await Send2Produder(Option.TopicName4MessageBroker, $"Обмен не найденн для этого ус-ва {keyExchange}");
                return;
            }
            await SendDataOrCommand(exchange, dataAction, inData, command4Device);  
        }


        /// <summary>
        /// Отправить данные или команду.
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="dataAction"></param>
        /// <param name="inData"></param>
        /// <param name="command4Device"></param>
        private async Task SendDataOrCommand(IExchange<TIn> exchange, DataAction dataAction, List<TIn> inData, Command4Device command4Device = Command4Device.None)
        {
            if (!exchange.IsStartedTransportBg)
            {
                await Send2Produder(Option.TopicName4MessageBroker, $"Отправка данных НЕ удачна, Бекграунд обмена {exchange.KeyExchange} НЕ ЗАПЦУЩЕН");
                return;
            }
            if (!exchange.IsOpen)
            {
                await Send2Produder(Option.TopicName4MessageBroker, $"Отправка данных НЕ удачна, соединение транспорта для обмена {exchange.KeyExchange} НЕ ОТКРЫТО");
                return;
            }
            switch (dataAction)
            {
                case DataAction.OneTimeAction:
                    exchange?.SendOneTimeData(inData);
                    break;

                case DataAction.CycleAction:
                    if (!exchange.IsStartedCycleExchange)
                    {
                        await Send2Produder(Option.TopicName4MessageBroker, $"Отправка данных НЕ удачна, Цикл. обмен для обмена {exchange.KeyExchange} НЕ ЗАПУЩЕН");
                        return;
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


        private async Task Send2Produder(string topicName, string formatStr)
        {
            try
            {
               await _produser.ProduceAsync(topicName, formatStr);
            }
            catch (KafkaException ex)
            {
                _logger.Error(ex, $"KafkaException= {ex.Message} для {topicName}");
            }
        }

        #endregion




        #region RxEventHandler 

        private async void ConnectChangeRxEventHandler(ConnectChangeRxModel model)
        {
            //await Send2Produder(Option.TopicName4MessageBroker, $"Connect = {model.IsConnect} для обмена {model.KeyExchange}");
            _logger.Debug($"ConnectChangeRxEventHandler.  Connect = {model.IsConnect} для обмена {model.KeyExchange}");
        }

        private async void LastSendDataChangeRxEventHandler(LastSendDataChangeRxModel<TIn> model)
        {
            await Send2Produder(Option.TopicName4MessageBroker, $"LastSendData = {model.LastSendData} для обмена {model.KeyExchange}");
        }

        private async void OpenChangeTransportRxEventHandler(IsOpenChangeRxModel model)
        {
            //await Send2Produder(Option.TopicName4MessageBroker,$"IsOpen = {model.IsOpen} для ТРАНСПОРТА {model.TransportName}");
            //_eventBus.Publish(isOpenChangeRxModel);  //Публикуем событие на общую шину данных  
            _logger.Debug($"OpenChangeTransportRxEventHandler.  IsOpen = {model.IsOpen} для ТРАНСПОРТА {model.TransportName}");
        }

        private async void TransportResponseChangeRxEventHandler(ResponsePieceOfDataWrapper<TIn> responsePieceOfDataWrapper)
        {
            var settings= new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,             //Отступы дочерних элементов 
                NullValueHandling = NullValueHandling.Ignore  //Игнорировать пустые теги
            };
            var jsonResp = JsonConvert.SerializeObject(responsePieceOfDataWrapper, settings);
            //await Send2Produder(Option.TopicName4MessageBroker, $"ResponseDataWrapper = {jsonResp}");
            _logger.Debug($"TransportResponseChangeRxEventHandler.  jsonResp = {jsonResp} ");
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