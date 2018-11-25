using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Options;
using Serilog;


namespace Infrastructure.MessageBroker.Produser
{
    public class KafkaProducer : IProduser
    {
        #region field

        private readonly ProduserOption _option;
        private readonly ILogger _logger;
        private readonly Producer<Null, string> _producer;

        #endregion



        #region ctor

        public KafkaProducer(ProduserOption option, ILogger logger)
        {
            _option = option;
            _logger = logger;

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", option.BrokerEndpoints },
                { "api.version.request", true },
                { "socket.blocking.max.ms", 1 },
                { "queue.buffering.max.ms", 5 },
                { "queue.buffering.max.kbytes", 10240 },
#if DEBUG
                { "debug", "msg" },
#endif
                {
                    "default.topic.config",
                    new Dictionary<string, object>
                    {
                        { "message.timeout.ms", 3000 },        //таймаут на подключение к брокеру (если таймаут вышел, то выставляется message.Error в ответе)
                        { "request.required.acks", -1 }        // гарантированная доставка сообщения до конкретного партишена. (самая высокая гарантия доставки в брокера)
                    }
                }
            };
            _producer = new Producer<Null, string>(config, new NullSerializer(), new StringSerializer(Encoding.UTF8));
            _producer.OnLog += OnLog;
            _producer.OnError += OnError;
        }

        #endregion




        #region Methode

        public async Task<Message<Null, string>> ProduceAsync(string topic, string value, int partition = -1)
        {
            try
            {
                Message<Null, string> message = null;
                if (partition < 0)
                {
                    message = await _producer.ProduceAsync(topic, null, value);
                }
                else
                {
                    message = await _producer.ProduceAsync(topic, null, value, partition);
                }

                if (message.Error.HasError)          //Продюсеру не нужен переконнект к брокеру в случае ошибки, он либо может отправить данные , либо нет.
                {
                    throw new KafkaException(message.Error);
                }

                return message;
            }
            catch (Exception ex)
            {
                //_logger.LogError(
                //    new EventId(),
                //    ex,
                //    "Error producing to Kafka. Topic/partition: '{topic}/{partition}'. Message: {message}'.",
                //    topic,
                //    partition,
                //    message?.Value ?? "N/A");
                throw;
            }
        }

        #endregion

        


        #region EventHandler

        private void OnLog(object sender, LogMessage logMessage)
        {
            //_logger.LogInformation(
            //    "Producing to Kafka. Client: {client}, syslog level: '{logLevel}', message: {logMessage}.",
            //    logMessage.Name,
            //    logMessage.Level,
            //    logMessage.Message);
        }


        private void OnError(object sender, Error error)
        {
            //_logger.LogInformation("Producer error: {error}. No action required.", error);
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            if (_producer != null)
            {
                _producer.OnLog -= OnLog;
                _producer.OnError -= OnError;
                _producer.Dispose();
            }
        }

        #endregion
    }
}