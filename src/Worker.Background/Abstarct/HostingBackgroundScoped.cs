using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Types;

namespace Worker.Background.Abstarct
{
    public abstract class HostingBackgroundScoped : HostingBackgroundBase
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;


        protected HostingBackgroundScoped(IServiceScopeFactory serviceScopeFactory, KeyExchange keyExchange) : base(keyExchange)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        /// <summary>
        /// Задает ограниченный scope для всех сервисов разрезолвенных через IServiceProvider, внутри метода ExecuteInScopeAsync.
        /// Это необходимо для всех НЕ синглтон зависимостей, иначе будет утечка памяти, т.к. они удерживаются синглтон BackgroundService.
        /// Для Scoped зависимостей переданных в ctor BackgroundService выдается исключение.
        /// </summary>
        protected override async Task ProcessAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                await ExecuteInScopeAsync(scope.ServiceProvider, stoppingToken);
            }
        }


        /// <summary>
        /// Метод который обрабатыывает все зависимости в строго заданном scope
        /// </summary>
        protected abstract Task ExecuteInScopeAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken);
    }
}