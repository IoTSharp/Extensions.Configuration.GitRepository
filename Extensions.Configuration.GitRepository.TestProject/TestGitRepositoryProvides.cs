using Microsoft.Extensions.Configuration;
using Microsoft.Testing.Platform.Builder;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Extensions.Configuration.GitRepository.TestProject
{
    [TestClass]
    public sealed class TestGitRepositoryProvides
    {
        [TestInitialize]
        public void TestInit()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // This method is called after each test method.
        }

        [TestMethod]
        [DataRow("GitLab", "https://gitlab.com/", "maikebing/gitcfg", "WithGitLab", typeof(GitLabProviderExtensions), DisplayName = "GitLabProvider")]
        public void TestGitLabProvider(string _proveiderName, string hosturl, string repoPath, string setProveiderMethodName, Type extType)
        {
            IConfigurationBuilder _builder;
            IConfigurationRoot config;
            var cfgfilename = Path.GetTempFileName();
            _builder = new ConfigurationBuilder()
                        .AddUserSecrets<TestGitRepositoryProvides>();
            config = _builder.Build();
            System.IO.File.WriteAllText(cfgfilename, $"{{\"{_proveiderName}\":\"{_proveiderName}\"}}");
            _builder.AddGitRepository(cfg =>
            {
                cfg = cfg.WithHostUrl(hosturl)
                    .WithRepositoryPath(repoPath)
                    .WithAuthenticationToken(config.GetValue<string>(_proveiderName))
                    .WithFileName($"{_proveiderName}.json")
                    .WithCache(cfgfilename);
                extType.GetMethod(setProveiderMethodName)?.Invoke(null, [cfg]);
            });
            var cfg = _builder.Build();
            Assert.IsNotNull(cfg);
            Assert.AreEqual(cfg.GetValue<string>(_proveiderName), _proveiderName);
        }
    }
}
