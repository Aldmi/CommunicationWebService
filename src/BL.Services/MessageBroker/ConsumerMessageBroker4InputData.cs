using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.Services.InputData;
using BL.Services.Storages;
using Confluent.Kafka;
using Infrastructure.MessageBroker.Abstract;
using InputDataModel.Base;
using Logger.Abstract.Abstract;
using Newtonsoft.Json;
using Worker.Background.Abstarct;

namespace BL.Services.MessageBroker
{
    /// <summary>
    /// Фоновый прлцесс получения даныых от брокера сообщений
    /// Входные данные поступают на нужные ус-ва.
    /// </summary>
    /// <typeparam name="TIn">Тип обрабатываемых входных данных</typeparam>
    public class ConsumerMessageBroker4InputData<TIn> : IDisposable
    {
        #region field

        private readonly IConsumer  _consumer;
        private readonly IBackground _background;
        private readonly ILogger _logger;
        private readonly GetInputDataService<TIn> _getInputDataService;
        private readonly int _batchSize;
        private IDisposable _registration;

        #endregion




        #region ctor

        public ConsumerMessageBroker4InputData(IConsumer consumer,   //TODO: заменить на Func<IOwned<IConsumer>>
            IBackground background,
            ILogger logger,
            GetInputDataService<TIn> getInputDataService,
            int batchSize)
        {
            _batchSize = batchSize;
            _consumer = consumer;
            _background = background; //TODO: RunAsync запускать на background
            _logger = logger;
            _getInputDataService = getInputDataService;
 

            _background.AddCycleAction(RunAsync);
        }

        #endregion




        #region Methode

        //public async Task StartBackgroundAsync(CancellationToken ct)
        //{
        //   await _background.StartAsync(ct);
        //}


        //public async Task StopBackgroundAsync(CancellationToken ct)
        //{
        //    await _background.StartAsync(ct);
        //}


        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var observable = _consumer.Consume(cancellationToken);  //создаем наблюдаемую коллекцию сообщений с брокера

            var subscription = observable
                .Buffer(_batchSize)
                .Subscribe(
                     messages =>
                    {
                       MessageHandler(messages);
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

        #endregion



        #region EventHandler

        private void MessageHandler(IList<Message<Null, string>> messages)
        {
            //Обработчик сообщений с kafka
            foreach (var message in messages)
            {
                try
                {
                    var value = message.Value;
                    var inputDatas = JsonConvert.DeserializeObject<List<InputData<TIn>>>(value);
                    _getInputDataService.ApplyInputData(inputDatas);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                //Console.WriteLine($"{message.Topic}/{message.Partition} @{message.Offset}: '{message.Value.Length}'"); //'{message.Value}'
                //Console.WriteLine($"{message.Topic}/{message.Partition} @{message.Offset}: '{message.Value.Length}'"); //'{message.Value}'
            }
        }

        #endregion



        #region Disposable

        public void Dispose()
        {
            _registration.Dispose();
            _consumer.Dispose();
        }

        #endregion
    }
}