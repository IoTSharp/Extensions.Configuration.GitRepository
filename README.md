## IoTSharp/Extensions.Configuration.GitRepository

### Project Overview
**Git repository configuration provider** is a library designed for managing and loading configuration files, specifically for C# developers. It allows developers to read configuration files directly from a Git repository, enabling centralized configuration management and version control in distributed systems or microservice architectures.

[![Build status](https://ci.appveyor.com/api/projects/status/egfxe7u2b23672j6?svg=true)](https://ci.appveyor.com/project/MaiKeBing/extensions-configuration-gitrepository)
[![NuGet Count](https://img.shields.io/nuget/dt/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://www.nuget.org/packages/Extensions.Configuration.GitRepository/)
[![Issues Open](https://img.shields.io/github/issues/IoTSharp/Extensions.Configuration.GitRepository.svg?style=flat-square)](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/issues)
![NuGet Version](https://img.shields.io/nuget/v/Extensions.Configuration.GitRepository)

### Key Features
- **Load Configuration from Git Repositories**: Allows you to load configuration files from a specified Git repository, supporting various formats (such as JSON, YAML, etc.).
- **Version Control**: Leverage Git's powerful features to easily track changes to configuration files, ensuring traceability and version management.
- **Dynamic Refresh**: Supports dynamic refresh of configuration files, automatically updating configurations when files in the Git repository change.
- **Flexible Configuration Sources**: Supports loading configurations from multiple branches or tags, adapting to different environments (development, testing, production).
- **Security**: Access Git repositories via SSH or HTTPS, ensuring secure transmission and access control of configuration files.

### Use Cases
- **Distributed Systems**: Centrally manage and distribute configuration files in distributed systems, ensuring consistency and manageability across services.
- **Microservice Architecture**: Simplify configuration management in a microservice architecture by unifying configuration file management through a Git repository.
- **Dynamic Configuration Management**: Suitable for scenarios requiring dynamic configuration updates, such as applications with frequently changing configuration files.
- **Version Control**: Utilize Git's version control features to ensure traceability and change management in scenarios requiring strict version control.

### Installation and Usage
#### Installation
You can install this library via the NuGet package manager. Run the following command to install:

```shell
dotnet add package Extensions.Configuration.GitRepository
```

#### Example Code
Here is a simple example demonstrating how to use this library to load configuration files from a Git repository:

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

### Contribution Guidelines
We welcome community contributions. You can contribute in the following ways:

- **Report Issues**: Report bugs or suggest features on the GitHub Issues page.
- **Submit PRs**: Fix bugs or add new features and submit your code through a Pull Request.
- **Improve Documentation**: Help improve the project's documentation to make it clearer and more understandable.

### License
This project is licensed under the MIT License. For details, please refer to the [LICENSE](https://github.com/IoTSharp/Extensions.Configuration.GitRepository/blob/main/LICENSE) file.

### Acknowledgments
Inspired by https://github.com/denis-ivanov/Extensions.Configuration.GitLab
