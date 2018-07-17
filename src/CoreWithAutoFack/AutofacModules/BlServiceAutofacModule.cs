using Autofac;
using BL.Services;

namespace WebServer.AutofacModules
{
    /// <summary>
    /// Каждому послед. порту создается свой IBackgroundService.
    /// </summary>
    public class BlServiceAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SerialPortCollectionService>().SingleInstance(); 
            builder.RegisterType<BackgroundCollectionService>().SingleInstance();
            builder.RegisterType<ExchangeCollectionService>().SingleInstance();
        }
    }
}