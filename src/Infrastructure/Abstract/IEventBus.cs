using System;
using System.Threading.Tasks;

namespace Infrastructure.EventBus.Abstract
{
    /// <summary>
    /// Интерфейс внутренней шины данных
    /// </summary>
    public interface IEventBus : IDisposable
    {
        void Publish<TMessage>(TMessage msg);
        Task PublishAsync<TMessage>(TMessage msg);
        IDisposable Subscribe<TMessage>(Action<TMessage> action);
        IObservable<TMessage> Observe<TMessage>();
    }
}