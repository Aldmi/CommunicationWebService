using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;


namespace Worker.Background.Abstarct
{
    public abstract class HostingBackgroundBase : IBackgroundService, IHostedService
    {
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();



        #region prop

        public KeyBackground KeyBackground { get; set; }
        public Func<int, CancellationToken> AddWork { get; set; }
        public Func<int, CancellationToken> RemoveWork { get; set; }

        #endregion




        #region ctor

        protected HostingBackgroundBase(KeyBackground key)
        {
            KeyBackground = key;
        }

        #endregion




        #region Methode

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _stoppingCts = new CancellationTokenSource();
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }
            return Task.CompletedTask;
        }


        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                var task = await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                if (task == _executingTask)
                {
                    Console.WriteLine("_executingTask");
                }
                else
                {
                    Console.WriteLine(" cancellationToken");
                }
            }

        }



        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessAsync(stoppingToken);
            }
        }
        protected abstract Task ProcessAsync(CancellationToken stoppingToken);

        #endregion




        #region Disposable

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }

        #endregion
    }
}