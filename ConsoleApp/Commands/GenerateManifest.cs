using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

            public IEnumerable<string> AddedDirectories { get; set; }
            public IEnumerable<string> AddedFiles { get; set; }
            public IEnumerable<string> ModifiedFiles { get; set; }
            public IEnumerable<string> RemovedDirectories { get; set; }
            public IEnumerable<string> RemovedFiles { get; set; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var outputDirectory = $@"{Directory.GetCurrentDirectory()}\output";
                Directory.CreateDirectory(outputDirectory);

                request.AddedDirectories = request.AddedDirectories.Select(item => $"AD {item}");
                request.AddedFiles = request.AddedFiles.Select(item => $"AF {item}");
                request.ModifiedFiles = request.ModifiedFiles.Select(item => $"MF {item}");
                request.RemovedDirectories = request.RemovedDirectories.Select(item => $"RD {item}");
                request.RemovedFiles = request.RemovedFiles.Select(item => $"RF {item}");

                using var manifest = new FileStream($@"{outputDirectory}\manifest.json", FileMode.Create);
                
                await JsonSerializer.SerializeAsync(manifest, request);

                return Unit.Value;
            }
        }
    }
}
