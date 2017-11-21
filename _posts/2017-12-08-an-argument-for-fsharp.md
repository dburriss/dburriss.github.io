---
layout: post
title: "How I got hooked on FSharp"
subtitle: "My path to embracing functional programming"
description: ""
permalink: how-i-got-hooked-on-fsharp
author: "Devon Burriss"
category: Programming
tags: [Functional]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/star-watch-bg.jpg"
social-img: "img/posts/2017/touched-by-god.jpg"
published: false
---
Have you ever thought you have the perfect tool for the job at work but it is not on the allowed list of languages or framework? At this stage you have a decision to make. Are you going to just move on and pick something that will be less resistance or are you going to do the work to drive some change? In this post I make my case for functional programming in enterprise development, specifically **fsharp** if your current team expertise is .NET. The same arguments could be leveled for JVM based languages like Scala if your experience is in Java.

## An argument for language diversity

Firstly I would like to make the case for why you should consider using different languages in your environment. Even if you don't buy that, I will make a case for at the very least hiring outside of the language expertise you need on the job.

### Slim pickings

Good developers are in short supply and the market is competitive. By opening up your hiring to other languages, or actually using multiple languages, you expend the pool of developers by a multiple of the number of languages you are willing to consider. This can be a huge advantage in the number of applicants you receive. Obviously sheer number of applicants is not the only concern and I will address this in a later point. The important point to buy in to here though is that a good developer in any language is a better pick that a poor or average developer in your language of choice. A language can be ramped up in fairly quickly. Experience and professionalism on the other hand is hard earned and hard to come by. Im my opinion the quality of a developer always trumps the language they use.

### Swag

Let's face it. Your reputation as a company influences who you attract. For professional, open-minded developers that are not fan boys, a company that is focused on hiring on quality and principles is far more appealing than a company that religiously hires on technical stack. Polyglot (fluent in multiple languages) is one of those buzzwords that started doing the rounds a while back in the programming space (in this case specific to programming languages). Being able to use it honestly in your recruitment is a real bonus.

### Skin the cat

Different experience and different language features allow for different ways of solving a problem. Often just having someone with a different background look at a problem allows them to solve the problem in a new (for the team) and elegant way. This can have huge benefits to the team and company as a whole.

### Mindset is key

Selecting for people who pick up new languages is selecting for people who enjoy learning new things on their own time. This is often the number one identifier I see in hiring between average developers and awesome developers. When those languages span different programming paradigms like imperative and functional, then you have someone who is really learning by pushing their own comfort boundaries to look for better solutions.

- Pool of developers to choose from - a really solid developer is more valuable than an average developer with many years of experience in a single language
- Polyglot is good for your hiring reputation
- Different experience allows for different ways of looking at a problem
- Those that know a functional language usually do it on their own time so you are selecting for a very specific type of developer

## An argument for functional programming

Minimizing language features that allow you to make mistakes providers productivity and fall in pit of success.

- `null` causes bugs
- Unintended state mutation causes bugs
- No state makes concurrency programming simple
- Purity means code is testable
- Throwing exceptions means execution paths are harder to reason about
- Functions always returning means intent is captured in the signature like void and exception throwing just dont do
- Letting developers learn another paradigm will make them better developers overall

## An argument for fsharp

- Is multiparadigm so can slowly ease OO developers in if you want
- Is .NET based so .NET developers already know the base library
- You can reuse existing investment in packages
- Can reference between C# and F# projects both ways
