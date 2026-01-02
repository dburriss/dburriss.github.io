---
layout: post
title: "An argument for functional programming"
subtitle: "Convincing your boss to let you use fsharp at work"
description: "Arguments in favour of multiple programming languages, functional languages to be more specific, and fsharp in particular"
permalink: argument-for-fp
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/stairwell-bg.jpg"
social-img: "img/posts/2017/apple.jpg"
published: true
topics: [platforms-runtime]
keywords: [Software Development, Functional, OOP, F#, FsAdvent]
---

---
Have you ever thought you have the perfect tool for the job at work but it is not on the allowed list of languages or frameworks? At this stage you have a decision to make. Are you going to just move on and pick something that will meet less resistance or are you going to do the work to drive some change? In this post I make my case for functional programming in enterprise development, specifically **F#** if your current team expertise is .NET. The same arguments could be leveled for JVM based languages like Scala if your experience is in Java.

> This post is part of [FsAdvent Calendar 2017](https://sergeytihon.com/2017/10/22/f-advent-calendar-in-english-2017/)

<!--more-->


## TL;DR

In this post I drill down through the different reasons why a business (applies to individual developers too) should consider broadening their language range in a carefully considered way. First I argue that being open to multiple languages can benefit your companies hiring as well as the experience pool. Secondly I argue that functional programming opens up new perspectives while increasing the correctness of your applications in less time. As a bonus functional programming filters even better in the hiring process for top developers. Lastly I make the case that if you already have .NET experience the F# is a natural choice for a functional language.

If this is all you are going to read I want to leave you with an excerpt from a study done over 728 projects on Github. I link to the full article at the end of the post.

> "The data indicates that functional languages are better than procedural languages; it suggests that disallowing implicit type conversion is better than allowing it; that static typing is better than dynamic; and that managed memory usage is better than unmanaged." - A Large-Scale Study of Programming Languages and Code Quality in GitHub

## An argument for language diversity

<img src="../img/posts/2017/scrolls.jpg" alt="Scrolls" class="img-rounded pull-left" width="290" style="margin-right: 1em;">
Firstly I would like to make the case for why you should consider using different languages in your environment. Even if you don't buy that, I will make a case for at the very least hiring outside of the language expertise you need on the job.

### Slim pickings

Good developers are in short supply and the market is competitive. By opening up your hiring to other languages, or actually using multiple languages, you **expand the pool of developers by a multiple of the number of languages you are willing to consider**. This can be a huge advantage in the number of applicants you receive. Obviously sheer number of applicants is not the only concern and I will address this in a later point. The important point to buy in to here though is that a good developer in any language is a better pick than a poor or average developer in your language of choice. Language specific skills can be ramped up fairly quickly. Experience and professionalism on the other hand is hard earned and hard to come by. In my opinion the quality of a developer always trumps the language they use.

### Swag

Let's face it. Your reputation as a company influences who you attract. For professional, open-minded developers that are not fan boys of a specific language, **a company that is focused on hiring on quality and principles is far more appealing than a company that religiously hires on technical stack**. **Polyglot** (fluent in multiple languages) is one of those **buzzwords** that started doing the rounds a while back in the programming space (in this case specific to programming languages). **Being able to use it honestly in your recruitment is a real bonus**.

### Skin the cat

Different experience and different language features allow for different ways of solving problems. Often just having **someone with a different background look at a problem allows them to come up with a solution in a new (for the team) and elegant way**. This can have huge benefits to the team and company as a whole.

### Mindset is key

At the rate that information based industries change it is impossible knowing everything. More important is that you can acquire new skills efficiently and effectively. Selecting for people who pick up new languages is **selecting for people who actively pursue skill acquisition**. This is often the number one identifier I see in hiring between average developers and awesome developers. When those languages span different programming paradigms like imperative and functional, then you have someone who is really pushing their comfort zones to find better solutions. That mindset is hard to teach and one you really want on your team. At the very least it is someone who is willing to pick up what needs to be done on the job.

## An argument for functional programming

<img src="../img/posts/2017/eye.jpg" alt="Eye" class="img-rounded pull-left" width="280" style="margin-right: 1em;">
When I was new to software development I was always looking for new and shiny ways to do things. Waiting for that new feature. Over the years I have come to appreciate a more minimal and opinionated approach. Some tools are great for edge-cases but are often not worth the hassle they cause when used liberally where they should not be used. Minimizing language features that allow you to make mistakes increases productivity and helps you fall into the pit of success. My path to functional programming was paved in development pain and failure. How so? When something seemed painful I would look for ways to close that path in general development so the mistake was not made by me, or any other future developer again. Functional programming increases the constraints in a good way.

### Choice of 2, take it or leave it

Most of the mainstream enterprise languages out there have the concept of 'null'. This has been described as the [billion dollar mistake](https://en.wikipedia.org/wiki/Tony_Hoare#Apologies_and_retractions). Functional programming has more **elegant ways of representing the absence of data** that encourages you to make unrepresentable states unrepresentable. This is of course not the sole domain of the functional paradigm (I have [written about it in the past for C#](/honest-return-types)) but null based exceptions are rare to find in functional languages and if found are usually because of interop concerns. Minimizing the chance of null removes a whole class of exceptions that can possibly occur.

### Who moved my cheese?

Another point were I experienced pain was with erratic or incorrect programs due to unintended state changes. Functional programming on the other hand pushes you toward immutability. A function has an input and an output and that output does not have a reference to the input. This makes code far more predictable. **Immutability removes a whole class of errors that can occur due to unintended side-effects**, which are often hard to find and fix.

### The I is an illusion

In the age of cloud computing, auto-scaling, and concurrency, **not having state means concurrency becomes almost as simple as concurrent** since there is no state to lock around. This makes functional programming great for scale as it keeps things simple for the developer. As a developer you don't need to be an expert in concurrency to get it right. Again, a whole host of concurrency bugs are not representable (in state).

### Purity matters

Functional programming values something called purity. This is basically the characteristic that you pass something into a function and get something out, and no state has been mutated inside. So for each input value you will always get the same output value. Valuing purity means code that is not pure is pushed to the boundaries of the application, which is good. **Purity ensures that the bulk of your codebase is easily testable**.

### The new goto

Since functional programming encourages purity, throwing exceptions is not something you regularly do. It only happens in exceptional cases. Functional languages make this less [clunky than doing it in an OO first language like C#](/better-error-handling). What this means for code is **there are no breaks in control flow so it is easier to reason about**. Easier to reason about means easier to maintain and less bugs.

### Signature move

I have written before about [honest arguments](/honest-arguments) and [honest return types](/honest-return-types) and it is something I have witnessed make a difference in code. **Not only is the code more descriptive but correctness is reinforced by the compiler**. Functional programming brings the signatures of functions front and center. Once again, more possible errors negated.

### Expanding horizons

I touched on this in the section on language diversity but encouraging developers to learn **a new paradigm equips them with more tools in the toolbox**. I am not talking about a new framework or pattern but a new perspective at looking at a problem. A new perspective may yield a better solution to a problem.

### Short and sweet

**Functional languages usually allow you to do more with less code**. This is because it is declarative rather than imperative. This means your code reads like a sentence telling you what it does rather than a list of commands telling you each and every task to do.

## An argument for F#

<img src="../img/posts/2017/fsharp512.png" alt="fsharp" class="img-rounded pull-left" width="280" style="margin-right: 1em;">
So hopefully by this point I have convinced you (or you have convinced your boss) that having multiple languages is good. Not only that but choosing a functional first language makes good sense. My final step will be to convince you that F# should be that language.

### No cold turkey necessary

Although F# is a functional first language, it is actually multi-paradigm. **F# supports both functional and object oriented paradigms. It has to since it interops easily with C#**. So technically developers could code in an OOP style while they learned the F# language. This is absolutely an option and a pretty low risk way of introducing F#. The down side will be you might not reap the majority of the benefits I have mentioned thus far.

### Protect the ecosystem

Part of what makes C# and .NET in general great is the tooling and libraries built up around it. **Runtimes, IDEs, BCL, and library packages, they are all still available to you in F#** since it is a .NET based language.

### Protect the investment

**Your existing investment in libraries and business logic can be re-used as is without a re-write**. You might want to write a small functional wrapper around them to make them fit in the new functional paradigm but that is a nice to have. This means your current code is re-usable and future code can still be written in whatever a team is comfortable in and still interop in the same solution.

### Leading the pack

F# has been ahead of the curve in the .NET ecosystem in a lot of ways. So many of the great language features since C#'s initial Java clone have been inspired by F#. Current **features like generics, `async`/`await`, auto-property initializers, exception filters, expression-bodied function members, and pattern-matching were all in F# first**(or [worked on by the creator of F#](https://blogs.msdn.microsoft.com/dsyme/2011/03/15/netc-generics-history-some-photos-from-feb-1999/)).

### Shoulders of giants

Although F# has been leading the charge with Open Source for longer than probably any other Microsoft endeavour, it still has the backing of Microsoft as well as an active OSS community. F# was released by Microsoft Research in 2005 and has been on Github since 2010. It is lead by the [FSharp Foundation](http://foundation.fsharp.org/) that is dedicated to advancing the language.

Then there is the actual OSS community. There are too many to name individually but some that you will either use or stand out because of their ambition are:

1. [Ionide](http://ionide.io/) - An IDE plugin for Visual Studio Code and Atom that has been ahead of Visual Studio in supporting F# features, especially with the new `netstandard` stuff
1. [F# Data](http://fsharp.github.io/FSharp.Data/) - is a useful library for working with data from varied sources
1. [Suave](https://suave.io/) - An ambitious and full-featured web library and server that provides a functional-first programming model for web development
1. [Giraffe](https://github.com/dustinmoris/Giraffe) - a micro web framework that wraps the Asp.Net Core functionality for a more functional-first programming model
1. [MBrace](mbrace.io) - provides a simple programming model that opens up cloud computing in a way that initially seems like magic

This is far from an exhaustive list. The point is there are mature and well supported projects out there because the F# community is dedicated and enthusiastic. The FsAdvent Calendar initiative is a great example of this.

## Caution

It would be remiss of me not to leave you with a few cautionary points.

### Learning curve

Functional programming, especially with non C like languages can be pretty mind bending when you first start. I wish I could find the quote but I think it was one of the JVM functional language designers (Scala or Clojure) who said something like "sacrificing future power and expressiveness for beginner ease of use is one of the worst traps language designers can fall into". I like the sentiment but in terms of language popularity it seems to have some unfortunate downsides. However, those who stick with it and start becoming fluent are usually die hard converts because they have realized the usefulness of the paradigm. On the other hand if most give up, the pool of developers will mostly consists of the smartest or most determined.

### Maturity of the team

Language diversity requires a high level of maturity in your development team. A team lacking in maturity is more likely to pick something based on what they feel like using rather than assessing fitness of the tool for the solution. Hiring in at least one for two experienced people to lead would probably be a good idea.

### Ramp up

Ramping up slowly and allowing more people in the organization to get experience on low risk projects could be a low risk way of introducing F#. [A developer could learn the syntax this way without taking the productivity hit of learning a new paradigm](https://youtu.be/qPlYbHKvk4g?t=376). Mark Seemann has talked about how he initially just did OOP with F# and slowly incorporated functional ideas. In Mark's case I think he was leaning toward functional concepts anyway. Without a push to do so a developer could remain a 100% OO programmer while using F#. Even worse, a developer doing this might then decide that F# provides no benefits. So a slow ramp up comes with it's own risks.

### Maturity of deployment

With a new language you might need new deployment pipelines so make sure you have this sorted on a technology you are familiar with before going crazy with choices.

### Pick smart

Although I argue for a polyglot environment I am not making the case for ALL languages being allowed. These projects still need to be supported by the organization. Pick a small set of languages after considering a few aspects of them:

1. Maturity of the language, ecosystem, and the community
1. Popularity of the language (no point jumping on a sinking ship)
1. Availability of developers
1. Expected salaries (you need to be competitive)

## Conclusion

So I covered reasons why you should consider more languages, why one of those should be functional, and hopefully convinced you to [give F# a try](http://fsharp.org/). This actually isn't an exhaustive list. Personally, I have found other reasons why learning F# has been great. Learning F# made it easier for me to jump into even more languages. Elm for instance was super low resistance. Also F# has a bunch of really cool features like Type Providers, Computation Expressions, and more that blow your mind when you come across them.

## Further Reading

1. [A Large-Scale Study of Programming Languages and Code Quality in GitHub](https://cacm.acm.org/magazines/2017/10/221326-a-large-scale-study-of-programming-languages-and-code-quality-in-github/fulltext)
1. [Comparing F# and C# with dependency networks](http://evelinag.com/blog/2014/06-09-comparing-dependency-networks/)
1. Mark Seemann has a brilliant posts on how a [language can reduce the potential for errors](http://blog.ploeh.dk/2015/04/13/less-is-more-language-features/)
1. Mark has an excellent talk on [falling into the pit of success](https://www.youtube.com/watch?v=US8QG9I1XW0) and another on [Dependency Rejection](https://www.youtube.com/watch?v=cxs7oLGrxQ4)
1. Scott Wlaschin has an excellent [series on low risk ways to start using F# at work](https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work/)

## Credits

1. Header photo by [Nicolas Hoizey](https://unsplash.com/@nhoizey)
1. Social photo by [Michał Grosicki](https://unsplash.com/@groosheck)
1. Scrolls photo by [Sindre Aalberg](https://unsplash.com/@sindreaalberg)
1. Eye photo by [Amanda Dalbjörn](https://unsplash.com/@amandadalbjorn)

