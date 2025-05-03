# Common.FileManager

[![NuGet version](https://img.shields.io/nuget/v/Common.FileManager.svg?style=flat-square)](https://www.nuget.org/packages/Common.FileManager)

**Common.FileManager** is a lightweight and extensible file management library for .NET applications. It provides a consistent interface for file storage operations with a local file system implementation out of the box and is designed to be extended for cloud storage providers like Azure Blob Storage or Amazon S3.

---

## 📚 Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Getting Started](#-getting-started)
- [Usage Example](#-usage-example)
- [Interfaces](#-interfaces)
- [Extending to Cloud Providers](#-extending-to-cloud-providers)
- [FAQ](#-faq)
- [License](#-license)
- [Contributing](#-contributing)

---

## ✨ Features

- ✅ Upload and retrieve files via stream
- ✅ Get file metadata (name, size, timestamp)
- ✅ Delete files safely
- ✅ Supports dependency injection
- ✅ Configurable via `appsettings.json`
- ✅ Designed for easy extension (e.g., Azure, S3)

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package Common.FileManager
```

🔗 [View on NuGet](https://www.nuget.org/packages/Common.FileManager)

---

## ⚙️ Configuration

Add the base path to your `appsettings.json`:

```json
{
  "FileStorage": {
    "BasePath": "C:\\FileStorage"
  }
}
```

---

## 🚀 Getting Started

1. Install the NuGet package:

```bash
dotnet add package Common.FileManager
```

2. Register the service in `Program.cs`:

```csharp
builder.Services.AddScoped<IFileManagerPapertrail, LocalFileManager>();
```

3. Inject and use the interface in your controller or service.

---

## 🧑‍💻 Usage Example

### Uploading a file in a controller

```csharp
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    using var stream = file.OpenReadStream();
    var success = await _fileManager.SaveFile(file.FileName, stream);
    return success ? Ok("Uploaded!") : StatusCode(500, "Failed");
}
```

---

## 📁 Interfaces

- `IFileManagerPapertrail` – supports saving, retrieving, deleting files and getting file metadata
- `LocalFileManager` – default implementation using the local file system

---

## 🛠️ Extending to Cloud Providers

To add Azure/S3 support:

1. Implement `IFileManagerPapertrail`
2. Register your provider conditionally in DI
3. Add configuration for your provider (e.g., Azure Blob settings)

```csharp
builder.Services.AddScoped<IFileManagerPapertrail, AzureFileManager>();
```

---

## 💬 FAQ

**Q: Can I use this in a web API project?**  
A: Yes, just register `IFileManagerPapertrail` and inject it in your controllers.

**Q: What happens if the file already exists?**  
A: The file will be overwritten unless you implement versioning.

---

## 🔒 License

This project is licensed under the [MIT License](LICENSE).

---

## 📬 Contributing

Feel free to open issues or submit PRs to extend support for:
- Azure Blob Storage
- Amazon S3
- File encryption and access control
