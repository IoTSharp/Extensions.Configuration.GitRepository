using Extensions.Configuration.GitRepository;
using NGitLab;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Configuration
{
    public static class GitRepositoryConfigurationExtensions
    {
        public static IConfigurationBuilder AddGitRepository(
            [NotNull] this IConfigurationBuilder builder,
            [NotNull] string hostUrl,
            [NotNull] string repositoryPath,
            [NotNull] string authenticationToken,
            [NotNull] string environmentName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (hostUrl == null)
            {
                throw new ArgumentNullException(nameof(hostUrl));
            }

            if (repositoryPath == null)
            {
                throw new ArgumentNullException(nameof(repositoryPath));
            }

            if (authenticationToken == null)
            {
                throw new ArgumentNullException(nameof(authenticationToken));
            }

            if (environmentName == null)
            {
                throw new ArgumentNullException(nameof(environmentName));
            }
            
            var options = new GitRepositoryConfigurationOptions(hostUrl, repositoryPath, authenticationToken, environmentName);
            return builder.AddGitRepository(options);
        }

        public static IConfigurationBuilder AddGitRepository(
            [NotNull] this IConfigurationBuilder builder,
            [NotNull] Action<GitRepositoryConfigurationOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new GitRepositoryConfigurationOptions();
            configure(options);
            return builder.AddGitRepository(options);
        }

        public static IConfigurationBuilder AddGitRepository(
            [NotNull] this IConfigurationBuilder builder,
            [NotNull] GitRepositoryConfigurationOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var gitlabClient = new   GitLabRepositoryClient(options.HostUrl, options.AuthenticationToken);
            var source = new GitRepositoryConfigurationSource(gitlabClient, options);
            return builder.Add(source);
        }
    }
}
