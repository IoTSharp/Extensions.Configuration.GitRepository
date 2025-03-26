using Microsoft.Extensions.Configuration;
using NGitLab;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Extensions.Configuration.GitRepository
{
    public class GitRepositoryConfigurationSource : IConfigurationSource
    {
        private readonly IGitRepositoryClient _gitLabClient;
        private readonly GitRepositoryConfigurationOptions _options;

        public GitRepositoryConfigurationSource(
            [NotNull] IGitRepositoryClient gitLabClient,
            [NotNull] GitRepositoryConfigurationOptions options)
        {
            _gitLabClient = gitLabClient ?? throw new ArgumentNullException(nameof(gitLabClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GitRepositoryConfigurationProvider(_gitLabClient, _options);
        }
    }
}
