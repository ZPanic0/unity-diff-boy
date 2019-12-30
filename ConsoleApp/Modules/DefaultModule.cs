using Autofac;

namespace ConsoleApp.Modules
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Application>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
