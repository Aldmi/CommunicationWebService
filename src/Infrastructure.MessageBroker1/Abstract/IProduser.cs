using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Infrastructure.MessageBroker.Abstract
{
    public interface IProduser : IDisposable
    {
        Task<Message<Null, string>> ProduceAsync(string value, string topic, int partition = -1);
    }
}