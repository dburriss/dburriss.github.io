---
layout: post
title: "An argument for functional programming"
subtitle: "Convincing your boss to let you use fsharp at work"
description: ""
permalink: an-argument-for-fsharp
author: "Devon Burriss"
category: Programming
tags: [Functional, OOP]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/star-watch-bg.jpg"
social-img: "img/posts/2017/touched-by-god.jpg"
published: false
---
Have you ever thought you have the perfect tool for the job at work but it is not on the allowed list of languages or framework? At this stage you have a decision to make. Are you going to just move on and pick something that will be less resistance or are you going to do the work to drive some change? In this post I make my case for functional programming in enterprise development, specifically **fsharp** if your current team expertise is .NET. The same arguments could be leveled for JVM based languages like Scala if your experience is in Java.
<!--more-->
## An argument for language diversity

<img src="../img/posts/2017/scripts128.jpg" alt="Scrolls" class="img-rounded pull-left" width="128" style="margin-right: 1em;">
Firstly I would like to make the case for why you should consider using different languages in your environment. Even if you don't buy that, I will make a case for at the very least hiring outside of the language expertise you need on the job.

### Slim pickings

Good developers are in short supply and the market is competitive. By opening up your hiring to other languages, or actually using multiple languages, you **expand the pool of developers by a multiple of the number of languages you are willing to consider**. This can be a huge advantage in the number of applicants you receive. Obviously sheer number of applicants is not the only concern and I will address this in a later point. The important point to buy in to here though is that a good developer in any language is a better pick that a poor or average developer in your language of choice. A language can be ramped up in fairly quickly. Experience and professionalism on the other hand is hard earned and hard to come by. Im my opinion the quality of a developer always trumps the language they use.

### Swag

Let's face it. Your reputation as a company influences who you attract. For professional, open-minded developers that are not fan boys, **a company that is focused on hiring on quality and principles is far more appealing than a company that religiously hires on technical stack**. **Polyglot** (fluent in multiple languages) is one of those **buzzwords** that started doing the rounds a while back in the programming space (in this case specific to programming languages). **Being able to use it honestly in your recruitment is a real bonus**.

### Skin the cat

Different experience and different language features allow for different ways of solving a problem. Often just having **someone with a different background look at a problem allows them to solve the problem in a new (for the team) and elegant way**. This can have huge benefits to the team and company as a whole.

### Mindset is key

Selecting for people who pick up new languages is **selecting for people who enjoy learning new things on their own time**. This is often the number one identifier I see in hiring between average developers and awesome developers. When those languages span different programming paradigms like imperative and functional, then you have someone who is really learning by pushing their own comfort boundaries to look for better solutions. That mindset is hard to teach and one you really want on your team.

## An argument for functional programming

<img src="../img/posts/2017/eye128.jpg" alt="Eye" class="img-rounded pull-left" width="128" style="margin-right: 1em;">
When I was new to software development I was always looking for new and shiny ways to do things. Waiting for that new feature. Over the years I have come to appreciate a more minimal and opinionated approach. Some tools are great for edge-cases but are often not worth the hassle they cause when used liberally where they should not be used. Minimizing language features that allow you to make mistakes increases productivity and helps you fall into the pit of success. My path to functional programming was paved in development pain and failure. How so? When something seemed painful I would look for ways to close that path in general development so the mistake was not made by me, or any other future developer again. Functional programming increases the constraints in a good way.

### Choice of 2, take it or leave it

