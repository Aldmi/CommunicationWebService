using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using BL.Services;
using DAL.Abstract.Concrete;
using DAL.Abstract.Extensions;
using Exchange.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Concrete.SpWin;
using WebServer.AutofacModules;
using Worker.Background.Abstarct;
using Worker.Background.Concrete;

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

            var backgroundServices = scope.Resolve<BackgroundCollectionService>();
            foreach (var back in backgroundServices.Backgrounds)
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
            var backgroundServices = scope.Resolve<BackgroundCollectionService>();
            foreach (var back in backgroundServices.Backgrounds)
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
            var serialPortOptionRepository = scope.Resolve<ISerialPortOptionRepository>();
            var exchangeOptionRepository = scope.Resolve<IExchangeOptionRepository>();
            var serialPortCollectionService = scope.Resolve<SerialPortCollectionService>();
            var backgroundCollectionService = scope.Resolve<BackgroundCollectionService>();

            if (env.IsDevelopment())
            {
                //ИНИЦИАЛИЦИЯ РЕПОЗИТОРИЕВ
                serialPortOptionRepository.Initialize();
                exchangeOptionRepository.Initialize();
            }

            try
            {
                foreach (var spOption in serialPortOptionRepository.List())
                {
                    var keyTransport = new KeyTransport(spOption.Port, TransportType.SerialPort);
                    var sp = new SpWinSystemIo(spOption, keyTransport);
                    serialPortCollectionService.AddNew(keyTransport, sp);
                    var bg = new BackgroundScoped(scope.Resolve<IServiceScopeFactory>(), keyTransport);
                    backgroundCollectionService.AddNew(keyTransport, bg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }   
        }
    }
}
