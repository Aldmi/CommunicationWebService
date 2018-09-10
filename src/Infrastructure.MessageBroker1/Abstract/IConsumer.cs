using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Infrastructure.MessageBroker.Abstract
{
    public interface IConsumer : IDisposable
    {
        IObservable<Message<Null, string>> Consume(CancellationToken cancellationToken);
        Task CommitAsync(Message<Null, string> message);                          //Подтвердить получение порции данных
    }
}