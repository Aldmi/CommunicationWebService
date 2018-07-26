using Autofac;
using BL.Services.Mediators;
using BL.Services.Storage;

namespace WebServer.AutofacModules
{
    /// <summary>
    /// Регистрируем сервисы хранения бизнесс логики
    /// </summary>
    public class BlStorageAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SerialPortStorageService>().SingleInstance(); 
            builder.RegisterType<BackgroundStorageService>().SingleInstance();
            builder.RegisterType<ExchangeStorageService>().SingleInstance();
            builder.RegisterType<DeviceStorageService>().SingleInstance();   
        }
    }
}