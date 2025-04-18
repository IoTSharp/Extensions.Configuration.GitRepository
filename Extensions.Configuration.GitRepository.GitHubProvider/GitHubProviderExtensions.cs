﻿using Extensions.Configuration.GitRepository;
using Extensions.Configuration.GitRepository.GitHubProvider;
using System.Diagnostics.CodeAnalysis;

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
            if (string.IsNullOrEmpty(options.HostUrl)) options.HostUrl = "https://github.com/";
            var gitlabClient = new GitHubRepositoryClient(options);
            options.GitRepositoryClient = gitlabClient;
            return options;
        }

        public static GitRepositoryConfigurationOptions WithGitHubGist([NotNull] this GitRepositoryConfigurationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.HostUrl)) options.HostUrl = "https://github.com/";
            var gitlabClient = new GitHubGistClient(options);
            options.GitRepositoryClient = gitlabClient;
            return options;
        }
    }
}