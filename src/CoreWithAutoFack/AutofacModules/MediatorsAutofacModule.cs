using Autofac;
using BL.Services.Mediators;
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