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
        private readonly IGitRepositoryClient __gitRepo;
        private readonly GitRepositoryConfigurationOptions _options;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _changeTrackingStarted;

        public GitRepositoryConfigurationProvider(
            [NotNull] IGitRepositoryClient gitlabClient,
            [NotNull] GitRepositoryConfigurationOptions options)
        {
            __gitRepo = gitlabClient ?? throw new ArgumentNullException(nameof(gitlabClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void Load()
        {
            SyncGitRepoFile();
        }

        private async Task LoadJsonFileAsync()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await WaitForReaload();

                try
                {
                    SyncGitRepoFile();
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

        private void SyncGitRepoFile()
        {
            try
            {
                JsonDocument _jsonData = null;
                if (File.Exists(_options.CacheToFile))
                {
                    _jsonData = ReadCache();
                }
               bool result=  ReadGitRepo( ref _jsonData);
                if (_jsonData != null && result)
                {
                    Data = JsonConfigurationFileParser.Parse(_jsonData);
                    OnReload();
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.ToString());
            }

            if (!_changeTrackingStarted)
            {
                _changeTrackingStarted = true;
                _ = Task.Run(async () => await LoadJsonFileAsync());
            }
        }

        private bool  ReadGitRepo( ref JsonDocument _jsonData)
        {
            bool result = false;
            try
            {
                if (__gitRepo.FileExists(_options.FileName))
                {
                    var fileContent = __gitRepo.GetFile(_options.FileName);
                    if (_jsonData == null)//如果远程文件存在，本地文件不存在， 就直接使用远程文件
                    {
                        _jsonData = JsonDocument.Parse(fileContent);
                        if (_jsonData != null)
                        {
                            SaveCache(fileContent);
                            result = true;
                        }
                    }
                    else
                    {//如果远程文件存在，本地文件存在， 就比较两个文件的差异，如果有差异， 就合并差异，然后缓存到本地，然后重新加载
                        var _diff = JsonDiff<JsonNode>.Diff(_jsonData, JsonDocument.Parse(fileContent)).ToArray();
                        if (_diff.Any())
                        {

                            var jp = new JsonPatch(_diff.ToArray());
                            var _jn = jp.Apply(_jsonData.RootElement);
                            string _json = _jn.ToJsonString(new JsonSerializerOptions() { WriteIndented = true });
                            SaveCache(_json);
                            _jsonData = JsonDocument.Parse(_json);
                            result = true;

                        }
                    }
                }
                else
                {
                    if (_jsonData != null)//如果远程文件不存在，本地文件存在， 就上传本地文件
                    {
                        __gitRepo.PutFile(_options.FileName, _jsonData.ToJsonString(new JsonWriterOptions { Indented = true }), "local file upload to git repo");
                    }
                }
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.ToString());

            }
            return result;
        }

        private JsonDocument ReadCache()
        {
            JsonDocument document = null;
            try
            {
                document = JsonDocument.Parse(File.ReadAllText(_options.CacheToFile));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"cant't load cache .{ex.Message}");
            }
            return document;
        }

        private void SaveCache(string fileContent)
        {
            try
            {
                if(!string.IsNullOrEmpty(_options.CacheToFile))
                {
                    System.IO.File.WriteAllText(_options.CacheToFile, fileContent);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"cant't save  cache .{ex.Message}");
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