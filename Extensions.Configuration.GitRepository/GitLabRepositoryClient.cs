using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.Configuration.GitRepository
{
    public class GitLabRepositoryClient: IGitRepositoryClient
    {
        private string hostUrl;
        private string authenticationToken;
        private readonly NGitLab.GitLabClient client;

        public GitLabRepositoryClient() { }

        public GitLabRepositoryClient(string hostUrl, string authenticationToken)
        {
            this.hostUrl = hostUrl;
            this.authenticationToken = authenticationToken;
           client = new NGitLab.GitLabClient(hostUrl, authenticationToken);
        }
        string __ref = "";
        public string GetRepositoryFile(string repoPath, string fileName, string _ref)
        {
            var context = string.Empty;
            var project = client.Projects.GetByNamespacedPathAsync(repoPath).GetAwaiter().GetResult();
            var repo = client.GetRepository(new ProjectId(project.Id));
          
            if (!string.IsNullOrEmpty(_ref))
            {
                __ref = _ref;
            }
            else
            {
                __ref=project.DefaultBranch;
            }
            if (repo.Files.FileExists(fileName, __ref))
            {
                context = repo.Files.Get(fileName, __ref).DecodedContent;
            }
            return context;
        }
    }
}
