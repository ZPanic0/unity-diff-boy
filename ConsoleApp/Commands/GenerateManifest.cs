using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Common;
using MediatR;

namespace ConsoleApp.Commands
{
    public static class GenerateManifest
    {
        public class Request : IRequest
        {
            public Request(
                IEnumerable<string> addedDirectories,
                IEnumerable<string> addedFiles,
                IEnumerable<string> modifiedFiles,
                IEnumerable<string> removedDirectories,
                IEnumerable<string> removedFiles)
            {
                AddedDirectories = addedDirectories;
                AddedFiles = addedFiles;
                ModifiedFiles = modifiedFiles;
                RemovedDirectories = removedDirectories;
                RemovedFiles = removedFiles;
            }

            public IEnumerable<string> AddedDirectories { get; }
            public IEnumerable<string> AddedFiles { get; }
            public IEnumerable<string> ModifiedFiles { get; }
            public IEnumerable<string> RemovedDirectories { get; }
            public IEnumerable<string> RemovedFiles { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var outputDirectory = $@"{Directory.GetCurrentDirectory()}\output";
                Directory.CreateDirectory(outputDirectory);

                using var manifestStream = new FileStream($@"{outputDirectory}\manifest.json", FileMode.Create);

                await JsonSerializer.SerializeAsync(manifestStream, new Manifest
                {
                    AddedDirectories = request.AddedDirectories,
                    AddedFiles = request.AddedFiles,
                    ModifiedFiles = request.ModifiedFiles,
                    RemovedDirectories = request.RemovedDirectories,
                    RemovedFiles = request.RemovedFiles
                });

                return Unit.Value;
            }
        }
    }
}
