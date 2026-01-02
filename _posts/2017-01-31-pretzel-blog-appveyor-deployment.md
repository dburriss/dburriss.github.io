---
layout: post
title: "Deploying a Pretzel generated static site to Github Pages using Appveyor"
subtitle: "Dynamic continuous delivery of static content."
author: "Devon Burriss"
comments: true
permalink: pretezel-blog-appveyor-deployment
excerpt_separator: <!--more-->
header-img: "img/backgrounds/vents-bg.jpg"
published: true
topics: [tooling-automation]
keywords: [Tools, Git, Github Pages, CI, AppVeyor, Pretzel]
---

---
# Background

I was using [Github Pages](https://pages.github.com/) and [Jekyll](https://jekyllrb.com/) to build and host this blog up until a few days ago. 
Getting Jekyll running on Windows (more specifically Ruby) is a gamble and running it in a Docker container just led me down Ruby gem issues with my theme.  
Finally I decided to stick with the statically generated site but move away from Jekyll. Enter Pretzel...
<!--more-->
## Github Pages

Github Pages allows you to host static websites and comes in 2 flavours. It natively supports building Jekyll source into a static site and deploying it.

### Organisation/User site

This one runs off a separate repository with the special convention based name of `<username>.github.io` and hosts any static content (or Jekyll) that is committed to **master** branch.

### Repository site

These allow a website to be hosted per repository. Think documentation and marketing site for the product being built in that repository. These are built from a special orphaned branch named **gh-pages** usually but can be set to **master** or a `/docs` folder.

## Pretzel

[Pretzel](https://github.com/Code52/pretzel) is a .NET based tool for generating a static, blog aware site. If you have used Jekyll, it is that without all the gem hell.  
Installing it locally is as easy as: `choco install pretzel`  

> Note that I used a plugin called [Pretzel.Categories](https://github.com/k94ll13nn3/Pretzel.Categories) to provide tag and category pages. You may need to explicitly add the dll to your repository as your global .gitignore may specify *.dll.  'git add .\_plugins\Pretzel.Categories.dll -f'

# Approach

Since I am no longer using Jekyll, Github pages can no longer build my site so I need to do that outside. I wanted to keep the same workflow of just being able to commit my changes and the content on the site is updated.  

The solution needed to satisfy the following:

1. develop locally and view my changes before pushing the commit
2. only 1 repository that represented my blog
3. a commit should trigger a build and deployment of the updated content

# Solution

Let's tackle each of these requirements one at a time. First off create a branch **source**. **master** will be reserved for our auto-generated content (we will get to this at the end of the post).  
`git checkout --orphan source`

## Local development

For local development I have a task setup in a [Cake build](http://cakebuild.net/) for building and running the Pretzel tool. This wouldn't give too much benefit over just command lining the 2 commands needed.
Which commands? Well Pretzel gives us a few. The 2 important ones for us though are:
`pretzel.exe bake` - this will build our static website and since we provided no output folder it puts it in a folder *_sites/*. This is important to remember later  
`pretzel.exe taste --port 5001` - this will serve up the site and launch the site in the browser so you can admire your work

Why do I put these 2 simple commands in a build script? Well I have a transformation against the *_config.yml* that will swap out my domain name and *localhost:5001* depending on whether I am building for Debug or Release. It always use localhost when I am tasting since I don't use pretzel to serve the files.

If you are following along converting your own blog then and have not used Cake don't worry, it is super simple.

1. Install the Powershell build script: `Invoke-WebRequest http://cakebuild.net/download/bootstrapper/windows -OutFile pretzel.ps1`
2. This Powershell creates a ps1 fiel for *build.ps1* usually but we specified *pretzel.ps1* so on line 43 change *build.cake* to *pretzel.cake*
3. Create a file called *pretzel.cake* that looks like this:

> Updated: 2017-03-19 with new *Pretzel.exe* install path

<script src="https://gist.github.com/dburriss/c7871549c2788c0dca507a2d24c683ed.js"></script>

With this setup we can build using `.\pretzel.ps1` and preview locally with `.\pretzel.ps1 -target Taste`

> If you want to check-in what you have so far delete the *_sites/* folder before adding the file to source control on the branch *source*.

## Single repository

This one was a bit of a head-scratcher for me but then I remembered Github submodules. These allow you to map a folder in your repository to another repository. What I thought I would try was create an orphaned branch in my blog repository that contains the pretzel source and link the *_sites/* folder to the **master** branch which is where Github pages expects the static contents if you are not using Jekyll.

### Some quick housekeeping

If you have run the Pretzel build but have not added anything to the Github repository (even locally) then just delete the *_sites/* folder before continuing.  
If you have checked in the *_sites/* folder run the following git command to remove it.

`git rm -r _sites`  
`git commit -m "Remove _sites (preparing for submodule)"`

> you might need to remove from the index as well with `git rm -r --cached _sites`

### Creating the submodule

Next we are going to create the submodule that links back to the **master** branch where the static content is expected.

> Note that the following command uses https and not git protocol. This is important and you will get an error later in the CD process if you use git protocol.

`git submodule add -b master https://github.com/dburriss/dburriss.github.io.git _site`  
`git commit -m "_sites submodule"`

## Continuous Delivery

I use AppVeyor to pickup changes to the **source** branch. It uses Choclatey to install Pretzel. It then uses Pretzel to generate the static site into *_sites/* folder.  
The *_sites/* folder you will remember is actually a submodule linked back to the **master** branch of the same repository. We will push the generated changes to **master**, thus updating the blog with the latest content.

Place the following *appveyor.yml* file in the root of your **source** branch.   
The only thing you will need to change in the *appveyor.yml* is the url for your repository and the access token.

You can get an access token in Github by: 

### Github token

1. Profile pic dropdown top right
2. Settings
3. *Personal access tokens* at the bottom of the left menu

See [here](https://help.github.com/articles/creating-an-access-token-for-command-line-use/) for detailed instructions.

### Encrypt the token

1. Next in AppVeyor click on the dropdown on your username on the top right
2. Click Encrypt data
3. Paste the Github token in and press Encrypt
4. Copy the result into the *appveyor.yml* on line 7

<script src="https://gist.github.com/dburriss/66b4809c5e534481bdc4426c1d430765.js"></script>

## Conclusion

And there we have it! We can commit to **source** and the generated changes are committed to **master**.  
Feel free to copy my blog at https://github.com/dburriss/dburriss.github.io

Please leave a comment if you found this useful or have any improvements.

