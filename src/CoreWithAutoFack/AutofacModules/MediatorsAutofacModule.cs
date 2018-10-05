using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using BL.Services.Mediators;
using Infrastructure.MessageBroker.Options;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace WebServer.AutofacModules
{
    public class MediatorsAutofacModule<TIn> : Module
    {
        public ProduserOption ProduserOption { get; }



        #region ctor

        /// <param name="configProduser">секция опций которая описывает ProduserOption</param>
        public MediatorsAutofacModule(IConfigurationSection configProduser)
        {
            ProduserOption = new ProduserOption
            {
                BrokerEndpoints = configProduser["BrokerEndpoints"],
            };
        }

        #endregion



        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediatorForStorages<TIn>>()
                   .WithParameters(new List<ResolvedParameter>
                   {
                       new ResolvedParameter(
                           (pi, ctx) => (pi.ParameterType == typeof(ProduserOption) && (pi.Name == "produser4DeviceOption")),
                           (pi, ctx) => ProduserOption)
                   }).InstancePerDependency();

            builder.RegisterType<MediatorForOptions>().InstancePerDependency();
        }
    }
}