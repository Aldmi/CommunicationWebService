﻿using Autofac;
using Exchange.Base.DataProviderAbstract;
using Exchange.Base.Model;
using InputDataModel.Autodictor.ByRuleDataProviders;
using InputDataModel.Autodictor.ManualDataProviders;
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
                    builder.RegisterType<VidorBinaryDataProvider>().Named<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>>("VidorBinary").InstancePerDependency();
                    builder.RegisterType<ByRulesDataProvider>().Named<IExchangeDataProvider<AdInputType, ResponseDataItem<AdInputType>>>("ByRules").InstancePerDependency();
                    break;

                case "OtherType":
                    //builder.RegisterType<OtherDataProvider>().As<IExchangeDataProvider<TIn, TransportResponse>>().InstancePerDependency();
                    break;
            }      
        }
    }
}