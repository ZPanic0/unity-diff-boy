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
        }
    }
}
