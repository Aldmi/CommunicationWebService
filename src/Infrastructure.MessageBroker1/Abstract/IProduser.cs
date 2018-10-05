using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Infrastructure.MessageBroker.Abstract
{
    public interface IProduser : IDisposable
    {
        Task<Message<Null, string>> ProduceAsync(string topic, string value, int partition = -1);
    }
}