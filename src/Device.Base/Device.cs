using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Abstract.Entities.Options.Device;
using Exchange.Base;
using Exchange.Base.Model;
using Infrastructure.EventBus.Abstract;
using Transport.Base.RxModel;

namespace Device.Base
{
    /// <summary>
    /// Устройство.
    /// Содержит список обменов.
    /// </summary>
    public class Device : IDisposable
    {
        #region field

        private readonly IEventBus _eventBus;
        private List<IDisposable> _disposeExchangesEventHandlers = new List<IDisposable>();

        #endregion



        #region prop

        public DeviceOption Option { get;  }
        public List<IExchange> Exchanges { get; }

        #endregion




        #region ctor

        public Device(DeviceOption option, IEnumerable<IExchange> exchanges, IEventBus eventBus)
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



        void SendCommand(string commandName, UniversalInputType data4Command = null)
        {

        }

        #endregion




        #region RxEventHandler 

        private void ConnectChangeRxEventHandler(IExchange exchange)
        {
            var connect = exchange.IsConnect;
            
        }


        private void LastSendDataChangeRxEventHandler(IExchange exchange)
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