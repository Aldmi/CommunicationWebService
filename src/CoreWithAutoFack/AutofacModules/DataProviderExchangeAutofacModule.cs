using System;
using Autofac;
using Exchange.Base.DataProviderAbstract;
using InputDataModel.Autodictor.ManualDataProvider;
using InputDataModel.Autodictor.Model;
using Shared.Types;

namespace WebServer.AutofacModules
{
    /// <summary>
    /// Регистрируем КОНКРЕТНЫЕ провайдеры по типу.
    /// </summary>
    public class DataProviderExchangeAutofacModule<TIn> : Module
    {
        protected override void Load(ContainerBuilder builder)
        {          
            switch (typeof(TIn).Name)
            {
                case "AdInputType":
                    builder.RegisterType<VidorBinaryDataProvider>().Named<IExchangeDataProvider<AdInputType, TransportResponse>>("VidorBinaryDataProvider").InstancePerDependency();
                    break;

                case "OtherType":
                    //builder.RegisterType<OtherDataProvider>().As<IExchangeDataProvider<TIn, TransportResponse>>().InstancePerDependency();
                    break;
            }      
        }
    }
}