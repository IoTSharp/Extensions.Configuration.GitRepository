using Octokit;
using Octokit.Internal;
using System.Net;

namespace Extensions.Configuration.GitRepository.GitHubProvider
{
    internal class GitHubGistClient : IGitRepositoryClient
    {
        private GitHubClient client = null;
        private string _gistId;
        private readonly GitRepositoryConfigurationOptions _options;

        public GitHubGistClient(GitRepositoryConfigurationOptions options)
        {
            _options = options;
        }

        private void check_connect()
        {
            if (client == null)
            {
                
                var connection = new Connection(new ProductHeaderValue("NConfiguration"),
                    new HttpClientAdapter(() =>
                    {
                        if (_options.Proxy != null)
                        {
                            return HttpMessageHandlerFactory.CreateDefault(new WebProxy(_options.Proxy));
                        }
                        else
                        {
                            return HttpMessageHandlerFactory.CreateDefault();
                        }
                    }));
                connection.Credentials = new Credentials(_options.AuthenticationToken); // This can be a PAT or an OAuth token.
                client = new GitHubClient(connection);
                _gistId = _options.RepositoryPath;
            }
        }

        public bool FileExists(string _filePath)
        {
            check_connect();
            bool result = false;    
             var g= client.Gist.Get(_gistId).GetAwaiter().GetResult();
            if (g.Files.Any(f=>f.Key==_filePath))
            {
                result = true;
            }
            return result;
        }

        public string GetFile(string _fileName)
        {
            check_connect();
            var g = client.Gist.Get(_gistId).GetAwaiter().GetResult();
            var txt = g.Files.FirstOrDefault(f => f.Key == _fileName).Value.Content;
            return txt;
        }

        public void PutFile(string _fileName, string _content, string _msg)
        {
            check_connect();
            var g = new GistUpdate();
            g.Files.Add(_fileName, new GistFileUpdate() { Content = _content, NewFileName = _fileName });
            g.Description = _msg;
            var eg= client.Gist.Edit(_gistId, g).GetAwaiter().GetResult();
        }
    }
}