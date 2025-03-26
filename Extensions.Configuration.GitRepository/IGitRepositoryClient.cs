using NGitLab.Models;

namespace Extensions.Configuration.GitRepository
{
    public interface IGitRepositoryClient
    {
        public string GetRepositoryFile(string repoPath, string fileName, string _ref);
    }
}