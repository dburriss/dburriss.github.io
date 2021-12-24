---
layout: post
title: "A simple functional architecture"
subtitle: "How do I build a production application in a functional language?"
description: "If we apply the ideas of FP can we land at an architecture that rises out of the core ideas of high-order functions and pure functions?"
permalink: functional-programming-architecture-part-1
author: "Devon Burriss"
category: Software Development
tags: [Architecture,F#,Functional,Programming]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2019/target-500.jpg"
published: true
---

The number one question I get after discussing the [benefits of functional programming](/what-is-fp) (FP) with a developer who is not familiar with FP is, "Ok, that makes sense but how do I actually build a large application out of functions?" In this post I want to look at what a simple functional architecture that could serve as a starting point. 
<!--more-->
I will not be talking about Functional Reactive Programming (FRP) or Functional Relational Programming (also FRP) in this post. These are far more opinionated architectures, trying to achieve specific goals. Instead, I will describe a simple architecture that builds on the core ideas of functional programming covered in a [previous post](/what-is-fp). Let's refresh those here quickly:

 1. The language it is written in should support higher-order functions
 2. More complex code should be built from composing simpler functions together
 3. The programmer should follow the discipline of keeping functions pure as much as possible and push impure functions to the boundaries of the application

Pure functions and high-order functions will be the main ideas at play here, so if those are not familiar terms, go read [this](/what-is-fp) first.

The "architecture" is actually embarrassingly simple. We adopt the pattern of wrapping our features in **usecases**. This is the name I prefer but I have seen them referred to as **feature** or even **service** (I dislike this as it is so overloaded already).
A **usecase** is called by the host, where the host is typically a web application or a console application (I don't have much experience with desktop or mobile but I don't see why it would differ). The host is also responsible for providing production implementations of impure functions to the **usecase**.

![Functional arch diagram](/img/posts/2021/fp-arch-1.png)

At this point you might be saying, "Wait, isn't this just Hexagonal/Onion/Clean architecture?". Yes. Maybe it is my background (OOP)? Maybe there are only so many ways to skin a cat? There are more similarities than there are differences. 

The important part is the design inside of a **usecase**. A stark difference between OOP and FP seems to be the *separation of behaviour and data*. A revelation for me while learning FP was this focus on behaviour. All my professional career I had been modelling data and relationships, and trying to overlay behaviour over this in ways that allowed me to apply business requirements and still felt as if I was working with models of things in the real world. This focus on "things" means when it comes to implementing actual behaviour we end up with Repositories, Factories, Services, Managers, and ManagerMangers. The worst part of all of this for me was that the *behaviour*, which is where the actual value is and the thing that is important to understand, is spread across the codebase.

![scattered logic](/img/posts/2018/deeply-nested-dep.jpg)

> Sprinkling important application logic throughout an object graph makes it difficult to reason about. From post [Managing code complexity](/managing-code-complexity).

Maybe you have worked on a codebase where dependency injection has run wild and each step is called on an object that was injected into the object executing the current step.

The first improvement for me, that I think was really sparked by more functional thinking, was to make sure I had an entry point that described in clear steps how a **usecase** is implemented. Functional programming encourages pipelines of behaviour that inspect data, make a decision, and change data based on this. If you think about it, this ETL process is the core of what the programs we write do. Now this isn't yet functional thinking. I have started applying this to codebases regardless of the language of whether it is functional or not.

![usecase](/img/posts/2018/use-case.jpg)

> Usecase as the entrypoint into you domain. From post [Managing code complexity](/managing-code-complexity).

In using these top level **usecases** I had gained the benefit of clarity of what a usecase entails, as well as an easy entry point to dive into a specific step. In C# codebases I was using a fluent method chaining, making it even *feel* more functional due to the style. I was even going to the inconvenient effort of making my classes immutable. 
What I did not yet have was the benefits of functional programming. Even though there was a nice clean entry point and a descriptive flow, reasoning and testability where not much improved.  Where the functional programming part comes in is in trying to maximize how much of a **usecase** is [pure](/what-is-fp). The problem was I was mixing chained calls in a "functional style" without making much distinction between method calls that were pure and those that were not.

![push IO dependencies to the boundary](/img/posts/2018/dependencies-on-boundary.jpg)

> Impure operations should be pushed to the boundary. From post [Managing code complexity](/managing-code-complexity).

## Example

Let's look at a small example. Unfortunately, architecture only starts becoming important once size and complexity scales up but then examples can become unwieldy. I am keeping things simple here to illustrate the moving parts. Module names like `DataAccess` and `Usecase` a bit on the nose and in bigger applications would not be a good way to organize functionality.

Imagine a little CLI based CRM system. Current functionality is to add a new customer and to change that customers email address. So we expect to have an implementation of a `changeEmail` usecase.

```fsharp
let changeEmail (readCustomer : ReadCustomer) (saveCustomer : SaveCustomer) (cmd : ChangeEmailCommand) : Result<((DomainEvent list) * Customer),string> =
    readCustomer cmd.CustomreId
    |> Result.map (Customer.updateEmail cmd.NewEmail)
    |> Result.bind saveCustomer
```

The host provides implementations for the `ReadCustomer` and `SaveCustomer` function types. You can see the setup of those below for the CLI tool. Although the details here are not important, something you may notice in the type signature is the events in the return type `DomainEvent list`. This is something a pattern I started using many years ago even before I started functional programming when I realized raising domain events the moment it happens and having in-proc handlers can lead to inconsistencies. A safer pattern is to collect domain events as you execute and [outbox](/reliability-with-intents) what you need to at the same time you persist your aggregates.

This **usecase** is testable, as the IO parts are passed into the function. You can imagine that as more things need to happen, they can just be appended to the steps in the **usecase**.

If you are not familiar with F#'s `Result` type check out [Railway oriented programming](https://fsharpforfunandprofit.com/rop/).

```fsharp
// composite root composes IO for app
let readCustomer = DataAccess.readCustomer // real impl would take some config
let saveCustomer = DataAccess.saveCustomer // real impl would take some config
let changeEmail = Usecase.changeEmail readCustomer saveCustomer
let newCustomer = Usecase.newCustomer saveCustomer

// Routing the parsed input
let handle command =
    match command with
    | Commands.NewCustomer cmd -> newCustomer cmd
    | Commands.ChangeEmail cmd -> changeEmail cmd
    
// Parse the input to a command
let cmd = Mapper.inputToCommand args
// Get result by routing the command to correct usecase
let result = cmd |> Result.bind handle

// handle result output to CLI
```

With our **usecase** available, we can parse input to a command that matches to a **usecase**, in this case the `ChangeEmailCommand`. This is not an article on F# modelling and frankly it's a bit rough here but the point is that the usecase just received the command and knows nothing of the host.

## Tips

1. Don't use a usecase in another usecase. Rather have functions that can be shared across different usecases easily.
2. If you need to participate or kickoff sagas, an event list is a useful pattern.
3. Keep things simple and refactor when things get more complex
4. Use specific types as much as possible and build up pipelines inside the usecase that operate on those types

## Conclusion

In this post we saw how some core ideas of functional programming like higher-order functions and pure functions come together in guiding us toward an architecture.