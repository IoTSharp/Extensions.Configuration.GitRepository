using Extensions.Configuration.GitRepository;
using Extensions.Configuration.GitRepository.GitLabProvider;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.Configuration
{
    public static class GitHubProviderExtensions
    {
        public static GitRepositoryConfigurationOptions WithGitHub([NotNull] this GitRepositoryConfigurationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var gitlabClient = new GitHubRepositoryClient(options);
            options.GitRepositoryClient = gitlabClient;
            return options;
        }
    }
}