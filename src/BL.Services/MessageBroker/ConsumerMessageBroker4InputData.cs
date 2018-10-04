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
    /// СТАРТ/СТОП сервиса происходит через background.
    /// </summary>
    /// <typeparam name="TIn">Тип обрабатываемых входных данных</typeparam>
    public class ConsumerMessageBroker4InputData<TIn> : IDisposable
    {
        #region field

        private readonly IConsumer  _consumer;
        private readonly ILogger _logger;
        private readonly InputDataApplyService<TIn> _inputDataApplyService;
        private readonly int _batchSize;
        private IDisposable _registration;

        #endregion




        #region ctor

        public ConsumerMessageBroker4InputData(
            ISimpleBackground background,
            IConsumer consumer,
            ILogger logger,
            InputDataApplyService<TIn> inputDataApplyService,
            int batchSize)
        {
            _batchSize = batchSize;
            _consumer = consumer;
            _logger = logger;
            _inputDataApplyService = inputDataApplyService;

            background.AddOneTimeAction(RunAsync); 
        }

        #endregion




        #region Methode

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
            Console.WriteLine(DateTime.Now.ToString("O"));//DEBUG
            //Обработчик сообщений с kafka
            foreach (var message in messages)
            {
                try
                {
                    var value = message.Value;
                    var inputDatas = JsonConvert.DeserializeObject<List<InputData<TIn>>>(value);
                    var errors= _inputDataApplyService.ApplyInputData(inputDatas);  //TODO: Выставлять обратно на шину (Produser) ответ об ошибках.
                }
                catch (Exception e)
                {
                    //LOG
                    Console.WriteLine(e);
                }

                //Console.WriteLine($"MessageBrokerGetingData {message.Topic}/{message.Partition} @{message.Offset}: '{message.Value}'");
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