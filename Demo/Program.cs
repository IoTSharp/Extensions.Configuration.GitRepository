var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Configuration.AddGitRepository("https://gitlab.com/", "IoTSharp/gitlabcfg", "", "cfg.json");
builder.Configuration.AddGitRepository(cfg =>
    cfg.WithHostUrl("https://gitlab.com/")
        .WithRepositoryPath("IoTSharp/gitlabcfg")
        .WithAuthenticationToken(Console.ReadLine())
        .WithFileName("cfg.json")
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
