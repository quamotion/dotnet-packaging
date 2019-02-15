# Packaging utilities for .NET Core

[![NuGet](https://img.shields.io/nuget/v/Packaging.Targets.svg)](https://www.nuget.org/packages/Packaging.Targets)
[![Build status](https://ci.appveyor.com/api/projects/status/ac3j676f9g8r0g15?svg=true)](https://ci.appveyor.com/project/qmfrederik/dotnet-packaging)

This repository contains command-line extensions for the .NET Core CLI which make it easy to create
deployment packages (such as `.zip` files, tarballs or installers) for .NET Core applications.

The following commands are already available:
* `dotnet tarball` - Create a `.tar.gz` file for Linux and OS X
* `dotnet rpm` - Create a CentOS/RedHat Linux installer
* `dotnet zip` - Create a `.zip` file
* `dotnet deb` - Create a Ubuntu/Debian Linux installer

And these are up next:

* `dotnet pkg` - Create a macOS installer
* `dotnet choco` - Create a Chocolatey package
* `dotnet msi` - Create a Windows Installer (msi) package

Did we miss anything? Feel free to file a feature request, or send a PR!

## Installation

Add the following entry to your `.csproj` file, under the `Project` node. You do not need to add all `dotnet-*` entries;
only add those you are going to use.

```xml
  <ItemGroup>
    <PackageReference Include="Packaging.Targets" Version="0.1.1-*" />
    <DotNetCliToolReference Include="dotnet-tarball" Version="0.1.1-*" />
    <DotNetCliToolReference Include="dotnet-zip" Version="0.1.1-*" />
    <DotNetCliToolReference Include="dotnet-rpm" Version="0.1.1-*" />
    <DotNetCliToolReference Include="dotnet-deb" Version="0.1.1-*" />
  </ItemGroup>
```

## Usage

From the command line, run `dotnet rpm`, `dotnet zip` or `dotnet tarball` to create a `.rpm`, `.zip` or `.tar.gz` archive of the published output of your project.

All commands take the following command line arguments:

* `-r`, `--runtime`: Required. The target runtime has to be specified in the project file. For example, `win7-x64` or `ubuntu.16.10-x64`.
* `-f`, `--framework`: Required. The target framework has to be specified in the project file. For example, `netcoreapp1.1` or `net462`.
* `-c`, `--configuration`: Target configuration. The default for most projects is 'Debug'.
*  `---version-suffix`: Defines the value for the `$(VersionSuffix)` property in the project.


### Note
If you have multiple `.csproj` files in a single directory, [.NET Core CLI tools don't work](https://github.com/dotnet/cli/issues/4808).
If that's the case, you can still invoke the packaging tools manually:

```
dotnet msbuild [your-project].csproj /t:CreateZip /p:TargetFramework=netcoreapp1.1 /p:RuntimeIdentifier=win7-x64 /p:Configuration=Release
```
