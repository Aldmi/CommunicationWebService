﻿using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using BL.Services.InputData;
using BL.Services.MessageBroker;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Options;
using Microsoft.Extensions.Configuration;
using Worker.Background.Abstarct;
using Worker.Background.Concrete.HostingBackground;

namespace WebServer.AutofacModules
{
    public class InputDataAutofacModule<TIn> : Module
    {
        public string BackgroundName { get;  }
        public bool AutoStartBg { get;  }
        public ConsumerOption ConsumerOption { get;}



        #region ctor

        public InputDataAutofacModule(IConfigurationSection config)
        {
            var hh = config.GetSection("Topics").Value;
            var gg = config["Topics"];

            BackgroundName= config["Name"];
            AutoStartBg=  bool.Parse(config["AutoStart"]);
            ConsumerOption= new ConsumerOption
            {
                BrokerEndpoints = config["BrokerEndpoints"],
                GroupId = config["GroupId"],
           
            };
        }

        #endregion



        protected override void Load(ContainerBuilder builder)
        {
            //
            var consumerOption = new ConsumerOption
            {
                BrokerEndpoints = "localhost:9092",
                GroupId = "rx-consumer",
                Topics = new List<string> { "CommunicationWebService_InputData" }
            };
            var backgroundName = "messageBrokerConsumerBg";
            var autoStartBg = true;

            builder.RegisterType<GetInputDataService<TIn>>().InstancePerDependency();
            builder.RegisterType<HostingBackgroundSimple>()
                .Named<ISimpleBackground>(backgroundName)
                .WithParameters(new List<ResolvedParameter>
                {
                    new ResolvedParameter(
                        (pi, ctx) => (pi.ParameterType == typeof(string) && (pi.Name == "key")),
                        (pi, ctx) => backgroundName),
                    new ResolvedParameter(
                        (pi, ctx) => (pi.ParameterType == typeof(bool) && (pi.Name == "autoStart")),
                        (pi, ctx) => autoStartBg),
                }).SingleInstance();



            builder.RegisterType<ConsumerMessageBroker4InputData<TIn>>()
                .WithParameters(new List<ResolvedParameter>
                {
                    new ResolvedParameter(
                        (pi, ctx) => (pi.ParameterType == typeof(ISimpleBackground) && (pi.Name == "background")),
                        (pi, ctx) => ctx.ResolveNamed<ISimpleBackground>(backgroundName)),
                    new ResolvedParameter(
                        (pi, ctx) => (pi.ParameterType == typeof(int) && (pi.Name == "batchSize")),
                        (pi, ctx) => 100),           
                    new ResolvedParameter(
                        (pi, ctx) => (pi.ParameterType == typeof(IConsumer) && (pi.Name == "consumer")),
                        (pi, ctx) =>
                        {
                            var consumerFactory = ctx.Resolve<Func<ConsumerOption, IConsumer>>();
                            return consumerFactory(consumerOption);
                        })
                }).SingleInstance();        
        }
    }
}
