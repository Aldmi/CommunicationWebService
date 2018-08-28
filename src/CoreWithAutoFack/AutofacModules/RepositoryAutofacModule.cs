using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using DAL.Abstract.Concrete;
using DAL.EFCore.Repository;

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
            builder.RegisterType<EfSerialPortOptionRepository>().As<ISerialPortOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .InstancePerLifetimeScope();

            builder.RegisterType<EfTcpIpOptionRepository>().As<ITcpIpOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .InstancePerLifetimeScope();

            builder.RegisterType<EfHttpOptionRepository>().As<IHttpOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .InstancePerLifetimeScope();

            builder.RegisterType<EfExchangeOptionRepository>().As<IExchangeOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .InstancePerLifetimeScope();

            builder.RegisterType<EfDeviceOptionRepository>().As<IDeviceOptionRepository>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("connectionString", _connectionString),
                })
                .InstancePerLifetimeScope();
        }
    }
}