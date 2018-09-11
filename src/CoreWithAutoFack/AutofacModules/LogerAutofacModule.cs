using Autofac;
using Infrastructure.EventBus.Concrete;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Consumer;
using Logger.Abstract.Abstract;
using Logger.Nlog;

namespace WebServer.AutofacModules
{
    public class LogerAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NlogWrapper>().As<ILogger>().InstancePerDependency();
        }
    }
}


