using Autofac;
using ConsoleApp.Modules;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args) =>
            await ComposeContainer().Resolve<Application>().Execute();

        static IContainer ComposeContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<DefaultModule>();

            return builder.Build();
        }
    }
}
