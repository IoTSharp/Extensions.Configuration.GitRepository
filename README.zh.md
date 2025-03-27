# Git 仓库配置文件提供器
[![Build status](https://ci.appveyor.com/api/projects/status/egfxe7u2b23672j6?svg=true)](https://ci.appveyor.com/project/MaiKeBing/extensions-configuration-gitrepository)
[![GitHub Tag](https://img.shields.io/github/tag/IoTSharp/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/releases)
[![NuGet Count](https://img.shields.io/nuget/dt/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://www.nuget.org/packages/Extensions.Configuration.GitRepository/)
[![Issues Open](https://img.shields.io/github/issues/IoTSharp/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/issues)

### 示例:

```csharp
builder.Configuration.AddGitRepository(cfg =>
    cfg.WithHostUrl("https://gitlab.com/")
        .WithRepositoryPath("IoTSharp/gitlabcfg")
        .WithAuthenticationToken("<you token>")
        .WithFileName("cfg.json")
    );
```
### 感谢
灵感来源于 https://github.com/denis-ivanov/Extensions.Configuration.GitLab
