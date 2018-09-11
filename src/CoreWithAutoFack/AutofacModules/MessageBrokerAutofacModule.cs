﻿using Autofac;
using BL.Services.MessageBroker;
using Infrastructure.EventBus.Concrete;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Consumer;

namespace WebServer.AutofacModules
{
    public class MessageBrokerAutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RxKafkaConsumer>().As<IConsumer>().InstancePerDependency();    
        }
    }
}


