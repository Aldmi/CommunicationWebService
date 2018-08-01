using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using DAL.Abstract.Concrete;
using DAL.InMemory.Repository;

namespace WebServer.AutofacModules
{
    public class RepositoryAutofacModule : Module
    {
        private readonly string _connectionString;



        #region ctor

        public RepositoryAutofacModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion



        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemorySerialPortOptionRepository>().As<ISerialPortOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .SingleInstance();//.InstancePerLifetimeScope();

            builder.RegisterType<InMemoryTcpIpOptionRepository>().As<ITcpIpOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .SingleInstance();

            builder.RegisterType<InMemoryHttpOptionRepository>().As<IHttpOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .SingleInstance();

            builder.RegisterType<InMemoryExchangeOptionRepository>().As<IExchangeOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .SingleInstance();

            builder.RegisterType<InMemoryDeviceOptionRepository>().As<IDeviceOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .SingleInstance();
        }
    }
}