﻿using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Exchange.Base;
using Exchange.MasterSerialPort;
using Exchange.MasterSerialPort.Option;
using Shared;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Abstract;
using Worker.Background.Abstarct;

namespace WebServer.AutofacModules
{
    public class ExchangeMasterSerialPortAutofacModule : Module
    {
        private readonly ExchangeMasterSpOptions _exchangeMasterSpOptions;

        public ExchangeMasterSerialPortAutofacModule(ExchangeMasterSpOptions exchangeMasterSpOptions)
        {
            _exchangeMasterSpOptions = exchangeMasterSpOptions;
        }


        protected override void Load(ContainerBuilder builder)
        {
            foreach (var exchMSpOption in _exchangeMasterSpOptions.ExchangesMasterSp)
            {
                var keyExchange = new KeyExchange(exchMSpOption.PortName, TransportType.SerialPort);//TODO: искать по ключу

                builder.RegisterType<ByRulesExchangeSerialPort>().As<IExchange>()
                    .WithParameters(new List<ResolvedParameter>
                    {      new ResolvedParameter(
                                       (pi, ctx) => (pi.ParameterType == typeof(ISerailPort) && (pi.Name == "serailPort")),
                                       (pi, ctx) => ctx.Resolve<IEnumerable<ISerailPort>>().FirstOrDefault(port=> port.SerialOption.Port == exchMSpOption.PortName)),
                           new ResolvedParameter(
                                       (pi, ctx) => (pi.ParameterType == typeof(IBackgroundService) && (pi.Name == "backgroundService")),
                                       (pi, ctx) => ctx.Resolve<IEnumerable<IBackgroundService>>().FirstOrDefault(backgr=> backgr.KeyExchange.TransportType == TransportType.SerialPort && backgr.KeyExchange.Key == exchMSpOption.PortName)),
                           new ResolvedParameter(
                                       (pi, ctx) => (pi.ParameterType == typeof(ExchangeMasterSpOption) && (pi.Name == "exchangeMasterSpOption")),
                                       (pi, ctx) => exchMSpOption)

                    })
                    .SingleInstance();
            }
        }
    }
}