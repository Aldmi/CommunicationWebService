using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Options;
using Logger.Abstract.Abstract;

namespace Infrastructure.MessageBroker.Consumer
{
    public sealed class RxKafkaConsumer : IConsumer
    {
        #region field

        private readonly ILogger _logger;
        private readonly IEnumerable<string> _topics;
        private readonly Consumer<Null, string> _consumer;

        #endregion




        #region ctor

        public RxKafkaConsumer(ConsumerOption option, ILogger logger)
        {
            _logger = logger;
            _topics = option.Topics;

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", option.BrokerEndpoints },
                { "api.version.request", true },
                { "group.id", !string.IsNullOrEmpty(option.GroupId) ? option.GroupId : Guid.NewGuid().ToString() },
                { "socket.blocking.max.ms", 1 },
                { "enable.auto.commit", false },  //отключить автокоммит офсета после прочтения сообщения (ручной коммит через CommitAsync())
                { "fetch.wait.max.ms", 5 },
                { "fetch.error.backoff.ms", 5 },
                { "fetch.message.max.bytes", 10240 },
                { "queued.min.messages", 1000 }, //1000
#if DEBUG
                // { "debug", "msg" },
#endif
                {
                    "default.topic.config",
                    new Dictionary<string, object>
                    {
                        { "auto.offset.reset", "beginning" }
                    }
                }
            };
            _consumer = new Consumer<Null, string>(config, new NullDeserializer(), new StringDeserializer(Encoding.UTF8));
            _consumer.OnLog += OnLog;
            _consumer.OnError += OnError;
            _consumer.OnConsumeError += OnConsumeError;
            _consumer.OnPartitionEOF += OnPartitionEof;
        }

        #endregion




        #region Methode

        public IObservable<Message<Null, string>> Consume(CancellationToken cancellationToken)
        {
            var observable = Observable.FromEventPattern<Message<Null, string>>(
                    x =>
                    {
                        _consumer.OnMessage += x;                  //Подписка на события получения данных с топиков в брокере
                        _consumer.Subscribe(_topics);              //Создание консюмера и подписка на брокера. 
                    },
                    x =>
                    {
                        _consumer.Unsubscribe();
                        _consumer.OnMessage -= x;
                    })
                .Select(x => x.EventArgs);

            Task.Factory.StartNew(
                    () =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            _consumer.Poll(TimeSpan.FromMilliseconds(100));  // проверка раз в 100мс новых данных в топиках брокера
                        }
                    },
                    cancellationToken,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default)
                .ConfigureAwait(false);

            return observable;
        }


        public async Task CommitAsync(Message<Null, string> message) => await _consumer.CommitAsync(message);

        #endregion




        #region EventHandler

        private void OnLog(object sender, LogMessage logMessage)
        {
            //Console.WriteLine($"{logMessage.Name}  {logMessage.Level}   {logMessage.Message}" );
            //_logger.LogInformation(
            //    "Consuming from Kafka. Client: '{client}', syslog level: '{logLevel}', message: '{logMessage}'.",
            //    logMessage.Name,
            //    logMessage.Level,
            //    logMessage.Message);
        }


        private void OnError(object sender, Error error)
        {
            Console.WriteLine($"Consumer error: {error}. No action required.");
            //_logger.LogInformation("Consumer error: {error}. No action required.", error);
        }


        private void OnConsumeError(object sender, Message message)
        {
            Console.WriteLine($"OnConsumeError: { message.Error}.");

            //_logger.LogError(
            //    "Error consuming from Kafka. Topic/partition/offset: '{topic}/{partition}/{offset}'. Error: '{error}'.",
            //    message.Topic,
            //    message.Partition,
            //    message.Offset,
            //    message.Error);

            throw new KafkaException(message.Error);
        }


        private void OnPartitionEof(object sender, TopicPartitionOffset e)
        {
            Console.WriteLine($"EOF     Topic= {e.Topic}    Offset= {e.Offset}");
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            if (_consumer != null)
            {
                _consumer.OnLog -= OnLog;
                _consumer.OnError -= OnError;
                _consumer.OnConsumeError -= OnConsumeError;
                _consumer.OnPartitionEOF -= OnPartitionEof;
                _consumer.Dispose();
            }
        }

        #endregion
    }
}