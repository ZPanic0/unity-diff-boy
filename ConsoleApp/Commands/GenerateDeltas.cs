using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DeltaCompressionDotNet.MsDelta;
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
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var baseDirectory = Directory.GetCurrentDirectory();
                var delta = new MsDeltaCompression();

                foreach (var relativePath in request.ModifiedFiles)
                {
                    var sourcePath = $"{request.SourcePath}{relativePath}";
                    var targetPath = $"{request.TargetPath}{relativePath}";
                    var deltaFilePath = $@"{baseDirectory}\output{relativePath}";

                    Directory.CreateDirectory(Path.GetDirectoryName(deltaFilePath));
                    delta.CreateDelta(sourcePath, targetPath, deltaFilePath);
                }

                return Unit.Task;
            }
        }
    }
}
