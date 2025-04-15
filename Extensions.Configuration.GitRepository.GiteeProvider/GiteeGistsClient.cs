using Extensions.Configuration.GitRepository.GiteeProvider;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Extensions.Configuration.GitRepository.GiteeProvider
{
 
    public record gist_file(int size, string raw_url, string type, bool truncated, string content);

    public record gist_file_put( string content);


    public record gist(string url, string forks_url, string commits_url, string id, string description, Dictionary<string, gist_file> files, bool truncated, string html_url, int comments, string comments_url, string git_pull_url, string git_push_url, string created_at, string updated_at, string forks, string history);

    internal class GiteeGistsClient : IGitRepositoryClient
    {
        private readonly GitRepositoryConfigurationOptions _options;
        private HttpClient _client;
        private string _id;
        private string owner;
        private string repoName;

        public GiteeGistsClient(GitRepositoryConfigurationOptions options)
        {
            _options = options;
        }

        private void check_config()
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _id = _options.RepositoryPath;
                _client.BaseAddress = new Uri($"https://gitee.com/api/v5/gists/{_id}");
            }
        }

        public bool FileExists(string _filePath)
        {
            bool result = false;
            check_config();
            var reps = _client.GetAsync($"?access_token={_options.AuthenticationToken}").GetAwaiter().GetResult();
            if (reps.IsSuccessStatusCode)
            {
                var text = reps.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var obj= JsonSerializer.Deserialize<gist>(text);
                result = obj.files.Any(s => s.Key == _filePath);
            }
            return result;
        }

        public string GetFile(string _fileName)
        {
            string result = string.Empty;
            check_config();
            var reps = _client.GetAsync($"?access_token={_options.AuthenticationToken}").GetAwaiter().GetResult();
          if (reps.IsSuccessStatusCode)
            {
                var text = reps.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var obj = JsonSerializer.Deserialize<gist>(text);
                var gf = obj.files.FirstOrDefault(s => s.Key == _fileName);
                result = gf.Value?.content;
            }
            return  result;
        }

        public void PutFile(string _fileName, string _content, string _msg)
        {
            check_config();
            //curl -X POST --header 'Content-Type: ' 'https://gitee.com/api/v5/repos/maikebing/gitcfg/contents/Gitee.json' -d '{"access_token":"d675106450be61985dd39ec076cc05c0","content":"sdfsd","message":"sdfsd"}'
            Dictionary<string, gist_file_put> content = new Dictionary<string, gist_file_put>();
            content.Add(_fileName, new gist_file_put( _content));
            var body = JsonContent.Create(new { access_token = _options.AuthenticationToken, id=_id, files =  content  , description = _msg });

            var reps = _client.PatchAsync($"", body).GetAwaiter().GetResult();
            var result = reps.IsSuccessStatusCode;
        }
    }
}