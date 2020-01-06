using DeltaCompressionDotNet.MsDelta;

namespace ConsoleApp.Common
{
    public class MsDeltaCompressionProvider : IDeltaCompression
    {
        private readonly MsDeltaCompression delta;
        public MsDeltaCompressionProvider()
        {
            delta = new MsDeltaCompression();
        }
        public void ApplyDelta(string deltaFilePath, string oldFilePath, string newFilePath) =>
            delta.ApplyDelta(deltaFilePath, oldFilePath, newFilePath);

        public void CreateDelta(string deltaFilePath, string oldFilePath, string newFilePath) =>
            delta.CreateDelta(oldFilePath, newFilePath, deltaFilePath);
    }
}
