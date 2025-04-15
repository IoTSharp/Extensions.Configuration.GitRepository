using Extensions.Configuration.GitRepository;
using Extensions.Configuration.GitRepository.GitLabProvider;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Configuration
{
    public static class GitLabProviderExtensions
    {
        public static GitRepositoryConfigurationOptions WithGitLab([NotNull] this GitRepositoryConfigurationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.HostUrl)) options.HostUrl = "https://gitlab.com/";
            options.GitRepositoryClient = new GitLabRepositoryClient(options);
            return options;
        }
    }
}