Most of the mainstream enterprise languages out there have the concept of 'null'. This has been described as the billion dollar mistake. Functional programming has more **elegant ways of representing the absence of data** that encourages you to make unrepresentable states unrepresentable. This is of course not the sole domain of the functional paradigm (I have [written about it in the past for C#](/honest-return-types)) but null based exceptions are rare to find in functional languages and if found are usually because of interop concerns. Minimizing the chance of null removes a whole class of exceptions that can possibly occur.

### Who moved my cheese?

Another point were I experienced pain was with erratic or incorrect programs due to unintended state changes. Functional programming on the other hand pushes you toward immutability. A function has an input and an output and that output does not have a reference to the input. This makes code far more predictable. **Immutability removes the a whole class of errors that can occur due to unintended side-effects**. And these are often hard to find and fix.

### The I is an illusion

In the age of cloud computing, auto-scaling, and concurrency, **not having state means concurrency becomes almost as simple as concurrent** since there is no state to lock around. This makes functional programming great for scale as it keeps things simple for the developer. As a developer you don't need to be an expert in concurrency to get it right. Again, a whole host of concurrency bugs are not representable (in state).

### Purity matters

Functional programming values something called purity. This is basically the characteristic that you pass something into a function and get something out, and no state has been mutated inside. So for each input value you will always get the same output value. Valuing purity means code that is not pure is pushed to the boundaries of the application, which is good. **Purity ensures that the bulk of your codebase is easily testable**.

### The new goto

Since functional programming encourages purity, throwing exceptions is not something you regularly do. It only happens in exceptional cases. Functional languages make this less [clunky than doing it in an OO first language like C#](/better-error-handling). What this means for code is **there are no breaks in control flow so it is easier to reason about**. Easier to reason about means easier to maintain and less bugs.

### Signature move

I have written before about [honest arguments](/honest-arguments) and [honest return types](/honest-return-types) and it something I have witnessed make a difference in code. **Not only is the code more descriptive but correctness is reinforced by the compiler**. Functional programming brings the signatures of functions front and center. Once again, more possible errors negated.

### Expanding horizons

I touched on this in the section on language diversity but encouraging developers to learn **a new paradigm equips them with more tools in the toolbox**. I am not talking about a new framework or pattern but a new perspective at looking at a problem. A new perspective may yield a better solution to a problem.

### Short and sweet

**Functional languages usually allow you to do more with less code**. This is because it is declarative rather than imperative. This means your code reads like a sentence telling you what it does rather than a list of commands telling you each and every task to do.

## An argument for fsharp

<img src="../img/posts/2017/fsharp128.png" alt="fsharp" class="img-rounded pull-left" width="128" style="margin-right: 1em;">
So hopefully by this point I have convinced you (or you have convinced your boss) that multiple languages is good. Not only that but choosing a functional first language makes good sense. My final step will be to convince you that F# should be that language.

### No cold turkey necessary

Although F# is a functional first language, it is actually multi-paradigm. **F# supports both functional and object oriented paradigms. It has to since it interops easily with C#**. So technically developers could code in an OOP style while they learned the F# language. This is absolutely an option and a pretty low risk way of introducing F#. The down side will be you might not reap the majority of the benefits I have mentioned thus far.

## Protect the ecosystem

Part of what makes C# and .NET in general great is the tooling and libraries built up around it. **Runtimes, IDEs, BCL, and library packages, they are all still available to you in F#** since it is a .NET based language.

### Protect the investment

**Your existing investment in libraries and business logic can be re-used as is without a re-write**. You might want to write a small functional wrapper around them to make them fit in the new functional paradigm but that is a nice to have. This means your current code is re-usable and future code can still be written in whatever a team is comfortable in and still interop in the same solution.

### Leading the pack

F# has been ahead of the curve in the .NET ecosystem in a lot of ways. So many of the great language features since C#'s initial Java clone have been inspired by F#. Current **features like generics, `async`/`await`, auto-property initializers, exception filters, expression-bodied function members, and pattern-matching were all in F# first**.

### Shoulders of giants

Although F# has been leading the charge with Open Source for longer than probably any other Microsoft endeavour, it still has the backing of Microsoft as well as an active OSS community. F# was released by Microsoft Research in 2005 and has been on Github since 2010. It is lead by the [FSharp Foundation](http://foundation.fsharp.org/) that is dedicated to advance the language.

* Is multiparadigm so can slowly ease OO developers in if you want
- Is .NET based so .NET developers already know the base library
- You can reuse existing investment in packages
- Can reference between C# and F# projects both ways
- Microsoft supporting it