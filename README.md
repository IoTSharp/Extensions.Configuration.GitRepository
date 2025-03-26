# Git repository configuration provider
[![GitHub Tag](https://img.shields.io/github/tag/IoTSharp/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/releases)
[![NuGet Count](https://img.shields.io/nuget/dt/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://www.nuget.org/packages/Extensions.Configuration.GitRepository/)
[![Issues Open](https://img.shields.io/github/issues/IoTSharp/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/issues)

### Example:

```csharp
builder.Configuration.AddGitRepository(cfg =>
    cfg.WithHostUrl("https://gitlab.com/")
        .WithRepositoryPath("IoTSharp/gitlabcfg")
        .WithAuthenticationToken("<you token>")
        .WithFileName("cfg.json")
    );
```
