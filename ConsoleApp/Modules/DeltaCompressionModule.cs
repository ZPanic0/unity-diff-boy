using Autofac;
using ConsoleApp.Common;

namespace ConsoleApp.Modules
{
    public class DeltaCompressionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<MsDeltaCompressionProvider>()
                .As<IDeltaCompression>()
                .SingleInstance();
        }
    }
}
