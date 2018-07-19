using Autofac;
using BL.Services;
using BL.Services.Storage;
using Exchange.Base;
using Transport.SerialPort.Abstract;
using Worker.Background.Abstarct;

namespace WebServer.AutofacModules
{
    /// <summary>
    /// Каждому послед. порту создается свой IBackgroundService.
    /// </summary>
    public class BlServiceAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SerialPortStorageService>().SingleInstance(); 
            builder.RegisterType<BackgroundStorageService>().SingleInstance();
            builder.RegisterType<ExchangeStorageService>().SingleInstance();
        }
    }
}