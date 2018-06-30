using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Communication.SerialPort.Abstract;
using Communication.SerialPort.Concrete.Sp4Win;
using Communication.SerialPort.Option;
using Shared;
using Shared.Enums;
using Worker.Background.Abstarct;
using Worker.Background.Concrete.BackgroundSerialPort;

namespace WebServer.AutofacModules
{

    /// <summary>
    /// Каждому послед. порту создается свой IBackgroundService.
    /// </summary>
    public class SerialPortAutofacModule : Module
    {
        private readonly SerialPortsOption _spOptions;



        public SerialPortAutofacModule(SerialPortsOption spOptions)
        {
            _spOptions = spOptions;
        }




        protected override void Load(ContainerBuilder builder)
        {
            foreach (var spOption in _spOptions.Serials)
            {
                builder.RegisterType<SpWinDefault>().As<ISerailPort>()
                    .WithParameters(new List<Parameter>
                    {
                        new NamedParameter("option", spOption),
                    })
                    .SingleInstance();
              

                builder.RegisterType<BackgroundMasterSerialPort>().As<IBackgroundService>()
                    .WithParameters(new List<Parameter>
                    {
                       new NamedParameter("keyBackground", new KeyBackground {Key = spOption.Port, TypeExchange = TypeExchange.SerialPort}),
                    })
                    //.OnActivating(e => e.Instance.StartAsync(CancellationToken.None))
                    .SingleInstance();
            }





            //Named ругистрация----------------------------------------------------------
            //builder.RegisterType<SpService>().Named<ISpService>("COM1")
            //    .WithParameters(new List<Parameter> {
            //                new NamedParameter("portName", @"COM1"),
            //             }).SingleInstance();

            //builder.RegisterType<SpService>().Named<ISpService>("COM2")
            //    .WithParameters(new List<Parameter> {
            //        new NamedParameter("portName", @"COM2"),
            //    }).SingleInstance();

            //builder.RegisterType<BaseExchangeSerialPort>().Named<IExhangeBehavior>("Exch.COM1")
            //    .WithParameters(new List<ResolvedParameter>
            //    {      new ResolvedParameter(
            //                    (pi, ctx) => (pi.ParameterType == typeof(ISpService) && (pi.Name == "spService")),
            //                    (pi, ctx) => ctx.ResolveNamed<ISpService>("COM1"))
            //    }).SingleInstance();
            //------------------------------------------------------------------------------



            //builder.RegisterType<CompositerTrainRecRepositoryDecorator>().Keyed<ITrainTableRecRepository>(TrainRecType.LocalMain)
            //    .WithParameters(new List<ResolvedParameter> {
            //        new ResolvedParameter(
            //            (pi, ctx) => (pi.ParameterType == typeof(ITrainTableRecRepository) && (pi.Name == "trainTableRecRep")),
            //            (pi, ctx) => ctx.ResolveNamed<ITrainTableRecRepository>("LocalMain")),
            //        new ResolvedParameter(
            //            (pi, ctx) => (pi.ParameterType == typeof(ITrainTypeByRyleRepository) && (pi.Name == "trainTypeByRyleRep")),
            //            (pi, ctx) => ctx.Resolve<ITrainTypeByRyleRepository>())
            //    }).InstancePerLifetimeScope();



            //builder.RegisterType<XmlSerializeTrainTableRecRepository>().Named<ITrainTableRecRepository>("LocalMain")
            //    .WithParameters(new List<Parameter> { new NamedParameter("key", @"LocalMain"),
            //        new NamedParameter("folderName", @"XmlSerialize"),
            //        new NamedParameter("fileName", @"TrainTableRec.xml")}).InstancePerLifetimeScope();

        }













        //protected override void Load(ContainerBuilder builder)
        //{
        //    // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
        //    // It was then registered with Autofac using the Populate method in ConfigureServices.
        //    //builder.Register(c => new ValuesService(c.Resolve<ILogger<ValuesService>>()))
        //    //    .As<IValuesService>()
        //    //    .InstancePerLifetimeScope();

    
        //    builder.RegisterType<SpService>().As<ISpService>()
        //        .WithParameters(new List<Parameter> {
        //            new NamedParameter("portName", @"COM1"),
        //        }).SingleInstance();



        //    //Named ругистрация----------------------------------------------------------
        //    //builder.RegisterType<SpService>().Named<ISpService>("COM1")
        //    //    .WithParameters(new List<Parameter> {
        //    //                new NamedParameter("portName", @"COM1"),
        //    //             }).SingleInstance();

        //    //builder.RegisterType<SpService>().Named<ISpService>("COM2")
        //    //    .WithParameters(new List<Parameter> {
        //    //        new NamedParameter("portName", @"COM2"),
        //    //    }).SingleInstance();

        //    //builder.RegisterType<BaseExchangeSerialPort>().Named<IExhangeBehavior>("Exch.COM1")
        //    //    .WithParameters(new List<ResolvedParameter>
        //    //    {      new ResolvedParameter(
        //    //                    (pi, ctx) => (pi.ParameterType == typeof(ISpService) && (pi.Name == "spService")),
        //    //                    (pi, ctx) => ctx.ResolveNamed<ISpService>("COM1"))
        //    //    }).SingleInstance();
        //   //------------------------------------------------------------------------------



        //    //builder.RegisterType<CompositerTrainRecRepositoryDecorator>().Keyed<ITrainTableRecRepository>(TrainRecType.LocalMain)
        //    //    .WithParameters(new List<ResolvedParameter> {
        //    //        new ResolvedParameter(
        //    //            (pi, ctx) => (pi.ParameterType == typeof(ITrainTableRecRepository) && (pi.Name == "trainTableRecRep")),
        //    //            (pi, ctx) => ctx.ResolveNamed<ITrainTableRecRepository>("LocalMain")),
        //    //        new ResolvedParameter(
        //    //            (pi, ctx) => (pi.ParameterType == typeof(ITrainTypeByRyleRepository) && (pi.Name == "trainTypeByRyleRep")),
        //    //            (pi, ctx) => ctx.Resolve<ITrainTypeByRyleRepository>())
        //    //    }).InstancePerLifetimeScope();



        //    //builder.RegisterType<XmlSerializeTrainTableRecRepository>().Named<ITrainTableRecRepository>("LocalMain")
        //    //    .WithParameters(new List<Parameter> { new NamedParameter("key", @"LocalMain"),
        //    //        new NamedParameter("folderName", @"XmlSerialize"),
        //    //        new NamedParameter("fileName", @"TrainTableRec.xml")}).InstancePerLifetimeScope();

        //}
    }
}