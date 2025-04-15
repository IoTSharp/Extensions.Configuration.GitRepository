﻿using Extensions.Configuration.GitRepository;
using Extensions.Configuration.GitRepository.GitLabProvider;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Configuration
{
    public static class GiteeProviderExtensions
    {
        public static GitRepositoryConfigurationOptions WithGitee([NotNull] this GitRepositoryConfigurationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.HostUrl)) options.HostUrl = "https://gitee.com/";
            var gitlabClient = new GiteeRepositoryClient(options);
            options.GitRepositoryClient = gitlabClient;
            return options;
        }
    }
}