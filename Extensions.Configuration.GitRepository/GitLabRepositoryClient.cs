using NGitLab;
using NGitLab.Models;
using System;

namespace Extensions.Configuration.GitRepository
{
    public class GitLabRepositoryClient : IGitRepositoryClient
    {

        private readonly NGitLab.GitLabClient client;
        private readonly string _repoNamespaced;
        private Project project;
        private IRepositoryClient repo;

        public GitLabRepositoryClient(string hostUrl, string authenticationToken, string repoNamespaced)
        {
            client = new NGitLab.GitLabClient(hostUrl, authenticationToken);
            _repoNamespaced = repoNamespaced;
        }

        private void check_connect()
        {
            if (project == null || repo == null)
            {
                try
                {
                    project = client.Projects.GetByNamespacedPathAsync(_repoNamespaced).GetAwaiter().GetResult();
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