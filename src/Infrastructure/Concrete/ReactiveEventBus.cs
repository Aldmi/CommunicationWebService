using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Infrastructure.EventBus.Abstract;

namespace Infrastructure.EventBus.Concrete
{
    /// <summary>
    /// TODO: для каждого типа подписчика сделать словарь Key= type value=  ISubject<type>
    /// </summary>
    public class ReactiveEventBus : IEventBus
    {
        private readonly ISubject<object> _bus;



        public ReactiveEventBus()
        {
            _bus = new Subject<object>();
        }



        public void Publish<TMessage>(TMessage msg)
        {
            _bus.OnNext(msg);
        }


        public Task PublishAsync<TMessage>(TMessage msg)
        {
            throw new NotImplementedException();
        }


        public IDisposable Subscribe<TMessage>(Action<TMessage> action)
        {
            return _bus
                .OfType<TMessage>()
                .Subscribe((obj) =>
                {
                    var val = (TMessage)obj;
                    action(val);
                });
        }


        public IObservable<TMessage> Observe<TMessage>()
        {
            return _bus.Cast<TMessage>().AsObservable();
        }


        #region Disposable

        public void Dispose()
        {
            _bus?.OnCompleted();
        }

        #endregion
    }
}