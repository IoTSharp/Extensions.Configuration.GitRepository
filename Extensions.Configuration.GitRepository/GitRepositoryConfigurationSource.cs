﻿using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Extensions.Configuration.GitRepository
{
    public class GitRepositoryConfigurationSource : IConfigurationSource
    {
        private readonly IGitRepositoryClient _gitRepo;
        private readonly GitRepositoryConfigurationOptions _options;

        public GitRepositoryConfigurationSource(
        
            [NotNull] GitRepositoryConfigurationOptions options)
        {
            _gitRepo = options?.GitRepositoryClient ?? throw new ArgumentNullException("GitRepositoryClient");
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new GitRepositoryConfigurationProvider(_gitRepo, _options);
        }
    }
}