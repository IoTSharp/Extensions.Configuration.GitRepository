namespace DemoUseStartup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, builder) =>
                 {

                     var pat = builder.AddUserSecrets("personal_access_tokens").Build();
                     builder.AddGitRepository(cfg =>
                                            cfg.WithGitLab()
                                            .WithHostUrl("https://gitlab.com/")
                                                .WithRepositoryPath("IoTSharp/gitlabcfg")
                                                .WithAuthenticationToken(pat.GetValue<string>("personal_access_tokens"))
                                                .WithFileName($"{Environment.GetEnvironmentVariable("UIXEID")}/cfg.json")
                                                .WithCache($"{context.HostingEnvironment.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}appsettings.{context.HostingEnvironment.EnvironmentName}.json")
                                            );
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
