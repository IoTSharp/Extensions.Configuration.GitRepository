using NGitLab;
using NGitLab.Models;

namespace Extensions.Configuration.GitRepository
{
    public class GitLabRepositoryClient : IGitRepositoryClient
    {
        private string hostUrl;
        private string authenticationToken;
        private readonly NGitLab.GitLabClient client;
        private readonly Project project;
        private readonly IRepositoryClient repo;

        public GitLabRepositoryClient()
        { }

        public GitLabRepositoryClient(string hostUrl, string authenticationToken, string repoNamespaced)
        {
            this.hostUrl = hostUrl;
            this.authenticationToken = authenticationToken;
            client = new NGitLab.GitLabClient(hostUrl, authenticationToken);
            project = client.Projects.GetByNamespacedPathAsync(repoNamespaced).GetAwaiter().GetResult();
            repo = client.GetRepository(new ProjectId(project.Id));
        }

        public bool FileExists(string filePath)
        {
            return repo.Files.FileExists(filePath, project.DefaultBranch);
        }

        public string GetFile(string fileName)
        {
            var context = repo.Files.Get(fileName, project.DefaultBranch).DecodedContent;
            return context;
        }

        public void PutFile(string fileName, string content, string msg)
        {
            var fileUpsert = new FileUpsert
            {
                Branch = project.DefaultBranch,
                CommitMessage = msg,
                RawContent = content,
                Encoding = "base64",
                Path = fileName
            };
            repo.Files.Create(fileUpsert);
        }
    }
}