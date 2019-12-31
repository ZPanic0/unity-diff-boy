using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp.Commands
{
    public static class CopyAddedFiles
    {
        public class Request : IRequest
        {
            public Request(string sourcePath, IEnumerable<string> added)
            {
                SourcePath = sourcePath;
                Added = added;
            }

            public string SourcePath { get; }
            public IEnumerable<string> Added { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                foreach (var relativePath in request.Added)
                {
                    var destinationPath = $@"{Directory.GetCurrentDirectory()}\output{relativePath}";
                    var sourcePath = $"{request.SourcePath}{relativePath}";

                    var directory = Path.GetDirectoryName(destinationPath);

                    Directory.CreateDirectory(directory);

                    await CopyAsync(sourcePath, destinationPath);
                }

                return Unit.Value;
            }

            private async Task CopyAsync(string sourcePath, string destinationPath)
            {
                var fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
                var buffer = 65536;

                using var sourceStream = new FileStream(
                    sourcePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    buffer,
                    fileOptions);

                using var destinationStream = new FileStream(
                    destinationPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    buffer,
                    fileOptions);

                await sourceStream.CopyToAsync(destinationStream);
            }
        }
    }
}
