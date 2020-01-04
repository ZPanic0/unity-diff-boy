using System.Collections.Generic;

namespace ConsoleApp.Common
{
    public class Manifest
    {
        public IEnumerable<string> AddedDirectories { get; set; }
        public IEnumerable<string> AddedFiles { get; set; }
        public IEnumerable<string> ModifiedFiles { get; set; }
        public IEnumerable<string> RemovedDirectories { get; set; }
        public IEnumerable<string> RemovedFiles { get; set; }
    }
}
