using ConsoleApp.Queries;
using MediatR;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Application
    {
        private readonly IMediator mediator;

        public Application(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Execute()
        {
            var path = await mediator.Send(new GetEditorsPath.Request());

            var result = await mediator.Send(new GetVersionComparison.Request(
                @"C:\Program Files\Unity\Hub\Editor\2019.2.16f1", 
                @"C:\Program Files\Unity\Hub\Editor\2019.2.17f1"));
        }
    }
}
