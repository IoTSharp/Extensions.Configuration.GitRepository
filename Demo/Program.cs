var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Configuration.AddUserSecrets("personal_access_tokens");
builder.Configuration.AddGitRepository(cfg => cfg.WithGitLab()
                                                .WithHostUrl("https://git.uixe.net/")
                                                .WithRepositoryPath("uixe/stdlanedevctlsvr")
                                                .WithAuthenticationToken(builder.Configuration.GetValue<string>("personal_access_tokens"))
                                                .WithFileName($"{Environment.GetEnvironmentVariable("UIXEID")}/appsettings.{builder.Environment.EnvironmentName}.json")
                                                .WithCache($"{builder.Environment.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}appsettings.{builder.Environment.EnvironmentName}.json")
                                        );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
Console.WriteLine($"abc={app.Configuration.GetValue<string>("abc")}");
app.UseAuthorization();
app.MapControllers();
app.Run();