namespace ConsoleApp.Common
{
    public interface IDeltaCompression
    {
        public void CreateDelta(string deltaFilePath, string oldFilePath, string newFilePath);
        public void ApplyDelta(string deltaFilePath, string oldFilePath, string newFilePath);
    }
}
