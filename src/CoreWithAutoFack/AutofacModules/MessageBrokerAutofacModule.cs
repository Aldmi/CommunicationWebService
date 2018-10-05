using Autofac;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Consumer;
using Infrastructure.MessageBroker.Produser;

namespace WebServer.AutofacModules
{
    public class MessageBrokerAutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RxKafkaConsumer>().As<IConsumer>().InstancePerDependency();
            builder.RegisterType<KafkaProducer>().As<IProduser>().InstancePerDependency();
        }
    }
}


