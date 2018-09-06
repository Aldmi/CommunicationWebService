using Autofac;
using BL.Services.Mediators;
using BL.Services.Storages;
using Exchange.Base.DataProviderAbstract;
using InputDataModel.Autodictor.Model;
using Shared.Types;

namespace WebServer.AutofacModules
{
    /// <summary>
    /// Регистрируем сервисы хранения бизнесс логики. (оперативные данные, хранятся в памяти (CuncurrentDictionary))
    /// </summary>
    public class BlStorageAutofacModule<TIn> : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransportStorageService>().SingleInstance(); 
            builder.RegisterType<BackgroundStorageService>().SingleInstance();
            builder.RegisterType<ExchangeStorageService<TIn>>().SingleInstance();
            builder.RegisterType<DeviceStorageService<TIn>>().SingleInstance();
        }
    }
}