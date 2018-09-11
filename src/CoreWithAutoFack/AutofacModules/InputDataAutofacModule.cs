using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using BL.Services.InputData;
using BL.Services.MessageBroker;
using Infrastructure.EventBus.Abstract;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Consumer;
using Infrastructure.MessageBroker.Options;

namespace WebServer.AutofacModules
{
    public class InputDataAutofacModule<TIn> : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GetInputDataService<TIn>>().InstancePerDependency();


            var consumerOption = new ConsumerOption
            {
                BrokerEndpoints = "localhost:9092",
                GroupId = "rx-consumer",
                Topics = new List<string> { "CommunicationWebService_InputData" }
            };
            //builder.RegisterType<ConsumerMessageBroker4InputData<TIn>>()
            //       .WithParameters(new List<ResolvedParameter>
            //            {      new ResolvedParameter(
            //                    (pi, ctx) => (pi.ParameterType == typeof(int) && (pi.Name == "batchSize")),
            //                    (pi, ctx) => 100),
            //                new ResolvedParameter(
            //                    (pi, ctx) => (pi.ParameterType == typeof(KafkaConsumerOption) && (pi.Name == "consumerOption")),
            //                    (pi, ctx) => consumerOption),
            //                //DEBUG-----------------------------------------------------------------------------
            //                new ResolvedParameter(
            //                (pi, ctx) => (pi.ParameterType == typeof(IConsumer) && (pi.Name == "consumer")),
            //                (pi, ctx) =>
            //                {
            //                    var consumerFactory = ctx.Resolve<Func<KafkaConsumerOption, IConsumer>>();
            //                    return consumerFactory(consumerOption);
            //                })
            //            }).SingleInstance();


            //DEBUG-----------------------------------------------------------------------------
            builder.RegisterType<ConsumerMessageBroker4InputData<TIn>>()
                .WithParameters(new List<ResolvedParameter>
                {      new ResolvedParameter(
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
