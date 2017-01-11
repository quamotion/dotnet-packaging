# Packaging utilities for .NET Core

This repository contains command-line extensions for the .NET Core CLI which make it easy to create
deployment packages (such as `.zip` files, tarballs or installers) for .NET Core applications.

The goal is to implement the following commands:

* `dotnet tarball` - Create a `.zip` file (for Windows) and a `.tar.gz` file for Linux and OS X
* `dotnet choco` - Create a Chocolatey package
* `dotnet msi` - Create a Windows Installer (msi) package
* `dotnet deb` - Create a Ubuntu/Debian installer
* `dotnet pkg` - Create a macOS installer

## Installation


Add the following entry to your `.csproj` file:

```xml

<DotNetCliToolReference Include="dotnet-tarball">

    <Version>0.1-*</Version>

</DotNetCliToolReference>

```

## Usage

Run `dotnet tarball`