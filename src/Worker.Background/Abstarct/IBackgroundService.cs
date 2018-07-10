﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Enums;
using Shared.Types;

namespace Worker.Background.Abstarct
{
    public interface IBackgroundService : IDisposable
    {
        KeyExchange KeyExchange { get; set; }

        bool IsStarted { get; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);

        void AddCycleAction(Func<CancellationToken, Task> action);
        void RemoveCycleFunc(Func<CancellationToken, Task> action);
        void AddOneTimeAction(Func<CancellationToken, Task> action);
    }
}