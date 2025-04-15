using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Extensions.Configuration.GitRepository
{
    public class GitRepositoryConfigurationOptions
    {
        public GitRepositoryConfigurationOptions()
        {
        }

        public GitRepositoryConfigurationOptions(
            [NotNull] string hostUrl,
            [NotNull] string repositoryPath,
            [NotNull] string authenticationToken,
            [NotNull] string filename)
        {
            HostUrl = hostUrl ?? throw new ArgumentNullException(nameof(hostUrl));
            RepositoryPath = repositoryPath ?? throw new ArgumentNullException(nameof(repositoryPath));
            AuthenticationToken = authenticationToken ?? throw new ArgumentNullException(nameof(authenticationToken));
            FileName = filename ?? throw new ArgumentNullException(nameof(filename));
        }

        public TimeSpan ReloadInterval { get; set; } = TimeSpan.FromSeconds(60);
        public Uri Proxy { get; set; }
        public GitRepositoryConfigurationOptions WithProxy(string _uri=null)
        {
            Proxy = null;
            if (!string.IsNullOrEmpty(_uri))
            {
                if (Uri.TryCreate(_uri, UriKind.RelativeOrAbsolute,out Uri _result))
                {
                    Proxy = _result;
                }
            }
            return this;
        }

        public string HostUrl { get; set; }

        public string AuthenticationToken { get; set; }

        public string RepositoryPath { get; set; }

        public string FileName { get; set; }
        public string Ref { get; set; } = string.Empty;

        public Func<string, string> KeyNormalizer { get; set; } = NormalizeKey;

        public GitRepositoryConfigurationOptions WithReloadInterval(TimeSpan reloadInterval)
        {
            ReloadInterval = reloadInterval;
            return this;
        }

        public GitRepositoryConfigurationOptions WithRef([NotNull] string _ref)
        {
            Ref = _ref ?? throw new ArgumentNullException(nameof(_ref));
            return this;
        }

        public GitRepositoryConfigurationOptions WithHostUrl([NotNull] string hostUrl)
        {
            HostUrl = hostUrl ?? throw new ArgumentNullException(nameof(hostUrl));
            return this;
        }

        public GitRepositoryConfigurationOptions WithAuthenticationToken([NotNull] string authenticationToken)
        {
            AuthenticationToken = authenticationToken ?? throw new ArgumentNullException(nameof(authenticationToken));
            return this;
        }

        public string CacheToFile { get; set; }
        public IGitRepositoryClient GitRepositoryClient { get; set; }

        public GitRepositoryConfigurationOptions WithRepositoryPath([NotNull] string repositoryPath)
        {
            RepositoryPath = repositoryPath ?? throw new ArgumentNullException(nameof(repositoryPath));
            return this;
        }

        public GitRepositoryConfigurationOptions WithCache(string _cacheToFile)
        {
            CacheToFile = _cacheToFile;
            return this;
        }

        public GitRepositoryConfigurationOptions WithFileName([NotNull] string fileName)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            return this;
        }

        public GitRepositoryConfigurationOptions WithKeyNormalizer([NotNull] Func<string, string> keyNormalizer)
        {
            KeyNormalizer = keyNormalizer ?? throw new ArgumentNullException(nameof(keyNormalizer));
            return this;
        }

        private static string NormalizeKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            var segments = Array.ConvertAll(key.Split('_'), e =>
            {
                e = e.ToLower();
                return e.Length <= 1 ? e : char.ToUpper(e[0]) + e.Substring(1);
            });

            return string.Join(ConfigurationPath.KeyDelimiter, segments);
        }
    }
}