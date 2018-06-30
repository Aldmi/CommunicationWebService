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
        Func<int, CancellationToken> AddWork { get; set; }
        Func<int, CancellationToken> RemoveWork { get; set; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}