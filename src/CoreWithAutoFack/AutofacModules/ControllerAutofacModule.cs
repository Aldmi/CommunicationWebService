using Autofac;

namespace WebServer.AutofacModules
{
    public class ControllerAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<ValuesController>()
            //    .PropertiesAutowired()
            //    .WithParameters(new List<ResolvedParameter>
            //{      new ResolvedParameter(
            //        (pi, ctx) => (pi.ParameterType == typeof(ISpService) && (pi.Name == "spService")),
            //        (pi, ctx) => ctx.ResolveNamed<ISpService>("COM2"))
            //}).InstancePerDependency();
        }
    }
}