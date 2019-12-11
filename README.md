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
* `dotnet snap` - Create a Linux Snapcraft package
* `dotnet flat` - Create a Linux Flatpak package

Did we miss anything? Feel free to file a feature request, or send a PR!

## Installation

First, install the .NET Packaging tools. You don't need to install all tools if you only plan to use one.

```bash
dotnet tool install --global dotnet-zip
dotnet tool install --global dotnet-tarball
dotnet tool install --global dotnet-rpm
dotnet tool install --global dotnet-deb
```

Then, in your project directory, run `dotnet {zip|tarball|rpm|deb} install` to add the tool to your project:

```bash
dotnet zip install
dotnet tarball install
dotnet rpm install
dotnet deb install
```

## Usage

From the command line, run `dotnet rpm`, `dotnet zip` or `dotnet tarball` to create a `.rpm`, `.zip` or `.tar.gz` archive of the published output of your project.

All commands take the following command line arguments:

* `-r`, `--runtime`: The target runtime to build your project for. For example, `win7-x64` or `ubuntu.16.10-x64`.
* `-f`, `--framework`: The target framework to build your project for. For example, `netcoreapp1.1` or `net462`.
* `-c`, `--configuration`: Target configuration. The default for most projects is 'Debug'.
* `-o`, `--output`: The output directory to place built packages in.
*  `---version-suffix`: Defines the value for the `$(VersionSuffix)` property in the project.
*  `--no-restore`: Skip the implicit call to `dotnet restore`.

All arguments are optional.

## Tutorial

Let's create a new console application and package it as a `.deb` file, so we can install it on our Ubuntu machine:

First, create your console application:

```bash
mkdir my-app
cd my-app
dotnet new console
```

Then, install the dotnet-deb utility:

```bash
dotnet tool install --global dotnet-deb
dotnet deb install
```

All set. Let's package your application as a deb package:

```bash
dotnet deb
```

There's now a `bin\Debug\netcoreapp3.0\my-app.1.0.0.deb` file wich you can install:

```bash
apt-get install bin\Debug\netcoreapp3.0\my-app.1.0.0.deb
```

Your application is installed into `/usr/local/share/my-app`. Invoke it by running `/usr/local/share/my-app/my-app`:

```bash
/usr/local/share/my-app/my-app
```

### Note
You can invoke the packaging tools manually, using a MSBuild target instead of using the a .NET Core CLI tool:

```
dotnet msbuild [your-project].csproj /t:CreateZip /p:TargetFramework=netcoreapp1.1 /p:RuntimeIdentifier=win7-x64 /p:Configuration=Release
```
