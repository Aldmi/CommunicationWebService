using Autofac;
using BL.Services.Actions;
using InputDataModel.Autodictor.InputData;

namespace WebServer.AutofacModules
{
    public class BlActionsAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DeviceActionService<UniversalInputType>>().InstancePerDependency();    
        }
    }
}