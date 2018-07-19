using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using BL.Services;
using BL.Services.Storage;
using DAL.Abstract.Concrete;
using DAL.Abstract.Extensions;
using Exchange.Base;
using Exchange.MasterSerialPort;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Abstract;
using Transport.SerialPort.Concrete.SpWin;
using WebServer.AutofacModules;
using Worker.Background.Abstarct;
using Worker.Background.Concrete;
using Worker.Background.Concrete.HostingBackground;

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

            //services.Configure<SerialPortsOption>(AppConfiguration);
            //services.Configure<DevicesWithSpOptions>(AppConfiguration);
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            var connectionString = "Test connection String";
            builder.RegisterModule(new RepositoryAutofacModule(connectionString));


            builder.RegisterModule(new BlServiceAutofacModule());
            builder.RegisterModule(new EventBusAutofacModule());
            builder.RegisterModule(new ControllerAutofacModule());
        }



        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILifetimeScope scope,
            IConfiguration config)
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


        private void ConfigurationBackgroundProcess(IApplicationBuilder app, ILifetimeScope scope)
        {
            var lifetimeApp = app.ApplicationServices.GetService<IApplicationLifetime>();
            ApplicationStarted(lifetimeApp, scope);
            ApplicationStopping(lifetimeApp, scope);
            ApplicationStopped(lifetimeApp, scope);
        }


        private void ApplicationStarted(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            Initialize(scope);

            var backgroundServices = scope.Resolve<BackgroundStorageService>();
            foreach (var back in backgroundServices.Values)
            {
                lifetimeApp.ApplicationStarted.Register(() => back.StartAsync(CancellationToken.None));
            }

            var exchangeServices = scope.Resolve<ExchangeStorageService>();
            foreach (var exchange in exchangeServices.Values)
            {
                lifetimeApp.ApplicationStarted.Register(async () => await exchange.CycleReOpened());
            }
        }


        private void ApplicationStopping(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            var backgroundServices = scope.Resolve<BackgroundStorageService>();
            foreach (var back in backgroundServices.Values)
            {
                lifetimeApp.ApplicationStopping.Register(() => back.StopAsync(CancellationToken.None));
            }

            var exchangeServices = scope.Resolve<ExchangeStorageService>();
            foreach (var exchange in exchangeServices.Values)
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
        private void Initialize(IComponentContext scope)
        {
            var env = scope.Resolve<IHostingEnvironment>();
            var serialPortOptionRepository = scope.Resolve<ISerialPortOptionRepository>();
            var exchangeOptionRepository = scope.Resolve<IExchangeOptionRepository>();
            var serialPortCollectionService = scope.Resolve<SerialPortStorageService>();
            var backgroundCollectionService = scope.Resolve<BackgroundStorageService>();
            var exchangeCollectionService = scope.Resolve<ExchangeStorageService>();

            if (env.IsDevelopment())
            {
                //ИНИЦИАЛИЦИЯ РЕПОЗИТОРИЕВ
                serialPortOptionRepository.Initialize();
                exchangeOptionRepository.Initialize();
            }

            try
            {
                //ADD SERIAL PORTS--------------------------------------------------------------------
                foreach (var spOption in serialPortOptionRepository.List())
                {
                    var keyTransport = new KeyTransport(spOption.Port, TransportType.SerialPort);
                    var sp = new SpWinSystemIo(spOption, keyTransport);
                    var bg = new HostingBackgroundTransport(keyTransport);
                    serialPortCollectionService.AddNew(keyTransport, sp);
                    backgroundCollectionService.AddNew(keyTransport, bg);
                }

                //ADD EXCHANGES------------------------------------------------------------------------
                foreach (var exchOption in exchangeOptionRepository.List())
                {
                    var keyTransport= new KeyTransport(exchOption.KeyTransport);
                    var sp= serialPortCollectionService.Get(keyTransport);
                    var bg= backgroundCollectionService.Get(keyTransport);
                    var exch = new ByRulesExchangeSerialPort(sp, bg, exchOption);
                    exchangeCollectionService.AddNew(keyTransport, exch);
                }
            }
            catch (Exception e)
            {
                //LOG
                Console.WriteLine(e);
                throw;
            }   
        }
    }
}
