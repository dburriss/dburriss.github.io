---
layout: post
title: "ASP.NET 5 Tips: UrlHelper"
subtitle: "ASPNET Core TagHelpers."
author: "Devon Burriss"
category: Software Development
tags: [ASP.NET 5, MVC 6, DNX]
comments: true
permalink: asp-net-5-tips-urlhelper
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
published: true
---

> Note that this is specific to the upcoming RC 2 using the dotnet CLI. Currently in RC 1 this is not an issue.

So I was messing around with [David Fowl's repository](https://github.com/davidfowl/dotnetcli-aspnet5) that makes use of the new RC 2 bits that run on the new [dotnet CLI](https://github.com/dotnet/cli).

Everything was fine until I tried to create a TagHelper that makes use of *IUrlHelper*.
In RC 1  *IUrlHelper* is registered automatically with the DI system but apparently not in RC 2. After much searching I found the following [commit](https://github.com/aspnet/Mvc/commit/9fc3a800562c866850d7c795cf24db7fa0354af6) which explained the change.

<!--more-->

So what follows is how I got an *IUrlHelper* into my TagHelper.

It seems we should instead make use of *IUrlHelperFactory* to get an instance of *IUrlHelper*.

In **Startup.cs** service configuration I register *IActionContextAccessor* and *IUrlHelperFactory*:
{% highlight csharp %}
public void ConfigureServices(IServiceCollection services)
{
  services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
  services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
  services.AddMvc();
}
{% endhighlight %}

Then I inject *IUrlHelperFactory* into the TagHelper constructor and use the factory to create a new instance of a *IUrlHelper*:
{% highlight csharp %}
public class EmailTagHelper : TagHelper
{
  private readonly IUrlHelper _urlHelper;

  public EmailTagHelper(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
  {
  	_urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
  }
  
  //process override here
}
{% endhighlight %}

I am guessing that this article will only be useful next month when RC 2 hits but it was great to see what is coming. I am quite liking the new CLI and with a bit of digging I have managed to get most things working, so the team seems to be making great progress toward RC 2.
Please let me know below if you found this useful... or if things change :)