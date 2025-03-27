using Hyperbee.Json.Patch;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        private void LoadAsync()
        {
            try
            {
                JsonDocument _jsonData = null;
                if (File.Exists(_options.CacheToFile))
                {
                    _jsonData = JsonDocument.Parse(File.ReadAllText(_options.CacheToFile));
                }
                if (_gitlabClient.FileExists(_options.FileName))
                {
                    var fileContent = _gitlabClient.GetFile(_options.FileName);
                    if (_jsonData == null)//如果远程文件存在，本地文件不存在， 就直接使用远程文件
                    {
                        _jsonData = JsonDocument.Parse(fileContent);
                        if (_jsonData != null)
                        {
                            System.IO.File.WriteAllText(_options.CacheToFile, fileContent);
                        }
                        Data = JsonConfigurationFileParser.Parse(_jsonData);
                        OnReload();
                    }
                    else
                    {//如果远程文件存在，本地文件存在， 就比较两个文件的差异，如果有差异， 就合并差异，然后缓存到本地，然后重新加载
                        var _diff = JsonDiff<JsonNode>.Diff(_jsonData, JsonDocument.Parse(fileContent)).ToArray();
                        if (_diff.Any())
                        {
                            var jp = new JsonPatch(_diff.ToArray());
                            var _jn = jp.Apply(_jsonData.RootElement);
                            string _json = _jn.ToJsonString(new JsonSerializerOptions() { WriteIndented = true });
                            System.IO.File.WriteAllText(_options.CacheToFile, _json);
                            _jsonData = JsonDocument.Parse(_json);
                            Data = JsonConfigurationFileParser.Parse(_jsonData);
                            OnReload();
                        }
                    }
                }
                else
                {
                    if (_jsonData != null)//如果远程文件不存在，本地文件存在， 就上传本地文件
                    {
                        _gitlabClient.PutFile(_options.FileName, _jsonData.ToJsonString(new JsonWriterOptions { Indented = true }), "local file upload to git repo");
                        Data = JsonConfigurationFileParser.Parse(_jsonData);
                        OnReload();
                    }
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
    }
}