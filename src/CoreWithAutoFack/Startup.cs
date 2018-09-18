using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using BL.Services.Actions;
using BL.Services.MessageBroker;
using BL.Services.Storages;
using DAL.Abstract.Concrete;
using DAL.Abstract.Extensions;
using InputDataModel.Autodictor.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebServer.AutofacModules;
using Worker.Background.Abstarct;

namespace WebServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
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
                    builder.RegisterModule(new InputDataAutofacModule<AdInputType>(AppConfiguration.GetSection("MessageBrokerConsumer4InData")));
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
            //ЗАПУСК БЕКГРАУНДА ОПРОСА ШИНЫ ДАННЫХ
            scope.Resolve<ConsumerMessageBroker4InputData<AdInputType>>();//перед запусклм bg нужно создать ConsumerMessageBroker4InputData
            bool.TryParse(AppConfiguration["MessageBrokerConsumer4InData:AutoStart"], out var autoStart);
            if (autoStart)
            {
                var backgroundName = AppConfiguration["MessageBrokerConsumer4InData:Name"];
                var bgConsumer = scope.ResolveNamed<ISimpleBackground>(backgroundName);
                lifetimeApp.ApplicationStarted.Register(() => bgConsumer.StartAsync(CancellationToken.None));
            }

            //ЗАПУСК БЕКГРАУНДА ОПРОСА УСТРОЙСТВ
            var backgroundServices = scope.Resolve<BackgroundStorageService>();
            foreach (var back in backgroundServices.Values.Where(bg => bg.AutoStart))
            {
                lifetimeApp.ApplicationStarted.Register(() => back.StartAsync(CancellationToken.None));
            }

            //ЗАПУСК КОННЕКТА УСТРОЙСТВ
            var exchangeServices = scope.Resolve<ExchangeStorageService<AdInputType>>();
            foreach (var exchange in exchangeServices.Values.Where(exch => exch.AutoStart))
            {
                lifetimeApp.ApplicationStarted.Register(async () => await exchange.CycleReOpened());
            }
        }


        private void ApplicationStopping(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            //ОСТАНОВ БЕКГРАУНДА ОПРОСА ШИНЫ ДАННЫХ
            var backgroundName = AppConfiguration["MessageBrokerConsumer4InData:Name"];
            var bgConsumer = scope.ResolveNamed<ISimpleBackground>(backgroundName);
            lifetimeApp.ApplicationStopping.Register(() => bgConsumer.StopAsync(CancellationToken.None));

            //ОСТАНОВ ЗАПУЩЕННОГО БЕКГРАУНДА ОПРОСА УСТРОЙСТВ
            var backgroundServices = scope.Resolve<BackgroundStorageService>();
            foreach (var back in backgroundServices.Values.Where(bg => bg.IsStarted))
            {
                lifetimeApp.ApplicationStopping.Register(() => back.StopAsync(CancellationToken.None));
            }

            //ОСТАНОВ КОННЕКТА УСТРОЙСТВ
            var exchangeServices = scope.Resolve<ExchangeStorageService<AdInputType>>();
            foreach (var exchange in exchangeServices.Values.Where(exch => !exch.IsOpen))
            {
                lifetimeApp.ApplicationStopping.Register(() => exchange.CycleReOpenedCancelation());
            }
        }


        private void ApplicationStopped(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            lifetimeApp.ApplicationStopped.Register(() => { });
        }


        /// <summary>
        /// Инициализация системы.
        /// </summary>
        private async Task InitializeAsync(ILifetimeScope scope)
        {
            var env = scope.Resolve<IHostingEnvironment>();      
            if (env.IsDevelopment()) //TODO: добавить переменную окружения OS (win/linux)
            {
                //ИНИЦИАЛИЦИЯ РЕПОЗИТОРИЕВ--------------------------------------------------------
                try
                {
                    var serialPortOptionRepository = scope.Resolve<ISerialPortOptionRepository>();
                    var tcpIpOptionRepository = scope.Resolve<ITcpIpOptionRepository>();
                    var httpOptionRepository = scope.Resolve<IHttpOptionRepository>();
                    var exchangeOptionRepository = scope.Resolve<IExchangeOptionRepository>();
                    var deviceOptionRepository = scope.Resolve<IDeviceOptionRepository>();

                    await serialPortOptionRepository.InitializeAsync();
                    await tcpIpOptionRepository.InitializeAsync();
                    await httpOptionRepository.InitializeAsync();
                    await exchangeOptionRepository.InitializeAsync();
                    await deviceOptionRepository.InitializeAsync();

                    //DEBUG CRUD----------------------------------------------------------------
                    //var singleElem = serialPortOptionRepository.GetSingle(option => option.Port == "COM1");
                    //var httpElem = httpOptionRepository.GetSingle(option => option.Name == "Http table 1");
                    //var tcpIpElem = tcpIpOptionRepository.GetSingle(option => option.Name == "RemoteTcpIpTable 2");
                    //var exchangeElem = exchangeOptionRepository.GetSingle(option => option.Key == "SP_COM2_Vidor2");
                    //TODO: проверить остальные CRUD операции
                    //-----------------------------------------------------------------------------
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            //СОЗДАНИЕ СПИСКА УСТРОЙСТВ НА БАЗЕ ОПЦИЙ--------------------------------------------------
            try
            {
                var buildDeviceService = scope.Resolve<BuildDeviceService<AdInputType>>();
                await buildDeviceService.BuildAllDevices();
            }
            catch (AggregateException ex)
            {
                //LOG
                foreach (var innerException in ex.InnerExceptions)
                {
                    Console.WriteLine(innerException.ToString());
                }
            }
        }
    }
}
