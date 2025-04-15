using Gitea.Net.Api;
using Gitea.Net.Client;
using Gitea.Net.Model;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Extensions.Configuration.GitRepository.GiteaProvider
{
    internal class GiteaRepositoryClient : IGitRepositoryClient
    {
        private readonly GitRepositoryConfigurationOptions _options;
        private RepositoryApi _client;
        private string owner;
        private string repoName;

        public GiteaRepositoryClient(GitRepositoryConfigurationOptions options)
        {
            _options = options;
        }

        private void check_config()
        {
            if (_client != null)
            {
                Gitea.Net.Client.Configuration config = new Gitea.Net.Client.Configuration();
                config.BasePath = $"{_options.HostUrl}/api/v1";
                if (_options.Proxy != null)
                {
                    config.Proxy = new WebProxy(_options.Proxy);
                }

                //// Configure API key authorization: TOTPHeader
                //config.ApiKey.Add("X-GITEA-OTP", "YOUR_API_KEY");
                //// Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
                //// config.ApiKeyPrefix.Add("X-GITEA-OTP", "Bearer");
                //// Configure API key authorization: AuthorizationHeaderToken
                //config.ApiKey.Add("Authorization", "YOUR_API_KEY");
                //// Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
                //// config.ApiKeyPrefix.Add("Authorization", "Bearer");
                //// Configure API key authorization: SudoHeader
                //config.ApiKey.Add("Sudo", "YOUR_API_KEY");
                //// Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
                //// config.ApiKeyPrefix.Add("Sudo", "Bearer");
                //// Configure HTTP basic authorization: BasicAuth
                //config.Username = "YOUR_USERNAME";
                //config.Password = "YOUR_PASSWORD";
                // Configure API key authorization: AccessToken
                config.ApiKey.Add("access_token", _options.AuthenticationToken);
                // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
                // config.ApiKeyPrefix.Add("access_token", "Bearer");
                // Configure API key authorization: SudoParam
                //config.ApiKey.Add("sudo", "YOUR_API_KEY");
                //// Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
                //// config.ApiKeyPrefix.Add("sudo", "Bearer");
                //// Configure API key authorization: Token
                //config.ApiKey.Add("token", "YOUR_API_KEY");
                // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
                // config.ApiKeyPrefix.Add("token", "Bearer");

                _client = new RepositoryApi(config);
                var rp = _options.RepositoryPath.Split('/');
                owner = rp[0];
                repoName = rp[1];
                try
                {
                    // Returns the Person actor for a user
                    //   ActivityPub result = apiInstance.ActivitypubPerson(userId);
                }
                catch (ApiException e)
                {
                    Debug.Print("Exception when calling ActivitypubApi.ActivitypubPerson: " + e.Message);
                    Debug.Print("Status Code: " + e.ErrorCode);
                    Debug.Print(e.StackTrace);
                }
            }
        }

        public bool FileExists(string _filePath)
        {
            check_config();
            //q=Gitea.json&search_mode=exact
            var result = _client.RepoGetContentsList(owner, repoName);
            var ok = result.Any(c => c.Name == _filePath);
            return ok;
        }

        public string GetFile(string _fileName)
        {
            check_config();
            var file = _client.RepoGetRawFileWithHttpInfo(owner, repoName, _fileName);
            return file.RawContent;
        }

        public void PutFile(string _fileName, string _content, string _msg)
        {
            check_config();
            var body = new CreateFileOptions(message: _msg, content: Convert.ToBase64String(Encoding.UTF8.GetBytes(_content)));
            var result = _client.RepoCreateFile(owner, repoName, _fileName, body);
        }
    }
}