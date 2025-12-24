---
layout: post
title: "Building a Cake Script"
subtitle: "Verifying syntax for Cake scripts by just building"
author: "Devon Burriss"
category: Tools
tags: [VS Code, Powershell, Cake]
comments: true
permalink: cake-build
excerpt_separator: <!--more-->
published: true
topics: [tooling-automation]
keywords: [Tools, VS Code, Powershell, Cake]
---
[CAKE](http://cakebuild.net/) is a great automation DSL that uses C#. Not only is it comfortable for C# developers to script automation tasks in, it has a stack of built in functionality and a great ecosystem of addins that give you a great jumpstart for just about anything you would like to automate.

This is a quick tip on how to create a Visual Studio Code task that will build your Cake script. This is a great way of verifying your scripts without actually running Cake tasks. 
Also make sure you have the Visual Studio Code extension for Cake installed to give you syntax highlighting.

<!--more-->

## Creating a tasks.json file

Press **Ctrl+Shift+P** and type **Tasks:C** and hit enter or click 'Tasks: Configure Task Runner'. If the file does not exist it will be created. If there is an existing build task be sure to replace it. Note that that this is building the cake script, not building whatever project your Cake script is probably meant to build. That being said, if you are using Cake to build something, this task described here should probably be a custom task, not the build task.

## Adding our Cake build task

Now that we have  add the following task to the json tasks array.

```json
{
    "taskName": "Build",
    "command": "powershell",
    "isShellCommand": true,
    "args": [".\\build.ps1 -Whatif"],
    "showOutput": "always",
    "isBuildCommand": true
}
```

Cake works by running a powershell script (default is *build.ps1*) that uses Roslyn to compile the Cake file. What our script does is execute the build script and trigger a compile but without actually executing any tasks. Not even the Default one. This is done by adding the `-Whatif` argument flag.  
In the example above the `isBuildCommand` is set to **true** so that **Ctrl+Shift+B** can be used to build the *build.cake* file.

## Conclusion

Automating your builds, testing and deployment is important but don't stop there. Making sure your workspace feedback cycle is fast can also be a great way to increase productivity and decrease frustration. Hope this quick tip helps someone. Leave a comment if you have any of your own Cake tips.
