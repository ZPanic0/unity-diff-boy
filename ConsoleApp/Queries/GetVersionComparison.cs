using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp.Queries
{
    public static class GetVersionComparison
    {
        public class Request : IRequest<Result>
        {
            public Request(string sourcePath, string targetPath)
            {
                SourcePath = sourcePath;
                TargetPath = targetPath;
            }

            public string SourcePath { get; }
            public string TargetPath { get; }
        }

        public class Result
        {
            public IEnumerable<string> Added { get; set; }
            public IEnumerable<string> Modified { get; set; }
            public IEnumerable<string> Removed { get; set; }
        }

        public class Handler : IRequestHandler<Request, Result>
        {
            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {

                var sourceDirectories = GetRelativeDirectories(request.SourcePath);
                var sourceFiles = GetRelativeFileDirectories(request.SourcePath);
                var targetDirectories = GetRelativeDirectories(request.TargetPath);
                var targetFiles = GetRelativeFileDirectories(request.TargetPath);

                var result = new Result
                {
                    Added = targetDirectories.Except(sourceDirectories).Concat(sourceFiles).ToArray(),
                    Modified = GetModifiedFiles(request.SourcePath, request.TargetPath, sourceFiles, targetFiles).ToArray(),
                    Removed = sourceDirectories.Except(targetDirectories).Concat(targetFiles).ToArray()
                };

                return result;
            }

            private string[] GetRelativeDirectories(string path)
            {
                return Directory
                    .GetDirectories(path, "*", SearchOption.AllDirectories)
                    .Select(directory => directory.Remove(0, path.Length))
                    .ToArray();
            }

            private string[] GetRelativeFileDirectories(string path)
            {
                return Directory
                    .GetFiles(path, "*", SearchOption.AllDirectories)
                    .Select(directory => directory.Remove(0, path.Length))
                    .ToArray();
            }

            private IEnumerable<string> GetModifiedFiles(
                string sourceBasePath,
                string targetBasePath,
                IEnumerable<string> sourceFileDirectories,
                IEnumerable<string> targetFileDirectories)
            {
                foreach (var fileDirectory in sourceFileDirectories.Intersect(targetFileDirectories))
                {
                    var sourceFile = File.ReadAllBytes(sourceBasePath + fileDirectory);
                    var targetFile = File.ReadAllBytes(targetBasePath + fileDirectory);

                    if (!sourceFile.SequenceEqual(targetFile))
                    {
                        yield return fileDirectory;
                    }
                }
            }
        }
    }
}
