using System.Collections.Generic;
using System.Threading;
using Autofac;
using DAL.Abstract.Concrete;
using DAL.Abstract.Extensions;
using DAL.InMemory.Repository;
using Exchange.Base;
using Exchange.MasterSerialPort.Option;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Transport.SerialPort.Option;
using WebServer.AutofacModules;
using WebServer.SettingsCommunication.Model;
using Worker.Background.Abstarct;

namespace WebServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("SettingsCommunication/Setting.json", optional: false, reloadOnChange: false);
            AppConfiguration = builder.Build();
        }


        public IConfiguration AppConfiguration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfiguration>(provider => AppConfiguration);
            services.AddMvc().AddControllersAsServices();
            services.AddOptions();

            services.Configure<SerialPortsOption>(AppConfiguration);
            services.Configure<DevicesWithSpOptions>(AppConfiguration);
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            var connectionString = "Test connection String";
            builder.RegisterModule(new RepositoryAutofacModule(connectionString));


            var serialPortsOption = MoqSerialPortsOption.GetSerialPortsOption();
            builder.RegisterModule(new SerialPortAutofacModule(serialPortsOption));

            var exchSerialPortsOption = MoqExchangeMasterSerialPortOptions.GetExchangeMasterSerialPortOptions();
            builder.RegisterModule(new ExchangeMasterSerialPortAutofacModule(exchSerialPortsOption));

            builder.RegisterModule(new EventBusAutofacModule());

            builder.RegisterModule(new ControllerAutofacModule());
        }



        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILifetimeScope scope,
            IConfiguration config,
            IOptions<SerialPortsOption> optionsSettingModel,
            IOptions<DevicesWithSpOptions> optionsDevicesWithSp)
        {
            //var spPorts = optionsSettingModel.Value;
            //var httpDev = optionsDevicesWithSp.Value;

            ConfigurationBackgroundProcess(app, scope);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }


        public void ConfigurationBackgroundProcess(IApplicationBuilder app, ILifetimeScope scope)
        {
            var lifetimeApp = app.ApplicationServices.GetService<IApplicationLifetime>();
            ApplicationStarted(lifetimeApp, scope);
            ApplicationStopping(lifetimeApp, scope);
            ApplicationStopped(lifetimeApp, scope);
        }

        private void ApplicationStarted(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            Initialize(scope);

            var backgroundServices = scope.Resolve<IEnumerable<IBackgroundService>>();
            foreach (var back in backgroundServices)
            {
                lifetimeApp.ApplicationStarted.Register(() => back.StartAsync(CancellationToken.None));
            }

            var exchangeServices = scope.Resolve<IEnumerable<IExchange>>();
            foreach (var exchange in exchangeServices)
            {
                lifetimeApp.ApplicationStarted.Register(async () => await exchange.CycleReOpened());
            }
        }

        private void ApplicationStopping(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            var backgroundServices = scope.Resolve<IEnumerable<IBackgroundService>>();
            foreach (var back in backgroundServices)
            {
                lifetimeApp.ApplicationStopping.Register(() => back.StopAsync(CancellationToken.None));
            }

            var exchangeServices = scope.Resolve<IEnumerable<IExchange>>();
            foreach (var exchange in exchangeServices)
            {
                lifetimeApp.ApplicationStopping.Register(() => exchange.CycleReOpenedCancelation());
            }
        }

        private void ApplicationStopped(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            lifetimeApp.ApplicationStopped.Register(() => {});
        }


        /// <summary>
        /// Инициализация системы.
        /// </summary>
        private void Initialize(ILifetimeScope scope)
        {
            var env = scope.Resolve<IHostingEnvironment>();
            if (env.IsDevelopment())
            {
                var serialPortOptionRepository = scope.Resolve<ISerialPortOptionRepository>();
                serialPortOptionRepository.Initialize();
            }
        }
    }
}
