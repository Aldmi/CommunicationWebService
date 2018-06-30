using Autofac;

namespace WebServer.AutofacModules
{
    public class ExchangeAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //var settingDevices = new List<DeviceSetting>
            //{
            //    new DeviceSetting
            //    {
            //        Name = "Dev1",
            //        TypeExchange = TypeExchange.SerialPort,
            //        PortName = "COM1"
            //    },
            //    new DeviceSetting
            //    {
            //        Name = "Dev2",
            //        TypeExchange = TypeExchange.SerialPort,
            //        PortName = "COM2"
            //    },
            //    new DeviceSetting
            //    {
            //        Name = "Dev3",
            //        TypeExchange = TypeExchange.Http,
            //        PortName = "192.168.1.1"
            //    }
            //};

            //foreach (var device in settingDevices)
            //{
            //    switch (device.TypeExchange)
            //    {
            //        case TypeExchange.SerialPort:
            //            builder.RegisterType<BaseExchangeSerialPort>().As<IExhangeBehavior>()
            //                .WithParameters(new List<ResolvedParameter>
            //                {      new ResolvedParameter(
            //                        (pi, ctx) => (pi.ParameterType == typeof(ISpService) && (pi.Name == "spService")),
            //                        (pi, ctx) => ctx.Resolve<IEnumerable<ISpService>>().FirstOrDefault(port=> port.PortName == device.PortName)),
            //                    new ResolvedParameter(
            //                        (pi, ctx) => (pi.ParameterType == typeof(IBackgroundService) && (pi.Name == "backgroundService")),
            //                        (pi, ctx) => ctx.Resolve<IEnumerable<IBackgroundService>>().FirstOrDefault(backgr=> backgr.KeyBackground.TypeExchange == TypeExchange.SerialPort && backgr.KeyBackground.Key == device.PortName)),
            //                })
            //                 //.OnActivated(e=>e.Instance.)
            //                .SingleInstance();
            //            break;

            //        case TypeExchange.TcpIp:
            //            break;

            //        case TypeExchange.Http:
            //            break;

            //        default:
            //            throw new ArgumentOutOfRangeException();
            //    }

            //}

        }
    }
}