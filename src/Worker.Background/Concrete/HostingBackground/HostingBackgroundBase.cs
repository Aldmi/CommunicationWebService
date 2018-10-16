using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worker.Background.Concrete.HostingBackground
{
    /// <summary>
    /// аналог IHostedService.
    /// </summary>
    public abstract class HostingBackgroundBase
    {
        protected const int CheckUpdateTime = 100;
        protected Task ExecutingTask;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();


        #region prop

        public bool AutoStart { get; }

        /// <summary>
        /// Бекгроунд запущен, если задача продолжает выполняться. 
        /// </summary>
        public bool IsStarted => !(ExecutingTask == null ||
                                   ExecutingTask.IsCanceled ||
                                   ExecutingTask.IsCompleted ||
                                   ExecutingTask.IsFaulted);

        #endregion



        #region ctor

        protected HostingBackgroundBase(bool autoStart)
        {
            AutoStart = autoStart;
        }

        #endregion



        #region Methode

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _stoppingCts = new CancellationTokenSource();
            ExecutingTask = ExecuteAsync(_stoppingCts.Token);
            return ExecutingTask.IsCompleted ? ExecutingTask : Task.CompletedTask;
        }


        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (ExecutingTask == null)
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
                await Task.WhenAny(ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
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