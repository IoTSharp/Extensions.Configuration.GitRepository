
using Octokit;
using Octokit.Internal;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Extensions.Configuration.GitRepository.GitLabProvider
{
    internal class GitHubRepositoryClient : IGitRepositoryClient
    {

        private GitHubClient client = null;
        private string owner;
        private string repoName;
        private readonly GitRepositoryConfigurationOptions _options;


        public GitHubRepositoryClient(GitRepositoryConfigurationOptions options)
        {
            _options = options;
        }

        private void  check_connect()
        {
            if (client == null)
            {
         
                var connection = new Connection(new ProductHeaderValue("NConfiguration"),
                    new HttpClientAdapter(() =>
                    {
                        if (_options.Proxy != null)
                        {
                            return HttpMessageHandlerFactory.CreateDefault( new WebProxy(_options.Proxy));
                        }
                        else
                        {
                            return HttpMessageHandlerFactory.CreateDefault();
                        }
                    }));
                connection .Credentials= new  Credentials(_options.AuthenticationToken); // This can be a PAT or an OAuth token.
                client = new GitHubClient(connection);
                var  rp = _options.RepositoryPath.Split('/');
                owner = rp[0]; 
                repoName = rp[1];
            }
        }



        public bool FileExists(string _filePath)
        {
            check_connect();
            bool result = false;
            // 搜索代码
            var searchRequest = new SearchCodeRequest();
            searchRequest.Repos.Add(owner, repoName);
            searchRequest.FileName = _filePath;
            var searchResult = client.Search.SearchCode(searchRequest).GetAwaiter().GetResult();
            return searchResult.TotalCount == 1;
        }

        public string GetFile(string _fileName)
        {
            var rs = client.Repository.Content.GetAllContents(owner, repoName, _fileName).GetAwaiter().GetResult();
            return  rs?.FirstOrDefault()?.Content;
        }

        public void PutFile(string _fileName, string _content, string _msg)
        {
            client.Repository.Content.CreateFile(owner, repoName, _fileName, new CreateFileRequest("upload file", _content)).GetAwaiter().GetResult();
        }
    }
}