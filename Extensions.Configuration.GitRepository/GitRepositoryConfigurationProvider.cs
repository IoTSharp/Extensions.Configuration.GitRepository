using Extensions.Configuration.GitRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NGitLab;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;


namespace Extensions.Configuration.GitRepository
{
    public class GitRepositoryConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IGitRepositoryClient _gitlabClient;
        private readonly GitRepositoryConfigurationOptions _options;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _changeTrackingStarted;

        public GitRepositoryConfigurationProvider(
            [NotNull] IGitRepositoryClient gitlabClient,
            [NotNull] GitRepositoryConfigurationOptions options)
        {
            _gitlabClient = gitlabClient ?? throw new ArgumentNullException(nameof(gitlabClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void Load()
        {
             LoadAsync();
        }

        private async Task LoadJsonFileAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await WaitForReaload();

                try
                {
                     LoadAsync();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private async Task WaitForReaload()
        {
            await Task.Delay(_options.ReloadInterval, _cancellationTokenSource.Token).ConfigureAwait(false);
        }
       private JsonDocument _jsonData { get; set; }
        private  void LoadAsync()
        {
            var newData = GetNewDataAsync();

            if (Changed(newData))
            {
                _jsonData =  newData;
                Data = JsonConfigurationFileParser.Parse(_jsonData);
                OnReload();
            }

            if (!_changeTrackingStarted)
            {
                _changeTrackingStarted = true;
                _ = Task.Run(async () => await LoadJsonFileAsync());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }

        private bool Changed(JsonDocument newData)
        {
            return !_jsonData.DeepEquals(newData);
        }

        private JsonDocument GetNewDataAsync()
        {
            var fileContent = _gitlabClient.GetRepositoryFile(_options.RepositoryPath,_options.FileName,_options.Ref);
            return JsonDocument.Parse(fileContent);
        }
    }

}