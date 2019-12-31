using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp.Commands
{
    public static class ZipDeltaFiles
    {
        public class Request : IRequest
        {
            public Request(string target, string output)
            {
                Target = target;
                Output = output;
            }

            public string Target { get; }
            public string Output { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                ZipFile.CreateFromDirectory(request.Target, request.Output, CompressionLevel.Optimal, false);

                return Unit.Task;
            }
        }
    }
}
