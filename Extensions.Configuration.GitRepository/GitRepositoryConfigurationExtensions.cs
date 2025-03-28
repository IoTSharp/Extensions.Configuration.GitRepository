using Extensions.Configuration.GitRepository;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.Configuration
{
    public static class GitRepositoryConfigurationExtensions
    {
        public static IConfigurationBuilder AddGitRepository(
            [NotNull] this IConfigurationBuilder builder,
            [NotNull] string hostUrl,
            [NotNull] string repositoryPath,
            [NotNull] string authenticationToken,
            [NotNull] string fileName)
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

            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var options = new GitRepositoryConfigurationOptions(hostUrl, repositoryPath, authenticationToken, fileName);
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

            var gitlabClient = new GitLabRepositoryClient(options.HostUrl, options.AuthenticationToken, options.RepositoryPath);
            var source = new GitRepositoryConfigurationSource(gitlabClient, options);
            return builder.Add(source);
        }

        internal static string ToJsonString(this JsonDocument jdoc, JsonWriterOptions options = default)
        {
            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, options);
                jdoc.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}