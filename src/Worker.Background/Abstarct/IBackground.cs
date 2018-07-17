using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Types;

namespace Worker.Background.Abstarct
{
    public interface IBackground : ISupportKeyTransport, IDisposable
    {
        bool IsStarted { get; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);

        void AddCycleAction(Func<CancellationToken, Task> action);
        void RemoveCycleFunc(Func<CancellationToken, Task> action);
        void AddOneTimeAction(Func<CancellationToken, Task> action);
    }
}