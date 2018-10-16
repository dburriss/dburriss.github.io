---
layout: post
title: "Up and running with PAKET"
subtitle: "The fastest ways to get going with PAKET dependency manager"
description: "Instructions on getting up and running fast with Paket dependency manager"
permalink: up-and-running-with-paket
author: "Devon Burriss"
category: Tools
tags: [Paket,Nuget,Powershell,F#,C#]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/stuff-bg.jpg"
social-img: "img/posts/2018/package-500.jpg"
published: true
---
[Paket](https://fsprojects.github.io/Paket/) is an awesome dependency manager for .NET. Comparing it to Nuget is both the easiest way to explain the basics of it and also a massive disservice to Paket. In this post I want to share some tips to make working with Paket even more awesome.
<!--more-->
The biggest problem with working with Paket has nothing to do with Paket itself, or even it's differences with Nuget. The biggest issue is that the ecosystem is geared to make using Nuget really easy. The tooling is all geared such that Nuget is embedded in your project whether you like it or not. So let's see how we can make working with Paket as smooth as possible. This ease isn't a bad thing, it just means the barrier to entry for something better seems high. Let us see what we can do about that.

## TL;DR

If you just want the commands to have Paket up and running in a folder fast:

```powershell
# Download Paket exe into .paket folder
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "iex (Invoke-WebRequest 'https://gist.githubusercontent.com/dburriss/b4075863873b5871d34e32ab1ae42baa/raw/b09c0b3735ef2392dcb3b1be5df0ca109b70d24e/Install-Paket.ps1')"
# Most NB this creates 'paket.dependencies' file
.\.paket\paket.exe init
# At this point add some lines to 'paket.dependencies'. Downloads dependencies.
.\.paket\paket.exe install
```

## Install fast

So as I mentioned, Nuget is there by default. Paket is not. You can [install Paket manually](https://fsprojects.github.io/Paket/getting-started.html#Downloading-Paket-s-Bootstrapper) but I wanted to provide another option. Let's create a Powershell script to install Paket with a single line.

### One liner

To work with Paket you need the binary available. Usually this is in a folder named *.paket* in the root of your solution. I have created [a Gist file](https://gist.github.com/dburriss/b4075863873b5871d34e32ab1ae42baa) that you can download and execute with a single line that will do just that.

```powershell
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "iex (Invoke-WebRequest 'https://gist.githubusercontent.com/dburriss/b4075863873b5871d34e32ab1ae42baa/raw/b09c0b3735ef2392dcb3b1be5df0ca109b70d24e/Install-Paket.ps1')"
```

### Part of the family

If you find yourself needing to setup Paket often as can happen if you are using F# fsx scripting files often, you may want to create an easier to remember command. The easiest way to do this is to add a function call to your Powershell profile.

Edit *"C:\Users\ &lt; your username &gt;\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1"* on Windows or *~/.config/powershell/profile.ps1* on Mac and add the following function:

```powershell
function New-Paket {
    New-Item -ItemType directory -Path ".paket"
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    $tag = (Invoke-WebRequest -Uri https://api.github.com/repos/fsprojects/Paket/releases | ConvertFrom-Json)[0].tag_name
    $uri = " https://github.com/fsprojects/Paket/releases/download/" + $tag + "/paket.bootstrapper.exe"
    Invoke-WebRequest $uri  -OutFile .paket/paket.exe
}
```

> You can reload your profile to make the command available in an already open console with `& $profile`

This will allow you to install paket with a simple `New-Paket` Powershell command.

## Adding dependencies

Once you have Paket binary installed you can initialize by typing `.\.paket\paket.exe init`.

This creates a *paket.dependencies* file. This is where you place all the dependencies your solution uses. As an example:

```yaml
source https://www.nuget.org/api/v2
nuget NETStandard.Library
nuget canopy
```

To download the referenced packages execute `.\.paket\paket.exe install`.

> Note: a *paket.lock* file is generated to ensure you get the same version every time. This should be committed to source control.

At this point you have enough to work with Paket when using it with FSX script files.

You can reference them like so in your fsx files:

```fsharp
#r "packages/Selenium.WebDriver/lib/netstandard2.0/WebDriver.dll"
#r "packages/canopy/lib/netstandard2.0/canopy.dll"
```

Check out the [Paket FSI documentation](https://fsprojects.github.io/Paket/reference-from-repl.html) for an alternative way to get going in a script file.

## Going further

When using Paket with projects (csproj/fsproj) there are a few more things to know. Most important is that in each project folder you need a *paket.references* file. This describes which dependencies from the *paket.dependencies* file it are used in any given project.

Something important to note here is that the *csproj/fsproj* files need to reference *.paket/paket.targets*. This usually looks something like this:

```xml
<Import Project="..\..\.paket\Paket.Restore.targets" />
```

And the project file now no longer needs to reference nuget packages.

If you have an existing project you want to convert from Nuget to Paket there is a handy command for just that `.\.paket\paket.exe convert-from-nuget`.

If you want more details on how Paket works I recommend [Isaac's introduction to Paket](https://cockneycoder.wordpress.com/2017/08/07/getting-started-with-paket-part-1/) and of course the [Paket documentation](https://fsprojects.github.io/Paket/).

> Paket can do more than pull in Nuget packages. It can pull files from disk, git, and entire repositories.

## Conclusion

Paket is an awesome replacement for Nuget and in this article we looked at how you can get up and running fast as well as make sure it is as easy as possible to get Paket quickly every time you need it.

## Resources

1. [Installing Powershell on Windows](https://docs.microsoft.com/en-us/powershell/scripting/setup/installing-powershell-core-on-windows?view=powershell-6)
1. [Installing Powershell on Linux](https://docs.microsoft.com/en-us/powershell/scripting/setup/installing-powershell-core-on-linux?view=powershell-6)
1. [Installing Powershell on MacOS](https://docs.microsoft.com/en-us/powershell/scripting/setup/installing-powershell-core-on-macos?view=powershell-6)

## Credits

1. Header image [Vitor Santos](https://unsplash.com/@vtrsnts)
1. Social image [Kira auf der Heide](https://unsplash.com/@moonshinechild)