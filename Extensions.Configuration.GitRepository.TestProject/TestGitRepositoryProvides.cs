using Microsoft.Extensions.Configuration;

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
        [DataRow("GitLab", "https://gitlab.com/", "maikebing/gitcfg", "WithGitLab", typeof(GitLabProviderExtensions), null, DisplayName = "GitLabProvider")]
        [DataRow("GitHub", "https://github.com/", "maikebing/gitcfg", "WithGitHub", typeof(GitHubProviderExtensions), "http://127.0.0.1:7890", DisplayName = "GitHubProvider")]
        [DataRow("Gitee", "https://gitee.com/", "maikebing/gitcfg", "WithGitee", typeof(GiteeProviderExtensions), null, DisplayName = "GiteeProvider")]
        [DataRow("Gitea", "https://gitea.com/", "maikebing/gitcfg", "WithGitea", typeof(GiteaProviderExtensions), null, DisplayName = "GiteaProvider")]
        [DataRow("Gitee", "https://gitee.com/", "r6bhzi1y90av2qtslgmwe86", "WithGiteeGists", typeof(GiteeProviderExtensions), null, DisplayName = "GiteeGistsProvider")]
        public void TestProvider(string _proveiderName, string hosturl, string repoPath, string setProveiderMethodName, Type extType, string proxy)
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
                extType.GetMethod(setProveiderMethodName)?.Invoke(null, [cfg]);
                cfg = cfg.WithHostUrl(hosturl)
                    .WithRepositoryPath(repoPath)
                    .WithAuthenticationToken(config.GetValue<string>(_proveiderName))
                    .WithFileName($"{_proveiderName}.json")
                    .WithCache(cfgfilename)
                    .WithProxy(proxy);
            });
            var cfg = _builder.Build();
            Assert.IsNotNull(cfg);
            Assert.AreEqual(cfg.GetValue<string>(_proveiderName), _proveiderName);
        }
    }
}