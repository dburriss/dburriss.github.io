---
layout: post
title: "Visual Studio Code Tasks"
subtitle: "Examples of some slightly different ones"
author: "Devon Burriss"
category: Tools
tags: [VS Code, Powershell, Pretzel]
comments: true
permalink: vscode-tasks
excerpt_separator: <!--more-->
published: true
---

I tend to try use [Visual Studio Code](https://code.visualstudio.com/) for tasks and languages I don't currently use on a day to day basis. Over the last few weeks that has included Java and Delphi. Then today I was trying to launch my blog from VS Code and ran into an issue because Pretzel listens for a console key. The only fix I could find for this was to launch a new Powershell window.

<!--more-->

# Tasks

Tasks are configured in */.vscode/tasks.json* from the worskspace root. Hit **ctrl+Shift+P** and type **Tasks:C** and hit enter or click 'Tasks: Configure Task Runner'. If it does not exist it will be created.

## Compiling a Java application

This command uses `javac` to compile the Java application and will report on compile errors. Note that this uses a single command. It assumes `javac` is on your PATH.

```json
{
    "version": "0.1.0",
    "command": "javac",
    "showOutput": "silent",
    "isShellCommand": true,
    "args": ["-d","${workspaceRoot}\\bin","${workspaceRoot}\\src\\*.java"],
    "problemMatcher": {
        "owner": "external",
        "fileLocation": ["absolute"],
        "pattern": [
        {
            "regexp": "^(.+\\.java):(\\d):(?:\\s+(error)):(?:\\s+(.*))$",
            "file": 1,
            "location": 2,
            "severity": 3,
            "message": 4
        }]
    }
}
```

## Control Maven for Java project

These control different Maven phases. Note that on the `exec` task you need to change the `me.devonburriss.App` to the entrypoint of your application. It assumes `mvn` is on your PATH.

```json
{
    "version": "0.1.0",
    "command": "mvn",
    "isShellCommand": true,
    "showOutput": "always",
    "suppressTaskName": true,
    "echoCommand": true,
    "tasks": [
        {
            "taskName": "verify",
            "args": ["-B", "verify"],
            "isBuildCommand": true
        },
        {
            "taskName": "test",
            "args": ["-B", "test"],
            "isTestCommand": true
        },
        {
            "taskName": "clean install",
            "args": ["clean install -U"],
            "isTestCommand": true
        },
        {
            "taskName": "exec",
            "args": ["-B", "exec:java", "-D", "exec.mainClass=\"me.devonburriss.App\""]
        }
    ]
}
```

### Delphi (Free Pascal) Build

This is using the Free Pascal compiler to compile Delphi code. It assumes that `fpc` is on your PATH. You can get it [here](http://www.freepascal.org/download.var).  
This only compiles a single unit, not a complete project.

```json
{
    "version": "0.1.0",
    "command": "fpc",
    "isShellCommand": true,
    "showOutput": "always",
    "suppressTaskName": true,
    "echoCommand": true,
    "tasks": [
        {
            "taskName": "Compile Unit",
            "args": ["-Sd", "${file}"],
            "isBuildCommand": true
        }
    ]
}
```

### Powershell, Cake, Pretzel blog Build 

This is one I use to call PS, which executes my Cake build and and run this blog locally. The targets for that are Bake and Taste (from Pretzel). See [this post](http://devonburriss.me/pretezel-blog-appveyor-deployment/) for details on that.

I use a *run.ps1* file because I needed to launch a new Powershell window so Pretzel can wait and watch for changes. 

```json
{
    "version": "0.1.0",
    "tasks": [
        {
            "taskName": "Build",
            "command": "powershell",
            "isShellCommand": true,
            "args": [".\\pretzel.ps1"],
            "showOutput": "always",
            "isBuildCommand": true
        },
        {
            "taskName": "Run",
            "command": "powershell",
            "isShellCommand": false,
            "args": [".\\run.ps1"],
            "showOutput": "always",
            "isTestCommand": true
        }
    ]    
}
```

## Extra: F5 Launch of Pretzel Blog 

If you want to use **F5** to run the blog you can press **Ctrl+Shift+P** and type **launch**. If it doesn't exist a *launch.json* file will be created. 

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "type": "PowerShell",
            "request": "launch",
            "name": "PowerShell Launch (Script)",
            "script": "${workspaceRoot}/run.ps1",
            "args": [],
            "cwd": "${workspaceRoot}"        
        }
    ]
}
```

## Conclusion

Visual Studio Code is a great editor and has plenty of extension points. If you have any great tips I would love to hear about them in the comments.