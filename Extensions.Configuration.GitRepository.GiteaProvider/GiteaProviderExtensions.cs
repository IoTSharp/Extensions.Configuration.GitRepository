using Extensions.Configuration.GitRepository;
using Extensions.Configuration.GitRepository.GiteaProvider;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Configuration
{
    public static class GiteaProviderExtensions
    {
        public static GitRepositoryConfigurationOptions WithGitea([NotNull] this GitRepositoryConfigurationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.HostUrl)) options.HostUrl = "https://gitea.com/";
            var gitlabClient = new GiteaRepositoryClient(options);
            options.GitRepositoryClient = gitlabClient;
            return options;
        }
    }
}