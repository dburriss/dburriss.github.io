---
layout: post
title: "Canopy as a FSX Script"
subtitle: "The bare minimum needed to get Canopy up and running"
description: "Examples of running Canopy Selenium automation in a F# fsx script file"
permalink: canopy-as-fsx
author: "Devon Burriss"
category: Software Development
tags: [Canopy,Functional,F#,Testing,Selenium]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/explore-590.jpg"
published: true
---
Recently I found myself doing a very repetitive task that entailed copying values one at a time off a page, navigating to the next page, then repeat. I would spend 2 hours automating 1 hour of work if said work is sufficiently boring, even if I may never need the automation again. I enjoy coding, I do not enjoy copy-pasting. So I wondered if it was even possible to run Canopy in an F# FSX script file. It turns out it is.
<!--more-->

## F# Scripting

In case you are new to F# let us briefly touch on what a FSX file is. F# code can be placed into `.fs` files in a project and compiled to DLLs. This is how you would write a console application, Windows Service, or a Web Application. Another option that is great for experimenting is using `.fsx` files (and nowadays C# as well with .csx). These are F# scripting file that allow you to run as a standalone script using **FSI** (FSharp Interactive).

```powershell
fsi .\basic.fsx 
```

This requires `Fsi.exe` be on your **PATH**. For more information see [the docs](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/fsharp-interactive/).

Worth mentioning is [Ionide Project's](http://ionide.io/) great support for running script files, as well as working with [PAKET](https://fsprojects.github.io/Paket/) which we will not go into in detail.

## Setup

So the first thing you will need is a way to pull down the necessary Nuget packages. See my article on [getting up and running with Paket fast](/up-and-running-with-paket) if you need help setting up Paket.

Here is the TL;DR version:

```powershell
# Download Paket exe into .paket folder
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "iex (Invoke-WebRequest 'https://gist.githubusercontent.com/dburriss/b4075863873b5871d34e32ab1ae42baa/raw/b09c0b3735ef2392dcb3b1be5df0ca109b70d24e/Install-Paket.ps1')"
# Most NB this creates 'paket.dependencies' file
.\.paket\paket.exe init
# At this point add some lines to 'paket.dependencies'. Downloads dependencies.
.\.paket\paket.exe install
```

## A basic script

First we use PAKET to pull down the Nuget package we need.

```text
source https://www.nuget.org/api/v2
nuget canopy
```

And run `.\.paket\paket.exe install` to download the packages.

```fsharp
#r "packages/Selenium.WebDriver/lib/netstandard2.0/WebDriver.dll"
#r "packages/canopy/lib/netstandard2.0/canopy.dll"

open canopy.classic
open canopy.configuration
open canopy.types

chromeDir <- "C:\\tools\\selenium\\"//or wherever you place your Selenium
start chrome
pin FullScreen

url "https://google.com/"

"[name=q]" << "Youtube: BGF Red and Blue"
press enter
```

> One gotcha I did run across here is that the order of the `#r` references here does matter. The *WebDriver.dll* is required before *canopy.dll*.

For more advanced examples see the [related Github repository](https://github.com/dburriss/CanopyFSX/).

## Conclusion

And that is how easy it is to start using Canopy from a FSX file. This is a great way of automating some repetitive web task where an API is not available or exploring interacting with some DOM elements via Canopy in preparation for a UI test. Hope you found this useful. If you have any other use-cases, I would love to hear about them in the comments below.

## Resources

1. [Canopy](https://lefthandedgoat.github.io/canopy/)
1. [FSI Reference](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/fsharp-interactive/)
1. [Selenium Download](https://www.seleniumhq.org/download/)