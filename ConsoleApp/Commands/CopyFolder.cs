using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp.Commands
{
    public static class CopyFolder
    {
        public class Request : IRequest
        {
            public Request(string sourcePath, string targetPath)
            {
                SourcePath = sourcePath;
                TargetPath = targetPath;
            }

            public string SourcePath { get; }
            public string TargetPath { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                Directory.CreateDirectory(request.TargetPath);

                CopyDirectories(request.SourcePath, request.TargetPath);
                CopyFiles(request.SourcePath, request.TargetPath);

                return Unit.Task;
            }

            private void CopyDirectories(string sourcePath, string targetPath)
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }
            }

            private void CopyFiles(string sourcePath, string targetPath)
            {
                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                }
            }
        }
    }
}
