using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worker.Background.Abstarct
{
    public interface IBackground : IDisposable 
    {
        /// <summary>
        /// Запуск Бегкроунда обмена.
        /// Флаг учитывается, только при старте сервиса.
        /// </summary>
        bool AutoStart { get; }
        bool IsStarted { get; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);

        void AddOneTimeAction(Func<CancellationToken, Task> action);
    }
}