---
layout: post
title: "F# Scripts"
subtitle: "Exploring running F# from a fsx script file"
description: "Start exploring code faster by running F# as a fsx script file"
permalink: fsharp-scripting
author: "Devon Burriss"
category: Software Development
tags: [F#,FSX]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/vents-bg.jpg"
social-img: "img/posts/2018/script-500.jpg"
published: true
---
Using F# scripts is something I only started doing after dabbling in F# for quite a while. This is unfortunate because they are a really fast and easy way of throwing some code together and thus a really good way to learn F#. This post is for anyone getting started with F# scripting.
<!--more-->

## Installation

Check out the [documentation for installing F#](https://docs.microsoft.com/en-us/dotnet/fsharp/get-started/install-fsharp?tabs=windows). The easiest way is to install F# as part of Visual Studio. You may still need to add FSI.exe to your **PATH**.

1. [Instructions for Windows](https://fsharp.org/use/windows/)
1. [Instructions for MacOS](https://fsharp.org/use/mac/)
1. [Instructions for Linux](https://fsharp.org/use/linux/)

## F# Interactive

FSI allows you to execute F# code in an interactive console. Just type `fsi.exe` on Windows or `fsharpi` on Linux/Mac.

```fsharp
> let x = 1;;
val x : int = 1
```

> Note: Each expression needs to end with `;;` in the interactive window.

To get help: `#help;;`  
To quit: `#quit;;`

## Scripting

So entering code directly into fsi is ok for trying simple things out but what about more complex code. That is where `.fsx` files come in. 

Imagine we have a file called *print-name.fsx* with the following content:

```fsharp
let name = "Devon"
printfn "Name: %s" name
```

Executing it we would see the following:

> &gt; fsi .\samples\print-name.fsx  
> &gt; Name: Devon

### Including other fsx files

You can load other fsx files into a script. If we have `Strings.fsx` containing the following code:

```fsharp
let toUpper (s:string) = s.ToUpper()
let toLower (s:string) = s.ToLower()
let replace (oldValue:string) (newValue:string) (s:string) = s.Replace(oldValue,newValue)

module StringBuilder =
    open System.Text
    let init() = new StringBuilder()
    let initWith(s:string) = new StringBuilder(s)
    let append (s:string) (sb:StringBuilder) = sb.Append(s)
```

We can now use it in our script file like so:

```fsharp
#load "Strings.fsx"
open Strings
let name = "Devon" |> StringBuilder.initWith
            |> StringBuilder.append " Burriss"
            |> string |> toUpper
printfn "Name: %s" name
```

Executing it we would see the following:

> &gt; fsi .\samples\print-name.fsx  
> &gt; Name: DEVON BURRISS

### Taking Arguments

It is possible to pass arguments into a script file. They are available in a field `fsi.CommandLineArgs`. Let's change our script one more time to demonstrate the usage. The arguments come through as an array so we pattern match on the number of elements to decide what to print.

> Note: The first element of the array is always the name of the script the arguments are passed into

```fsharp
#load "Strings.fsx"
open Strings
let stringWithSpace x = x |> string |> (sprintf " %s")
let name first = first |> toUpper
let nameAndLastName first last = first |> StringBuilder.initWith |> StringBuilder.append last |> stringWithSpace |> toUpper
let nameAndLastNameWithOccupation first last occ = 
    first |> StringBuilder.initWith 
    |> StringBuilder.append " " 
    |> StringBuilder.append last 
    |> StringBuilder.append (sprintf " (%s)" occ)
    |> string |> toUpper

match fsi.CommandLineArgs with
| [|scriptName;|] -> failwith (sprintf "At least a name required for %s" scriptName)
| [|_;firstName|] -> name firstName |> printfn "Name: %s"
| [|_;firstName; lastName|] -> nameAndLastName firstName lastName |> printfn "Name: %s"
| [|_;firstName; lastName; occ|] -> nameAndLastNameWithOccupation firstName lastName occ |> printfn "Name: %s"
| _ -> failwith (sprintf "Too many arguments %A" (fsi.CommandLineArgs |> Array.tail))
```

Executing it we would see the following:

> &gt; fsi .\samples\print-name.fsx devon burriss developer  
> &gt; Name: DEVON BURRISS (DEVELOPER)

### Referencing DLLs

You can reference DLLs using `#r "path/to/file.dll"`. If you want to pull down DLLs from Nuget, check out [my article on using Paket dependency manager](/up-and-running-with-paket).

## Resources

1. [FSI Documentation](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/fsharp-interactive/)
1. [10 Tips for Productive F# Scripting](http://brandewinder.com/2016/02/06/10-fsharp-scripting-tips/)

## Credits

1. Social Image by [yifei chen](https://unsplash.com/@0628fromchina)