using Extensions.Configuration.GitRepository;
using Hyperbee.Json.Extensions;
using Hyperbee.Json.Patch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NGitLab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
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
            LoadCache();
            LoadAsync();
        }

        private void LoadCache()
        {
            if (File.Exists(_options.CacheToFile))
            {
                _jsonData = JsonDocument.Parse(File.ReadAllText(_options.CacheToFile));
                Data = JsonConfigurationFileParser.Parse(_jsonData);

            }
         
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
       private JsonDocument  _jsonData { get; set; }
        private  void LoadAsync()
        {
            try
            {
                var newData = GetNewDataAsync();
                var _diff = JsonDiff<JsonNode>.Diff(_jsonData, newData).ToArray();
                if (_diff.Any())
                {
                    _jsonData = newData;
                    var jp = new JsonPatch(_diff.ToArray());
                    var _jn = jp.Apply(_jsonData.RootElement);
                    System.IO.File.WriteAllText(_options.CacheToFile, _jn.ToJsonString(new JsonSerializerOptions() { WriteIndented=true }));
                    LoadCache();
                    OnReload();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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

     

        private JsonDocument GetNewDataAsync()
        {
            var fileContent = _gitlabClient.GetRepositoryFile(_options.RepositoryPath,_options.FileName,_options.Ref);
            return JsonDocument.Parse(fileContent);
        }
    }

}