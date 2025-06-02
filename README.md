# MPP Reader SDK
A simple SDK to read Microsoft Project (MPP) files.

## Author
- Junpyo

## Tech Stack

- .NET 8
- C#
- Microsoft Project (MPP) file parsing

## Getting Started

### Running the Application

```bash
dotnet run
```

### Building the Application

To publish a binary:

```bash
dotnet publish -c Release -o ./build
```

Or, to create a self-contained binary for a specific platform:

```bash
dotnet publish -c Release -r osx-x64 --self-contained true -o ./build
```

> **Note:**  
> Replace `osx-x64` with your target runtime identifier (e.g., `win-x64`, `linux-x64`, `osx-arm64`).  
> See the [.NET RID catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog) for more options.

## License
This project is open source and distributed under the MIT License. See the [LICENSE](./LICENSE) file for details.