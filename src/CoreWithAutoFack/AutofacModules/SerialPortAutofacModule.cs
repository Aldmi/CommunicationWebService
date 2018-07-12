using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Shared;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Abstract;
using Transport.SerialPort.Concrete.SpWin;
using Transport.SerialPort.Option;
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
                var keyTransport = new KeyTransport(spOption.Port, TransportType.SerialPort);//Ключ - это название порта

                builder.RegisterType<SpWinSystemIo>().As<ISerailPort>()
                    .WithParameters(new List<Parameter>
                    {
                        new NamedParameter("option", spOption),
                        new NamedParameter("keyTransport", keyTransport)
                    })
                    .SingleInstance();
              
                builder.RegisterType<BackgroundMasterSerialPort>().As<IBackgroundService>()
                    .WithParameters(new List<Parameter>
                    {
                       new NamedParameter("keyTransport", keyTransport),
                    })
                    .SingleInstance();
            }

        }
    }
}