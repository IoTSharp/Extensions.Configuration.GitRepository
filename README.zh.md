##  IoTSharp/Extensions.Configuration.GitRepository

### 项目概述
**Git repository configuration provider** 是一个用于管理和加载配置文件的库，专门为 C# 开发者设计。它允许开发者从 Git 仓库中直接读取配置文件，以便在分布式系统或微服务架构中实现集中配置管理和版本控制。

[![Build status](https://ci.appveyor.com/api/projects/status/egfxe7u2b23672j6?svg=true)](https://ci.appveyor.com/project/MaiKeBing/extensions-configuration-gitrepository)
[![NuGet Count](https://img.shields.io/nuget/dt/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://www.nuget.org/packages/Extensions.Configuration.GitRepository/)
[![Issues Open](https://img.shields.io/github/issues/IoTSharp/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/issues)
![NuGet Version](https://img.shields.io/nuget/v/Extensions.Configuration.GitRepository)


## 配置存储平台

此项目现在新增了从以下平台获取配置文件的支持：
- **Gitee.com**
- **GitLab**
- **Gitea**
- **GitHub**

通过此功能，您可以从上述平台的 Git 仓库中直接获取配置文件，进一步扩展了配置的来源，方便在多平台环境下管理和加载配置文件。

### 主要功能
- **从 Git 仓库加载配置**：允许你从指定的 Git 仓库中加载配置文件，支持多种格式（如 JSON、YAML 等）。
- **版本控制**：借助 Git 的强大功能，轻松跟踪配置文件的变更历史，确保配置的可追溯性和版本管理。
- **动态刷新**：支持动态刷新配置文件，当 Git 仓库中的配置文件发生变化时，可以自动更新配置。
- **灵活的配置源**：支持从多个分支或标签中读取配置，适应不同环境（开发、测试、生产）下的配置需求。
- **安全性**：通过 SSH 或 HTTPS 访问 Git 仓库，确保配置文件的安全传输和访问控制。

### 使用场景
- **分布式系统**：在分布式系统中，集中管理和分发配置文件，确保各个服务的一致性和可管理性。
- **微服务架构**：在微服务架构中，通过 Git 仓库统一管理配置文件，简化配置管理流程。
- **动态配置管理**：适用于需要动态更新配置的场景，比如配置文件频繁变更的应用程序。
- **版本控制**：在需要严格版本控制和变更管理的场景下，利用 Git 的版本控制功能，确保配置文件的可追溯性。

### 安装和使用
#### 安装
你可以通过 NuGet 包管理器安装此库。运行以下命令安装：

```shell
dotnet add package Extensions.Configuration.GitRepository
```

#### 示例代码
以下是一个简单的示例，展示如何使用此库从 Git 仓库中加载配置文件：

```csharp
using Microsoft.Extensions.Configuration;
using Extensions.Configuration.GitRepository;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddGitRepository(options =>
            {
                options.RepositoryUrl = "https://gitlab.com/your-repo.git";
                options.Branch = "main";
                options.FilePath = "appsettings.json";
                options.PollingInterval = TimeSpan.FromMinutes(5);
            });

        IConfiguration configuration = builder.Build();

        string mySetting = configuration["MySetting"];
        Console.WriteLine($"MySetting: {mySetting}");
    }
}
```

### 贡献指南
我们欢迎社区贡献者的参与。你可以通过以下方式贡献：

- **报告问题**：在 GitHub 的 Issues 页面报告 Bug 或提出功能建议。
- **提交 PR**：修复 Bug 或添加新功能，并通过 Pull Request 提交代码。
- **文档改进**：帮助改进项目的文档，使其更加清晰和易于理解。

### 许可证
该项目基于 MIT 许可证，详情请参阅 [LICENSE](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/blob/main/LICENSE) 文件。

 
### 感谢
灵感来源于 https://github.com/denis-ivanov/Extensions.Configuration.GitLab
