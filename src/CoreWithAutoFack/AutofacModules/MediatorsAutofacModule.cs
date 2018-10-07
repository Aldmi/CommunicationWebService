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
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediatorForStorages<TIn>>().InstancePerDependency();
            builder.RegisterType<MediatorForOptions>().InstancePerDependency();
        }
    }
}