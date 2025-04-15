using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Extensions.Configuration.GitRepository.GitLabProvider
{
    public record GitContent(string type, string encoding, int size, string name, string path, string content, string sha, string url, string html_url, string download_url);

    internal class GiteeRepositoryClient : IGitRepositoryClient
    {
        private readonly GitRepositoryConfigurationOptions _options;
        private HttpClient _client;
        private string owner;
        private string repoName;

        public GiteeRepositoryClient(GitRepositoryConfigurationOptions options)
        {
            _options = options;
        }

        private void check_config()
        {
            if (_client == null)
            {
                _client = new HttpClient();
                var rp = _options.RepositoryPath.Split('/');
                owner = rp[0];
                repoName = rp[1];
                _client.BaseAddress = new Uri($"https://gitee.com/api/v5/repos/{owner}/{repoName}/contents/");
            }
        }

        public bool FileExists(string _filePath)
        {
            check_config();
            var reps = _client.GetAsync($"/{_filePath}?access_token={_options.AuthenticationToken}").GetAwaiter().GetResult();
            if (reps.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var rep = JsonSerializer.Deserialize<List<GitContent>>(reps.Content.ReadAsStream());
                return rep?.Any(gc => gc.path == _filePath) == true;
            }
            else
            {
                return false;
            }
        }

        public string GetFile(string _fileName)
        {
            check_config();
            //https://gitee.com/api/v5/repos/{owner}/{repo}/raw/{path}
            var reps = _client.GetStringAsync($"raw/{_fileName}?access_token={_options.AuthenticationToken}").GetAwaiter().GetResult();
            return reps;
        }

        public void PutFile(string _fileName, string _content, string _msg)
        {
            check_config();
            //curl -X POST --header 'Content-Type: ' 'https://gitee.com/api/v5/repos/maikebing/gitcfg/contents/Gitee.json' -d '{"access_token":"d675106450be61985dd39ec076cc05c0","content":"sdfsd","message":"sdfsd"}'
            var body = JsonContent.Create(new { access_token = _options.AuthenticationToken, content = Convert.ToBase64String(Encoding.UTF8.GetBytes(_content)), message = _msg });
            var reps = _client.PostAsync($"{_fileName}", body).GetAwaiter().GetResult();
        }
    }
}