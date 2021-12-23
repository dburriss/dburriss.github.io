---
layout: post
title: "Functional Architecture - Part 1"
subtitle: "Defining the functional part in Functional Architecture"
description: "A look at the big ideas in functional programming like pure functions, higher-order functions, and composition. This will form the baseline for future architecture posts on the subject."
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

The number one question I get after discussing the benefits of functional programming (FP) with a developer who is not familiar with FP is, "Ok, that makes sense but how do I actually build a large application out of functions?" This will be the first in a series discussing basic functional architecture. This series will incrementally build from basic FP concepts to architecture.
<!--more-->

I have [made more general arguments for FP](https://devonburriss.me/argument-for-fp/) in other posts. In this post I want to go into some of the actual ideas of FP. If you are already an experienced functional programmer, then there won't likely be anything new here. If on the other hand you are new to FP then this can set the baseline for future posts.

It turns out, just like with OOP, to be pretty difficult to define exactly what makes up FP. Let's try though.

## Maximize use of pure functions

This one is fairly uncontroversial. If you have heard people talking about FP then you have likely heard about pure functions. 

At this point talk of **referential transparency** comes up. [Referential transparency seems to be a term borrowed from analytical philosophy](https://stackoverflow.com/a/9859966/2613363). If something is referentially transparent it means it's value is not dependent on some context like time. From a code perspective, this means that once something is assigned a value, that value does not change over the lifetime of the programs execution. Put more flippantly, "equals equals equals".

The characteristic people are more often seeking with pure functions is *side-effect free* functions. Side-effect free is much easier to understand than referential transparency. It means that nothing outside the scope of the function is mutated.

So for a function to be **pure** it needs to satisfy 2 criteria:

1. The function must be referentially transparent
2. The function must be side effect free

Note that I said *maximize* pure functions. We cannot build programs that interact with the outside world without having side effects. What we strive for in FP is increasing the amount of functions that are pure and pushing the side-effects to the boundary of our applications. We will dive into this more in part 2 of this series.

### Benefits

- Calls to a function are idempotent, so they can be repeated without fear of unexpected state updates
- If a function does not depend on the output of another function, the order does not matter
- Since pure functions depend only on their input, they can be called in parallel without fear of deadlock or data corruption
- Pure functions are easy to test because they depend on only the input and must have an output (to be useful)
- Since a pure function only depends on input, reasoning about it should be simpler
- If the value of a pure function is not used, it can be removed without altering the behaviour of a program

One last thing to point out before we move on from pure functions. Immutability comes along for the ride with pure functions, at least for where it really matters when programming with immutable values.

## Higher-order functions

A higher-order functions is a function that meets at least one of the following two criteria:

1. A function that takes another function as an input
2. A function that returns a function as its output

Although most modern languages support this now days, functional-first languages tend to make this feel a lot more natural to use.

```fsharp
let isEven x = (x % 2) = 0              // predicate function for determining an even number
let selectEven = List.filter isEven     // Use predicate to returns new function of that selects even numbers

let evenInts = selectEven [0..10]       // use the function
```

### Benefits

- Functions are values and so can be passed around
- Functions that take functions can far more flexible as behaviour can be decided by the caller
- When returning a function from another function it can be evaluated later (or not at all)

## Function composition

Function composition is the combination of simple functions into more complex ones. To compose functions the output of a function needs to be the input for the next function in the composition.

This is probably easiest explained with examples since you have probably used it in both school maths and programming.

Say we want to normalize some strings by trimming the whitespace off and making them lower-case.

```fsharp
let trim (s : string) = s.Trim()
let lower (s : string) = s.ToLowerInvariant()
let normalize (s : string) = lower(trim(s))
```

In a functional-first language like F# we can build this up in a way that structurally matches the order the functions are applied.

```fsharp
let normalize = trim >> lower
```

This creates the new function `normalize` out of the 2 functions `trim` and `lower`. Remember that the output type of `trim` needs to match the input type of `lower`. In this case they are both `string`.

### Benefits

- Functions are small testable units
- Small and generic functions enable reusability
- Build more and more complex functions out of simpler functions helps in building in small steps

## Conclusion

 In this post we looked at some of the core ideas of functional programming. These ideas all have a long tradition in mathematics but hopefully from the benefits listed you can see that they might have some real practical benefits if adopted into how you design and implement applications. Depending on what languages you have been exposed to, you may have expected other topics here like immutability and algebraic data types. These language constructs being built into the language can really help but I don't believe are necessary for programming in a functional way. In the next post we will look at these to see what benefits they bring.