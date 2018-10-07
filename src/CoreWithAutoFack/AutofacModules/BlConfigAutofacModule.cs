using Autofac;
using BL.Services.Config;
using BL.Services.Storages;

namespace WebServer.AutofacModules
{
    /// <summary>
    /// Регистрируем сервисы хранения бизнесс логики. (оперативные данные, хранятся в памяти (CuncurrentDictionary))
    /// </summary>
    public class BlConfigAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppConfigWrapper>().SingleInstance(); 
        }
    }
}