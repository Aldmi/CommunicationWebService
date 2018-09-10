using System;
using System.Threading.Tasks;
using Infrastructure.EventBus.Abstract;
using MemBus;
using MemBus.Configurators;

namespace Infrastructure.EventBus.Concrete
{
    public class MemEventBus : IEventBus
    {
        private readonly IBus _bus;


        #region ctor

        public MemEventBus()
        {
            _bus = BusSetup.StartWith<Conservative>().Construct();
        }

        #endregion




        #region Methode

        public void Publish<TMessage>(TMessage msg)
        {
            _bus.Publish(msg);
        }

        public Task PublishAsync<TMessage>(TMessage msg)
        {
            return _bus.PublishAsync(msg);
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> action)
        {
            return _bus.Subscribe(action);
        }


        /// <summary>
        /// Использовать для наблюдения за любыми публикациями объектов типа TMessage, внутри класса наблюдателя реализующего IObserver<TMessage>.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns>наблюдаемый объект</returns>
        public IObservable<TMessage> Observe<TMessage>()
        {
            return _bus.Observe<TMessage>();
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            _bus?.Dispose();
        }

        #endregion
    }

}