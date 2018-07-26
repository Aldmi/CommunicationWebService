using System;
using System.Linq;
using System.Threading;
using Autofac;
using AutoMapper;
using BL.Services.Storage;
using DAL.Abstract.Concrete;
using DAL.Abstract.Extensions;
using Exchange.MasterSerialPort;
using Infrastructure.EventBus.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Concrete.SpWin;
using WebServer.AutofacModules;
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
            services.AddMvc()
                .AddControllersAsServices()
                .AddJsonOptions(o =>
            {              
                o.SerializerSettings.Formatting = Formatting.Indented;
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }); 
            services.AddOptions();
            services.AddAutoMapper();
        

            //services.Configure<SerialPortsOption>(AppConfiguration);
            //services.Configure<DevicesWithSpOptions>(AppConfiguration);
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            var connectionString = "Test connection String";
            builder.RegisterModule(new RepositoryAutofacModule(connectionString));

            builder.RegisterModule(new BlStorageAutofacModule());
            builder.RegisterModule(new MediatorsAutofacModule());
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
            var deviceOptionRepository = scope.Resolve<IDeviceOptionRepository>();
            var serialPortStorageService = scope.Resolve<SerialPortStorageService>();
            var backgroundStorageService = scope.Resolve<BackgroundStorageService>();
            var exchangeStorageService = scope.Resolve<ExchangeStorageService>();
            var deviceStorageService = scope.Resolve<DeviceStorageService>();
            var eventBus = scope.Resolve<IEventBus>();

            try
            {
                if (env.IsDevelopment()) //TODO: добавить переменную окружения OS (win/linux)
                {
                    //ИНИЦИАЛИЦИЯ РЕПОЗИТОРИЕВ--------------------------------------------------------
                    serialPortOptionRepository.Initialize();
                    exchangeOptionRepository.Initialize();
                    deviceOptionRepository.Initialize();
                }

                //ADD SERIAL PORTS--------------------------------------------------------------------
                foreach (var spOption in serialPortOptionRepository.List())
                {
                    var keyTransport = new KeyTransport(spOption.Port, TransportType.SerialPort);
                    var sp = new SpWinSystemIo(spOption, keyTransport);
                    var bg = new HostingBackgroundTransport(keyTransport);
                    serialPortStorageService.AddNew(keyTransport, sp);
                    backgroundStorageService.AddNew(keyTransport, bg);
                }

                //ADD EXCHANGES------------------------------------------------------------------------
                foreach (var exchOption in exchangeOptionRepository.List())
                {
                    var keyTransport= exchOption.KeyTransport;
                    var sp= serialPortStorageService.Get(keyTransport);
                    var bg= backgroundStorageService.Get(keyTransport);
                    if (sp == null || bg == null) continue;
                    var exch = new ByRulesExchangeSerialPort(sp, bg, exchOption);
                    exchangeStorageService.AddNew(keyTransport, exch);
                }

                //ADD DEVICES--------------------------------------------------------------------------
                foreach (var deviceOption in deviceOptionRepository.List())
                {
                    var excanges= exchangeStorageService.GetMany(deviceOption.KeyTransports).ToList();
                    var device= new Device.Base.Device(deviceOption, excanges, eventBus);
                    deviceStorageService.AddNew(deviceOption.Id, device);
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
