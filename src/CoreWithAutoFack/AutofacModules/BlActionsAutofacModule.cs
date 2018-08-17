using Autofac;
using BL.Services.Actions;

namespace WebServer.AutofacModules
{
    public class BlActionsAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DeviceActionService>().InstancePerDependency();    
        }
    }
}