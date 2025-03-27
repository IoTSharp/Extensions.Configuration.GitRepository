namespace Extensions.Configuration.GitRepository
{
    public interface IGitRepositoryClient
    {
        bool FileExists(string _filePath);

        public string GetFile(string _fileName);

        void PutFile(string _fileName, string _content, string _msg);
    }
}