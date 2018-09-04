using Autofac;
using BL.Services.Mediators;
using BL.Services.Storages;
using Exchange.Base.DataProviderAbstract;
using InputDataModel.Autodictor.InputData;
using InputDataModel.Autodictor.ManualDataProvider;
using Shared.Types;

namespace WebServer.AutofacModules
{
    /// <summary>
    /// Регистрируем сервисы хранения бизнесс логики. (оперативные данные, хранятся в памяти (CuncurrentDictionary))
    /// </summary>
    public class BlStorageAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransportStorageService>().SingleInstance(); 
            builder.RegisterType<BackgroundStorageService>().SingleInstance();
            builder.RegisterType<ExchangeStorageService<UniversalInputType>>().SingleInstance();
            builder.RegisterType<DeviceStorageService<UniversalInputType>>().SingleInstance();
        }
    }
}