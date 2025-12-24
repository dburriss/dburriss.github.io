---
layout: post
title: "Developer Quest I - Getting started with C#"
subtitle: "So you want to be a C# developer?"
author: "Devon Burriss"
category: Software Development
tags: [Programming, Learning c#, Tutorial]
comments: true
permalink: developer-quest-getting-started
excerpt_separator: <!--more-->
published: true
topics: [notes-reflections]
keywords: [Software Development, Programming, Learning c#, Tutorial]
---
## Getting Started

The first thing you are going to need as a developer is an Integrated Development Environment (IDE). Technically this is not necessary, you could use a text editor and the compiler in command line but trust me, you don't want to go that route.
Head over to http://www.visualstudio.com/downloads/download-visual-studio-vs and download Microsoft Visual Studio Express for Windows Desktop.

![hero running from monster](/img/posts/2014/quest-for-glory-i-so-you-want-to-be-a-hero-dos-title-73699.jpg)

> Update: [Visual Studio Community 2015](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) is now available which is still free but much fuller featured.

<!--more-->

## Your first application
We are going to be building a Console Application initially, since this is probably the easiest to get up and running with.
A console application project is what is used to create .exe programs that you may have seen or used.
Once you have Visual Studio installed, launch it and follow these steps to create the Console Application.

* Click **File > New Project...**
* In the left-hand tree structure menu pick Visual C# and select **Console Application**
* In the name field enter **DeveloperQuest1**
* Click **OK**

![VS New Project Window](/img/posts/2014/new-project.jpg)
Visual Studio will now create a solution for you. A solution can hold many projects. A project can be a console app, a Windows Store app, a desktop application, website, etc. The solution file groups all these together for you in a way that lets you easily create references to related projects. Don't worry about it too much at the moment. We will come back to it in another tutorial.
You should now have a screen that looks similar to this (may differ slightly based on your setup and theme).
![new console application](/img/posts/2014/ide.jpg)
The IDE shows 3 main windows above.

* **Document Editor** - this is where you edit your program files. Currently it shows the Program.cs source file, which is the starting point for the console application.
* **Solution Explorer** - allows you to browse the contents of your solution, open files and view properties of items in the solution.
* **Output Window** - shows messages of what Visula Studio is doing.

If you hit the **F5** key Visual Studio will build and run the application. Building basically means it takes your **.cs** files in the solution and turns them in instructions that a computer can understand.
So lets make a change to the program and run it. Add the folowing lines within the {} in the **Main** method of **Program** so it looks like this.
{% highlight csharp %}
System.Console.WriteLine("So you want to be a C# developer?");
System.Console.ReadKey();
{% endhighlight %}
Also remove all the *using* statements at the top from line 1 - 5.
![added console writeline charp code](/img/posts/2014/code-change1.jpg)
Now hit **F5** again to build and run the application. The console application should ask you if this is the path for you. If it is, look out for the following tutorial in this series. 
## Whats Next?
Next we will be looking at how you can capture input from the console application so you can interact with it.
If you have any questions or suggestions, please don't hesitate to leave a comment below. Happy coding!
[The adventure continues here.](http://devonburriss.me/developer-quest-variables/)
