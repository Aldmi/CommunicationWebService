using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using BL.Services.InputData;
using BL.Services.MessageBroker;
using BL.Services.Storages;
using DAL.Abstract.Concrete;
using DAL.Abstract.Extensions;
using Infrastructure.MessageBroker.Abstract;
using Infrastructure.MessageBroker.Consumer;
using InputDataModel.Autodictor.Model;
using Logger.Abstract.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            var connectionString = AppConfiguration.GetConnectionString("OptionDbConnection");
            builder.RegisterModule(new RepositoryAutofacModule(connectionString));
            builder.RegisterModule(new EventBusAutofacModule());
            builder.RegisterModule(new ControllerAutofacModule());
            builder.RegisterModule(new MessageBrokerAutofacModule());
            builder.RegisterModule(new LogerAutofacModule());

            var inputDataName = AppConfiguration["InputDataModel"];
            switch (inputDataName)
            {
                case "AdInputType":
                    builder.RegisterModule(new DataProviderExchangeAutofacModule<AdInputType>());
                    builder.RegisterModule(new BlStorageAutofacModule<AdInputType>());
                    builder.RegisterModule(new BlActionsAutofacModule<AdInputType>());
                    builder.RegisterModule(new MediatorsAutofacModule<AdInputType>());
                    builder.RegisterModule(new InputDataAutofacModule<AdInputType>());
                    break;

                case "OtherInputType":
                    throw new NotImplementedException();
            }
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

            var exchangeServices = scope.Resolve<ExchangeStorageService<AdInputType>>();
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

            var exchangeServices = scope.Resolve<ExchangeStorageService<AdInputType>>();
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

                    //DEBUG CRUD----------------------------------------------------------------
                    var singleElem= serialPortOptionRepository.GetSingle(option => option.Port == "COM1");
                    var httpElem = httpOptionRepository.GetSingle(option => option.Name == "Http table 1");
                    var tcpIpElem = tcpIpOptionRepository.GetSingle(option => option.Name == "RemoteTcpIpTable 2");
                    var exchangeElem = exchangeOptionRepository.GetSingle(option => option.Key == "SP_COM2_Vidor2");
                    //TODO: проверить остальные CRUD операции
                    //-----------------------------------------------------------------------------
                    //DEBUG MessageBroker---------------------------------------------------------
                    var consumerMessageBroker4InputData = scope.Resolve<ConsumerMessageBroker4InputData<AdInputType>>();

                    //START        
                    consumerMessageBroker4InputData.Start().Wait();
                    Console.WriteLine("START CONSUMER >>>>>>");
                    await Task.Delay(3000);

                    //STOP
                    await consumerMessageBroker4InputData.StopAsync(CancellationToken.None);               
                    Console.WriteLine("STOP CONSUMER <<<<<<<");
                    await Task.Delay(3000);

                    //START
                    consumerMessageBroker4InputData.Start().Wait();
                    Console.WriteLine("START CONSUMER >>>>>>");
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
