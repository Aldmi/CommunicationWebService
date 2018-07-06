using System;
using System.Threading;
using System.Threading.Tasks;
using Shared;
using Shared.Enums;

namespace Worker.Background.Abstarct
{
    public class KeyBackground
    {
       public string Key { get; set; }
       public TypeExchange TypeExchange { get; set; }
    }


    public interface IBackgroundService : IDisposable
    {
        KeyBackground KeyBackground { get; set; }

        bool IsStarted { get; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);

        void AddCycleAction(Func<CancellationToken, Task> action);
        void RemoveCycleFunc(Func<CancellationToken, Task> action);
        void AddOneTimeAction(Func<CancellationToken, Task> action);
    }
}