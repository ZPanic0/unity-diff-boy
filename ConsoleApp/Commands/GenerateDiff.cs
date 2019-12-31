using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp.Commands
{
    public static class GenerateDiff
    {
        public class Request : IRequest
        {
            public Request(
                string sourcePath, 
                string targetPath, 
                IEnumerable<string> addedFolders,
                IEnumerable<string> addedFiles,
                IEnumerable<string> modifiedFiles, 
                IEnumerable<string> removedFolders,
                IEnumerable<string> removedFiles)
            {
                SourcePath = sourcePath;
                TargetPath = targetPath;
                AddedFolders = addedFolders;
                AddedFiles = addedFiles;
                ModifiedFiles = modifiedFiles;
                RemovedFolders = removedFolders;
                RemovedFiles = removedFiles;
            }

            public string SourcePath { get; }
            public string TargetPath { get; }
            public IEnumerable<string> AddedFolders { get; }
            public IEnumerable<string> AddedFiles { get; }
            public IEnumerable<string> ModifiedFiles { get; }
            public IEnumerable<string> RemovedFolders { get; }
            public IEnumerable<string> RemovedFiles { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                await mediator.Send(new GenerateManifest.Request(
                    request.AddedFolders,
                    request.AddedFiles,
                    request.ModifiedFiles,
                    request.RemovedFolders,
                    request.RemovedFiles));

                await mediator.Send(new CopyAddedFiles.Request(request.TargetPath, request.AddedFiles));

                await mediator.Send(new GenerateDeltas.Request(request.SourcePath, request.TargetPath, request.ModifiedFiles));

                var targetDir = $@"{Directory.GetCurrentDirectory()}\output";
                var sourceVersion = new DirectoryInfo(request.SourcePath).Name;
                var targetVersion = new DirectoryInfo(request.TargetPath).Name;
                var outputDir = $@"{Directory.GetCurrentDirectory()}\{sourceVersion} to {targetVersion}.zip";

                await mediator.Send(new ZipDeltaFiles.Request(targetDir, outputDir));

                return Unit.Value;
            }
        }
    }
}
