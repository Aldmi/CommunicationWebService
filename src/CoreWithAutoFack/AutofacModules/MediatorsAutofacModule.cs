using Autofac;
using BL.Services.Mediators;

namespace WebServer.AutofacModules
{
    public class MediatorsAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediatorForStorages>().InstancePerDependency();
            builder.RegisterType<MediatorForOptionsRepository>().InstancePerDependency();
        }
    }
}