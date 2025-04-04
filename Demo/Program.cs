var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Configuration.AddGitRepository("https://gitlab.com/", "IoTSharp/gitlabcfg", "", "cfg.json");
 
builder.Configuration.AddUserSecrets("personal_access_tokens");
builder.Configuration.AddGitRepository(cfg =>
                       cfg.WithHostUrl("https://git.uixe.net/")
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