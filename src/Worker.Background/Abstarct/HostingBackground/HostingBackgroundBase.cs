using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Types;

namespace Worker.Background.Abstarct.HostingBackground
{
    /// <summary>
    /// аналог IHostedService.
    /// </summary>
    public abstract class HostingBackgroundBase : IBackground
    {
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();


        #region prop

        public KeyTransport KeyTransport { get; }

        /// <summary>
        /// Бекгроунд запущен, если задача продолжает выполняться. 
        /// </summary>
        public bool IsStarted => !(_executingTask == null ||
                                   _executingTask.IsCanceled ||
                                   _executingTask.IsCompleted ||
                                   _executingTask.IsFaulted);

        #endregion


        #region ctor

        protected HostingBackgroundBase(KeyTransport key)
        {
            KeyTransport = key;
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
        public abstract void AddCycleAction(Func<CancellationToken, Task> action);
        public abstract void RemoveCycleFunc(Func<CancellationToken, Task> action);
        public abstract void AddOneTimeAction(Func<CancellationToken, Task> action);

        #endregion


        #region Disposable

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }

        #endregion
    }
}