---
layout: post
title: "Functional modeling"
subtitle: "The benefits of focusing on function rather than state"
description: "As OOP developers it is often far too easy for us to try create a model that captures every state. This is almost always a poor abstraction. What if instead of trying to model every possible state at rest we modeled the change of state over time? This article explores what that might look like."
permalink: functional-modeling
author: "Devon Burriss"
category: Modeling
tags: [DDD,OOP,Functional Modeling,Architecture, Modeling, Temporal Modeling]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "/img/posts/2018/functional-model-500.jpg"
published: true
---

In my [previous post](/functional-structural-impedance-mismatch) I introduced the idea of a structural model in the code that closely matches what a use-case should do functionally. Just as an ubiquitous language helps us tie concepts in our code, so a functional model helps us capture the functioning of a use-case. In this post I will go into this idea in a little more detail, giving some tips on how to get started.
<!--more-->

> I am not explicitly talking about functional programming in this article although any familiar with it will see it's influences. Even if you do not embrace FP, the concepts from it that I mention here can be applied to the benefit of your codebase.

As an example we are looking at a real life project where we are allocating monetary amounts to sales or purchases based on agreements we have with suppliers.
Let's start with a deeper look at the example that was used in the previous post:

![Allocation functional structure](/img/posts/2018/functional-structure.jpg)

This was a very simplified view of the components involved for calculating the amounts to be allocated to an agreement due to sales or inbound orders. It also still shows the structural components involved. As an exercise I mapped out the calls that are made while completing a use-cae. This style is borrowed from Simon Brown's [C4 Model](https://c4model.com) but with a focus on function rather than structure.

![Allocation functional structure](/img/posts/2018/functional-model.jpg)

And here is the top the entry point for this use-case.

```csharp
return await TryGetAgreement(agreementId)
            .Bind(agreement => 
                    _agreementSupportedValidator.IsSupported(agreement).ToAsync())
            .Bind(supportedAgreement => 
                    TryCreateAgreementWithHistory(supportedAgreement).ToAsync())
            .Bind(agreementWithHistory => 
                    AllocationPathfinder(agreementWithHistory).ToAsync())
            .Map(allocationResult => 
                    AllocationsFilter.Filter(allocationResult))
            .Bind(newAllocationResult => 
                    TryStoreAllocations(newAllocationResult).ToAsync())
            .Try();
```

Although I am the first to admit that this style is not too pretty in C#, once you get used to the [Functional Language Extensions](https://github.com/louthy/language-ext) like `Bind`, `Map`, and `Try`, it really reads like what it does at this level of abstraction.

So why would we want to write code like this?

## High level description of process

When exploring a codebase it is always nice to find the entry point to a feature that describes what happens at a single abstraction level. Too often each step is wrapped in some infected factory or manager that conveys very little intent and quickly become a class quagmire.

## Maps well to Event Storming

[Event Storming](https://www.eventstorming.com/) is becoming increasingly more popular as a means of learning a domain. By it's very nature Event Storming is a time based rather than a state based model and it can be quite difficult 

![Event Storming legend](/img/posts/2018/es-legend.jpg)

## Focus on doing

Following on from the previous point but true of every level of the codebase, bringing the functional process forward into plain view is simpler when focusing on what is being done, rather than on the doer. we now move from modeling stateful doers to modeling the state between transformations over time. This is way more inline with how the business thinks in terms of getting work done.

## Testability (unit tests)

If you are able to constrain external IO to the beginning and end of your flows you will have simple input/output functions in between. This sort of code is a lot more testable than those that have many dependencies. You can now concentrate on just testing the output from a certain input without worrying about injecting mocked dependencies.

## Composability

Often in business we have branching flows. Too often this results in bad abstractions that try to handle every branch, even those not yet added by the business. A far more maintainable way to handle these is to reuse that which is common and compose it with specific implementations when things branch. This usually results in cleaner code that is far more future proof than using inheritance.

## Conclusion

In this post we went into a little more detail of what code may look like if we started modeling the flow of events throw time even within small the use-cases. We looked briefly at what this could look like and reasons it might be worth trying. The keys to implementing this well is to;

1. Chain steps at a single abstraction level that make sense, allowing developers to dive only to the depth needed to understand what is needed
1. Instead of trying to come up with an abstraction that captures every state, model the states between transitions
1. Capture the domain language in both the states and the functions that transition from state to state
1. Push dependencies to the outside to increase testability and how easy it is to reason about the system

If you are interested in really drilling into this and learn functional programming a highly recommend [Scott Wlaschin's Domain Modeling made Functional](https://pragprog.com/book/swdddf/domain-modeling-made-functional).

What do you think? If you have any ideas please leave a comment or reach out on Twitter [@DevonBurriss](https://twitter.com/DevonBurriss). You also may be interested in my [previous article on the differences between structural and functional modeling](/functional-structural-impedance-mismatch) and my [tips for managing code complexity](/managing-code-complexity).

## Resources

1. [Function model](https://en.wikipedia.org/wiki/Function_model)