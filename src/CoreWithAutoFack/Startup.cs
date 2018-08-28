using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using BL.Services.Storages;
using DAL.Abstract.Concrete;
using DAL.Abstract.Extensions;
using Infrastructure.EventBus.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebServer.AutofacModules;

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
            var connectionString = AppConfiguration.GetConnectionString("OptionDbConnection");
            builder.RegisterModule(new RepositoryAutofacModule(connectionString));

            builder.RegisterModule(new BlStorageAutofacModule());
            builder.RegisterModule(new BlActionsAutofacModule());
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

            ConfigurationBackgroundProcessAsync(app, scope);
            InitializeAsync(scope).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }


        private void ConfigurationBackgroundProcessAsync(IApplicationBuilder app, ILifetimeScope scope)
        {
            var lifetimeApp = app.ApplicationServices.GetService<IApplicationLifetime>();
            ApplicationStarted(lifetimeApp, scope);
            ApplicationStopping(lifetimeApp, scope);
            ApplicationStopped(lifetimeApp, scope);
        }


        private void ApplicationStarted(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
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
        private async Task InitializeAsync(ILifetimeScope scope)
        {
            var env = scope.Resolve<IHostingEnvironment>();
            var serialPortOptionRepository = scope.Resolve<ISerialPortOptionRepository>();
            var tcpIpOptionRepository = scope.Resolve<ITcpIpOptionRepository>();
            var httpOptionRepository = scope.Resolve<IHttpOptionRepository>();
            var exchangeOptionRepository = scope.Resolve<IExchangeOptionRepository>();
            var deviceOptionRepository = scope.Resolve<IDeviceOptionRepository>();

            try
            {
                if (env.IsDevelopment()) //TODO: добавить переменную окружения OS (win/linux)
                {
                    //ИНИЦИАЛИЦИЯ РЕПОЗИТОРИЕВ--------------------------------------------------------
                    try
                    {
                        await serialPortOptionRepository.InitializeAsync();
                        await tcpIpOptionRepository.InitializeAsync();
                        await httpOptionRepository.InitializeAsync();
                        await exchangeOptionRepository.InitializeAsync();
                        await deviceOptionRepository.InitializeAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    //DEBUG------------------------------------------
                    var singleElem= serialPortOptionRepository.GetSingle(option => option.Port == "COM1");//spOption => spOption.Port == "COM1"
                    var httpElem = httpOptionRepository.GetSingle(option => option.Name == "Http table 1");
                    var tcpIpElem = tcpIpOptionRepository.GetSingle(option => option.Name == "RemoteTcpIpTable 2");
                    var exchangeElem = exchangeOptionRepository.GetSingle(option => option.Key == "SP_COM1_Vidor1");

                    //TODO: проверить остальные CRUD операции
                    //DEBUG------------------------------------------
                }
            }
            catch (Exception e)
            {
                //LOG
                Console.WriteLine(e);
                //throw;
            }
        }
    }
}
