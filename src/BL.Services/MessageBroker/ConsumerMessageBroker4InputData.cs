using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BL.Services.InputData;
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
        private readonly ILogger _logger;
        private readonly GetInputDataService<TIn> _getInputDataService;
        private readonly ISimpleBackground _background;
        private readonly int _batchSize;
        private IDisposable _registration;

        protected Task ExecutingTask;
        private CancellationTokenSource _stoppingCts;

        #endregion




        #region ctor

        public ConsumerMessageBroker4InputData(
            ISimpleBackground background,
            IConsumer consumer,
            ILogger logger,
            GetInputDataService<TIn> getInputDataService,
            int batchSize)
        {
            _background = background;
            _batchSize = batchSize;
            _consumer = consumer;
            _logger = logger;
            _getInputDataService = getInputDataService;

            _background.AddOneTimeAction(RunAsync);
        }

        #endregion




        #region Methode

        public Task Start()
        {
            _stoppingCts = new CancellationTokenSource();     
            ExecutingTask = RunAsync(_stoppingCts.Token);
            return ExecutingTask.IsCompleted ? ExecutingTask : Task.CompletedTask;
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {       
            if (ExecutingTask == null)
            {
                return;
            }
            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }


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