using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;


namespace Worker.Background.Abstarct
{
    /// <summary>
    /// аналог IHostedService.
    /// </summary>
    public abstract class HostingBackgroundBase : IBackgroundService
    {
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();



        #region prop

        public KeyBackground KeyBackground { get; set; }

        /// <summary>
        /// Бекгроунд запущен, если задача продолжает выполняться. 
        /// </summary>
        public bool IsStarted => !(_executingTask == null ||
                                  _executingTask.IsCanceled ||
                                  _executingTask.IsCompleted ||
                                  _executingTask.IsFaulted);

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
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
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
        public abstract void AddCycleFunc(Func<CancellationToken, Task> action);
        public abstract void RemoveCycleFunc(Func<CancellationToken, Task> action);
        public abstract void AddOneTimeFunc(Func<CancellationToken, Task> action);

        #endregion




        #region Disposable

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }

        #endregion
    }
}