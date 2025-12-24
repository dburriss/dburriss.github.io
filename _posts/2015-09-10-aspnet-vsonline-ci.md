---
layout: post
title: "ASP.NET 5 CI from Git to Azure without Visual Studio"
subtitle: "Using Visual Studio Online Build Services"
author: "Devon Burriss"
category: DevOps
tags: [OSX, Powershell, Azure, ASP.NET 5, MVC 6, DNX, CI, Visual Studio Online, Git, VS Code]
comments: true
permalink: aspnet-vsonline-ci
excerpt_separator: <!--more-->
published: true
topics: [tooling-automation]
keywords: [DevOps, OSX, Powershell, Azure, ASP.NET 5, MVC 6, DNX, CI, Visual Studio Online, Git, VS Code]
---
> Using Visual Studio Online Build Services for a MSBuild/xproj free deployment.

So my laptop was in for repairs so I decided to dust off my old Macbook Pro. I upgraded to Yosemite, downloaded [VSCode](https://code.visualstudio.com/) and ran through the the [setup for DNX](http://docs.asp.net/en/latest/getting-started/installing-on-mac.html) on Mac. Very quickly I started to wonder about deploying to [Azure](http://azure.microsoft.com/en-us/get-started/).

<!--more-->

I had previously used the steps described [here](https://msdn.microsoft.com/Library/vs/alm/Build/azure/deploy-aspnet5) to deploy a Visual Studio 2015 ASP.NET 5 project from Git but that relied on an xproj file for publishing.

The other option is publishing to Azure via source control as described [here](https://azure.microsoft.com/en-us/documentation/articles/web-sites-publish-source-control/).

I wanted something similar to the 1st option but for a solution created in VSCode and the aspnet [Yeoman](http://yeoman.io/) [generator](https://www.npmjs.com/package/generator-aspnet) though so what follows is what I have come up with so far.

*NOTE: The project structure could use some work but the scripts work.*

![guy on mac](/img/posts/2015/guy-on-mac_800.jpg)

### Step 1: Project Setup

The publish script uses the `global.json` file to determine the version and runtime. In the root is also `Publish.ps1` and `Upload.ps1` powershell scripts.
[Example](https://github.com/dburriss/vsfree-azure-deploy/tree/master/example)

#### Global

<script src="https://gist.github.com/dburriss/155c693de8f534bd1536.js"></script>
Setup the `global.json` file with properties needed for the publish.

#### Publish script

<script src="https://gist.github.com/dburriss/ea01dad652e00b480a7a.js"></script>

This script does a couple things along the way to publishing a folder for deployment.

1. Bootstraps DNVM into the Powershell session
2. Installs DNX on the build host
3. Restores the packages for the project using `dnu restore`
4. Packages the project using `dnu package`
5. Copies the runtime foler into the package (I think dnu restore is supposed to do this but at time of writing it was not)
6. Sets the **web.config** DNX version and runtime

#### Upload Script

This is a script found here [davideicardi/kuduSiteUpload.ps1 ](https://gist.github.com/davideicardi/a8247230515177901e57) which worked like a charm.
**UPDATE:** *I changed this script to stop the website before upload and start it again after as deployment was failing regularly with a 500 server error. My guess is locked files.*
<script src="https://gist.github.com/dburriss/af2e1593543b36b1ee23.js"></script>

#### VSOnline Build Setup

##### Step 1: Publish

![Build step 1 - Publish](/img/posts/2015/Build1.png)
Firstly we add a PowerShell script and point the script at our publish script:

* Script fielname: site/Publish.ps1
* Arguments: -sourceDir $(Build.SourcesDirectory)\pub

##### Step 2: Upload

![Build step 1 - Upload](/img/posts/2015/Build2.png)
Next we setup the upload script by creating an **Azure PowerShell** script:

* Azure Subscription: If you do not have one setup click Manage to do so
* Script Path: site/Upload.ps1
* Arguments: -websiteName *MyWebSite* -sourceDir $(Build.SourcesDirectory)\pub -destinationPath /site

Where *MyWebSite* is the name of the website in Azure.

Hit **Save** to save the build configuration.

#### Step 3: Setup CI (optional)

If you want CI you can go to the **Triggers** tab and set a build to trigger on commit to a branch.

* Select **CI**.
* Select **Batch changes**
* I filtered on **master** branch. Choose whatever is applicable.

Hit the **Save** button.

#### Step 4: Test your Build

Now you can either hit **Queue build...** or if you setup CI do a push to the trigger enabled branch. Note that the triggered build can sometimes take a few minutes to be queued and takes almost 5 minutes to build and deploy even for a small test site.

### Conclusion

Thats it for deploying to Azure with a solution developed on OSX (or Linux). Just 2 scripts really.
I hope this helps someone and please leave a comment below if you have any questions or suggestions. Or just want to say it helped :)
