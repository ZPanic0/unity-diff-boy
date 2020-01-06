using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Common;
using MediatR;

namespace ConsoleApp.Commands
{
    public static class GenerateDeltas
    {
        public class Request : IRequest
        {
            public Request(string sourcePath, string targetPath, IEnumerable<string> modifiedFiles)
            {
                SourcePath = sourcePath;
                TargetPath = targetPath;
                ModifiedFiles = modifiedFiles;
            }

            public string SourcePath { get; }
            public string TargetPath { get; }
            public IEnumerable<string> ModifiedFiles { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IDeltaCompression delta;

            public Handler(IDeltaCompression delta)
            {
                this.delta = delta;
            }

            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var baseDirectory = Directory.GetCurrentDirectory();

                foreach (var relativePath in request.ModifiedFiles)
                {
                    var sourcePath = $"{request.SourcePath}{relativePath}";
                    var targetPath = $"{request.TargetPath}{relativePath}";
                    var deltaFilePath = $@"{baseDirectory}\output{relativePath}";

                    Directory.CreateDirectory(Path.GetDirectoryName(deltaFilePath));
                    delta.CreateDelta(deltaFilePath, sourcePath, targetPath);
                }

                return Unit.Task;
            }
        }
    }
}
