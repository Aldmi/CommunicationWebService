using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Worker.Background.Abstarct;

namespace Worker.Background.Concrete.BackgroundSerialPort
{
    public class BackgroundMasterSerialPort : HostingBackgroundScoped
    {
        // Func<>  //очередь послед. порта


        public BackgroundMasterSerialPort(IServiceScopeFactory serviceScopeFactory, KeyBackground keyBackground) : base(serviceScopeFactory, keyBackground)
        {
        }

        protected override async Task ExecuteInScopeAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            //РАЗМАТЫВАЕМ ОЧЕРЕДЬ ПОСЛЕД. ПОРТА
            //Task.Delay ИСПОЛЬЗУЕТСЯ В КАЖДОМ Func<>


            await Task.Delay(500, stoppingToken); //Период выполнения
            Console.WriteLine("BackGroundMasterSp ");
        }
    }


}