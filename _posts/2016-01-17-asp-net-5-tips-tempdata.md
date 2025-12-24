---
layout: post
title: "ASP.NET 5 Tips: TempData"
subtitle: "Just hold this."
author: "Devon Burriss"
category: Software Development
tags: [ASP.NET 5, MVC 6, DNX]
comments: true
permalink: asp-net-5-tips-tempdata
excerpt_separator: <!--more-->
published: true
topics: []
keywords: [Software Development, ASP.NET 5, MVC 6, DNX]
---
> NOTE: Handling TempData and Session is made easy with extension methods in the [BetterSession](https://www.nuget.org/packages/BetterSession.AspNet.Mvc/) Nuget package.

ASPNET 5 is designed to be configurable. It starts out with almost nothing and you choose what you need. In previous versions of MVC we got TempData out the box. Not so with the new iteration.

![bridge cables](/img/posts/2016/footprint-resized.jpg)

<!--more-->

So to enable TempData for MVC you need sessions.
In **project.json** add the following lines to *dependencies*

{% highlight csharp %}
"Microsoft.AspNet.Session": "1.0.0-*",
"Microsoft.Extensions.Caching.Memory": "1.0.0-*"
{% endhighlight %}

In **Startup.cs** the configuration of your services will need the following:

{% highlight csharp %}
public void ConfigureServices(IServiceCollection services)
{
  services.AddCaching();
  //this is the NB line for this post
  services.AddSession(o =>
  {
  	o.IdleTimeout = TimeSpan.FromSeconds(3600);
  });
  services.AddMvc();
}
{% endhighlight %}

While the app builder configuration will be something like so:

{% highlight csharp %}
public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
{
  loggerFactory.AddConsole(Configuration.GetSection("Logging"));
  loggerFactory.AddDebug();
  //this is the NB line for this post
  app.UseSession();
  app.UseIISPlatformHandler();
  app.UseStaticFiles();
  app.UseMvc();
}
{% endhighlight %}

Then accessing TempData is done through the dependency injection/service locator:

{% highlight csharp %}
public class TempController : Controller
{
  private const string key = "name";
  private readonly ITempDataDictionary _tempData;

  public TempController(ITempDataDictionary tempData)
  {
  	this._tempData = tempData;
  }

  public IActionResult Index()
  {
    _tempData[key] = "Devon";
    return RedirectToAction("Carry");
  }

  public IActionResult Carry()
  {
  	return View("Index", _tempData[key]);
  }
}
{% endhighlight %}

OR

{% highlight csharp %}
var tempData = HttpContext.RequestServices.GetRequiredService<ITempDataDictionary>();
{% endhighlight %}
    
>NOTE 1: When using ITempDataDictionary in a custom **ActionResult** I needed to mark the class with **IKeepTempDataResult** for it to work.

>NOTE 2: I am not sure if this is going to change but currently the implementation for ITempDataDictionary only accepts primitive values (and string). I got around this by serializing to and from json. If you want to do this, you might find these extension methods useful.

{% highlight csharp %}
public static void SetAsJson<T>(this ITempDataDictionary tempData, string key, T data)
{
  var sData = JsonConvert.SerializeObject(data);
  tempData[key] = sData;
}

public static T GetFromJson<T>(this ITempDataDictionary tempData, string key)
{
  if(tempData.ContainsKey(key))
  {
  	var v = tempData[key];

    if(v is T)
    {
    	return (T)v;
    }

    if(v is string && typeof(T) != typeof(string))
    {
      var obj = JsonConvert.DeserializeObject<T>((string)v);
      return obj;
    }
  }
  return default(T);
}
{% endhighlight %}
    
So hope you and future me finds this post useful. I am going to try blog little things like this as I work more with ASP.NET 5. Please let me know in the comments below if you did find it useful or if I missed anything. Also let me know if there are other topics you want me to cover.

