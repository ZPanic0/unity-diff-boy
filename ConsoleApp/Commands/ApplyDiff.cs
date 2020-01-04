using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Common;
using DeltaCompressionDotNet.MsDelta;
using MediatR;

namespace ConsoleApp.Commands
{
    public static class ApplyDiff
    {
        public class Request : IRequest
        {
            public Request(string targetPath, string zipPath)
            {
                TargetPath = targetPath;
                ZipPath = zipPath;
            }

            public string TargetPath { get; }
            public string ZipPath { get; }
        }

        public class Handler : IRequestHandler<Request>
        {
            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                if (!File.Exists(request.ZipPath)) throw new ArgumentException($"Could not find zip path at {request.ZipPath}");
                if (!Directory.Exists(request.TargetPath)) throw new ArgumentException($"Could not find path at {request.TargetPath}");

                var diffPath = $@"{Directory.GetCurrentDirectory()}\diff";
                
                ExtractDiff(request.ZipPath, diffPath);

                var manifest = JsonSerializer.Deserialize<Manifest>(await File.ReadAllTextAsync($@"{diffPath}\manifest.json"));

                DeleteFiles(request.TargetPath, manifest.RemovedFiles);
                DeleteDirectories(request.TargetPath, manifest.RemovedDirectories);
                AddDirectories(request.TargetPath, manifest.AddedDirectories);
                AddFiles(diffPath, request.TargetPath, manifest.AddedFiles);
                ModifyFiles(diffPath, request.TargetPath, manifest.ModifiedFiles);

                Directory.Delete(diffPath, true);

                return Unit.Value;
            }

            private void ExtractDiff(string zipPath, string diffPath)
            {
                if (Directory.Exists(diffPath)) Directory.Delete(diffPath, true);

                using var fileStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read);
                using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

                archive.ExtractToDirectory(diffPath);
            }

            private void DeleteFiles(string targetPath, IEnumerable<string> deletedFilePaths)
            {
                foreach (var partialFilePath in deletedFilePaths)
                {
                    File.Delete(targetPath + partialFilePath);
                }
            }

            private void DeleteDirectories(string targetPath, IEnumerable<string> deletedDirectoryPaths)
            {
                foreach (var partialDirectoryPath in deletedDirectoryPaths)
                {
                    Directory.Delete(targetPath + partialDirectoryPath);
                }
            }

            private void AddDirectories(string targetPath, IEnumerable<string> addDirectoryPaths)
            {
                foreach (var partialDirectoryPath in addDirectoryPaths)
                {
                    Directory.CreateDirectory(targetPath + partialDirectoryPath);
                }
            }

            private void AddFiles(string diffPath, string targetPath, IEnumerable<string> addedFilePaths)
            {
                foreach (var partialFilePath in addedFilePaths)
                {
                    File.Move(diffPath + partialFilePath, targetPath + partialFilePath);
                }
            }

            private void ModifyFiles(string diffPath, string targetPath, IEnumerable<string> modifiedFilePaths)
            {
                var delta = new MsDeltaCompression();

                foreach (var partialFilePath in modifiedFilePaths)
                {
                    var oldPath = targetPath + partialFilePath;
                    var deltaPath = diffPath + partialFilePath;
                    var newPath = oldPath + "1";

                    delta.ApplyDelta(deltaPath, oldPath, newPath);
                    File.Delete(oldPath);
                    File.Move(newPath, oldPath);
                }
            }
        }
    }
}
