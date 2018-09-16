using Autofac;
using BL.Services.Actions;

namespace WebServer.AutofacModules
{
    public class BlActionsAutofacModule<TIn> : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DeviceActionService<TIn>>().InstancePerDependency();    
            builder.RegisterType<BuildDeviceService<TIn>>().InstancePerDependency();          
        }
    }
}