using Autofac;
using BL.Services.Mediators;
using InputDataModel.Autodictor.InputData;

namespace WebServer.AutofacModules
{
    public class MediatorsAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediatorForStorages<UniversalInputType>>().InstancePerDependency();
            builder.RegisterType<MediatorForOptions>().InstancePerDependency();
        }
    }
}