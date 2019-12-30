using Autofac;
using MediatR;

namespace ConsoleApp.Modules
{
    class MediatRModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .SingleInstance();

            builder
                .Register<ServiceFactory>(context =>
                    context.Resolve<IComponentContext>().Resolve);

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AsImplementedInterfaces();
        }
    }
}
