using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Infrastructure.MessageBroker.Abstract;
using Logger.Abstract.Abstract;
using Worker.Background.Abstarct;

namespace BL.Services.MessageBroker
{
    public class ConsumerMessageBrokerService : IDisposable
    {

        #region field

        private readonly IConsumer  _consumer;
        private readonly IBackground _background;
        private readonly ILogger _logger;
        private readonly int _batchSize;
        private IDisposable _registration;

        #endregion




        #region ctor

        public ConsumerMessageBrokerService(IConsumer consumer, IBackground background, ILogger logger, int batchSize)
        {
            _batchSize = batchSize;
            _consumer = consumer;
            _background = background; //TODO: RunAsync запускать на background
            _logger = logger;
            // _kafkaConsumer = new ConsumerMessageBrokerService(logger, brokerEndpoints, GroupId, topics);
        }

        #endregion





        public async Task RunAsync(CancellationToken cancellationToken)
        {



            var observable = _consumer.Consume(cancellationToken);  //создаем наблюдаемую коллекцию сообщений с брокера

            var subscription = observable
                .Buffer(_batchSize)
                .Subscribe(
                    messages =>
                    {
                        KafkaMessageHandler(messages);
                        _consumer.CommitAsync(messages[messages.Count - 1]).GetAwaiter().GetResult();  //ручной коммит офсета для этого консьюмера
                    });

            var taskCompletionSource = new TaskCompletionSource<object>();  // задача (отписка от RX события) заверешающаяся только при сработке cancellationToken 
            _registration = cancellationToken.Register(
                () =>
                {
                    subscription.Dispose();
                    taskCompletionSource.SetResult(null);
                });

            await taskCompletionSource.Task;
        }



        private void KafkaMessageHandler(IList<Message<Null, string>> messages)
        {
            //Обработчик сообщений с kafka
            foreach (var message in messages)
            {
                //Console.WriteLine($"{message.Topic}/{message.Partition} @{message.Offset}: '{message.Value.Length}'"); //'{message.Value}'
                //Console.WriteLine($"{message.Topic}/{message.Partition} @{message.Offset}: '{message.Value.Length}'"); //'{message.Value}'
            }
        }




        public void Dispose()
        {
            _registration.Dispose();
            _consumer.Dispose();
        }
    }
}