using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Configuration.AddUserSecrets("personal_access_tokens");
builder.Configuration.AddGitRepository(cfg => cfg.WithGitLab()
                                                .WithRepositoryPath("IoTSharp/gitlabcfg")
                                                .WithAuthenticationToken(builder.Configuration.GetValue<string>("personal_access_tokens"))
                                                .WithFileName($"appsettings.json")
                                                .WithCache($"{builder.Environment.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}appsettings.{builder.Environment.EnvironmentName}.json")
                                        );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
Console.WriteLine($"abc={app.Configuration.GetValue<string>("abc")}");

ChangeToken.OnChange(() => app.Configuration.GetReloadToken(), () =>
 {
     Console.WriteLine($"abc={app.Configuration.GetValue<string>("abc")}");
     var settings = app.Configuration.Get<AppSettings>();
     foreach (var item in settings?.Menus)
     {
         Console.WriteLine($"Menu={item}");
     }  
 });
 app.UseAuthorization();
app.MapControllers();
app.Run();

public class AppSettings
{
    public List<string> Menus { get; set; } = new List<string>();

}