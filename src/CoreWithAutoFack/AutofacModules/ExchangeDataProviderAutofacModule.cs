using Autofac;
using Exchange.Base.DataProviderAbstract;
using InputDataModel.Autodictor.InputData;
using InputDataModel.Autodictor.ManualDataProvider;
using Shared.Types;

namespace WebServer.AutofacModules
{
    public class ExchangeDataProviderAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VidorBinaryDataProvider>().As<IExchangeDataProvider<UniversalInputType, TransportResponse>>().InstancePerDependency();
        }
    }
}