using Extensions.Configuration.GitRepository;
using Extensions.Configuration.GitRepository.GiteaProvider;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;

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
            var gitlabClient = new GiteaRepositoryClient(options);
            options.GitRepositoryClient = gitlabClient;
            return options;
        }
    }
}