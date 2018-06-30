using System.Collections.Generic;
using System.Threading;
using Autofac;
using Communication.SerialPort.Option;
using CoreWithAutoFack.SettingsCommunication.Model;
using Exchange.MasterSerialPort.Option;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.AddTransient<IConfiguration>(provider=> AppConfiguration);
            services.AddMvc().AddControllersAsServices();
            services.AddOptions();

            services.Configure<SerialPortsOption>(AppConfiguration);
            services.Configure<DevicesWithSpOptions>(AppConfiguration);
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            var serialPortsOption= MoqSerialPortsOption.GetSerialPortsOption();
            builder.RegisterModule(new SerialPortAutofacModule(serialPortsOption));

            var exchSerialPortsOption = MoqExchangeMasterSerialPortOptions.GetExchangeMasterSerialPortOptions();
            builder.RegisterModule(new ExchangeMasterSerialPortAutofacModule(exchSerialPortsOption));

            builder.RegisterModule(new ControllerAutofacModule());
        }



        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              ILifetimeScope scope,
                              IConfiguration config,
                              IOptions<SerialPortsOption> optionsSettingModel,
                              IOptions<DevicesWithSpOptions> optionsDevicesWithSp)
        {
            var spPorts = optionsSettingModel.Value;
            var httpDev = optionsDevicesWithSp.Value;

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

            //lifetimeApp.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }

        private void ApplicationStarted(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            var backgroundServices = scope.Resolve<IEnumerable<IBackgroundService>>();
            foreach (var back in backgroundServices)
            {
                lifetimeApp.ApplicationStarted.Register(() => back.StartAsync(CancellationToken.None));
            }
        }

        private void ApplicationStopping(IApplicationLifetime lifetimeApp, ILifetimeScope scope)
        {
            var backgroundServices = scope.Resolve<IEnumerable<IBackgroundService>>();
            foreach (var back in backgroundServices)
            {
                lifetimeApp.ApplicationStopping.Register(() => back.StopAsync(CancellationToken.None));
            }
        }
    }
}
