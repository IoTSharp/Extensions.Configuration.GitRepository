using NGitLab;
using NGitLab.Models;

namespace Extensions.Configuration.GitRepository.GitLabProvider
{
    internal class GitLabRepositoryClient : IGitRepositoryClient
    {
        private NGitLab.GitLabClient client;
        private readonly GitRepositoryConfigurationOptions _options;
        private Project project;
        private IRepositoryClient repo;

        public GitLabRepositoryClient(GitRepositoryConfigurationOptions options)
        {
            _options = options;
        }

        private void check_connect()
        {
            if (client == null)
            {
                client = new NGitLab.GitLabClient(_options.HostUrl, _options.AuthenticationToken);
            }
            if (project == null || repo == null)
            {
                try
                {
                    project = client.Projects.GetByNamespacedPathAsync(_options.RepositoryPath).GetAwaiter().GetResult();
                    repo = client.GetRepository(new ProjectId(project.Id));
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"GitLabRepositoryClient:{ex.ToString()}");
                    throw;
                }
            }
        }

        public bool FileExists(string filePath)
        {
            check_connect();
            return repo.Files.FileExists(filePath, project.DefaultBranch);
        }

        public string GetFile(string fileName)
        {
            check_connect();
            var context = repo.Files.Get(fileName, project.DefaultBranch).DecodedContent;
            return context;
        }

        public void PutFile(string fileName, string content, string msg)
        {
            check_connect();
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