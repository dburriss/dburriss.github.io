---
layout: post
title: "How to F# - Part 2"
subtitle: "Putting the function in Functional Programming"
description: "In part 2 of this series we will look at what gives functional programming its name, functions"
permalink: how-to-fsharp-pt-2
author: "Devon Burriss"
category: Software Development
tags: [Functional,F#,.NET]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/hole-bg.jpg"
social-img: "img/posts/2017/apple.jpg"
published: true
---
In the [previous post](/how-to-fsharp-pt-1) we looked at assigning values and the different types that those values could be. In this second installment we will be looking at functional programmings namesake: *functions*.

## Introduction

Functional programming as a paradigm is quite a hard thing to pin down, just like other paradigms. In object-oriented programming the one thing that really isn't up for debate though is that the general idea is that we have an object (whatever that may mean to you) and we represent data and behavior in these objects. In functional programming then it will come as no surprise that *functions* are first class citizens and that we accomplish our goals by transforming data using these functions.

What is a function though?

## A brief reminder about mathematics

Do not worry, I will not be going into deep mathematics theory here. I instead want to remind you of some mathematics you probably touched on in school just to show you that this isn't necessarily something completely new to you. Secondly, it will show that functional programming has roots that go far deeper than computer programming. Do not worry though if you didn't like this stuff at school, I promise this is way more `fun`.

In mathematical terms a function is a process that associates each element *x* of a set **X** to another value *y* which is of set **Y**. Let us call this process *f*. Then we have an expression:

```fsharp
y = f(x)
```

This is usually read as "let *y* equal *f* of *x*".

The set **X** of possible values of *x* is known as the **domain**. The set **Y** of possible outputs *y* is known as the **codomain**. To label the parts of the expression, *x* is the **argument** and the value of the function is the output.

So how would we accurately define a specific function?

> let *f*: R &#8594; R be the function defined by the equation *f(x) = x<sup>2</sup>*, valid for all real values of x

Notice how we have the `Domain -> Codomain` defined using `->`. We will come back to this a little later.

One last thing. Remember common functions like `sin` and `cos`? It is common to write them as `cos x`, without the the brackets as long as this does not lead to any ambiguity in the meaning. So now that we have had a little mathematics refresher, let us see if this brings us any insight into F# functions.

Mathematics done!

## Defining functions in F#

In the [previous post we looked at assigning values](/how-to-fsharp-pt-1). F# being a functional-first language means that we can treat functions like any other value.

```fsharp
// int -> int
let f x = x*x
let y = f 3 //val y : int = 9
```

So above we define function *f* that takes argument *x*. Then we pass `3` as the argument and assign that to value `y`.

Another way to define functions in F# is to use the `fun` keyword. Let us define the same function again, this time as function *g*:

```fsharp
// int -> int
let g = fun x -> x*x
let z = g 3 //val z : int = 9
```

This way makes it way more explicit that `g` is simply just another value that is assigned to. Note that this way of defining functions using `fun` is common when using functions once off inline, say for filtering a collection. We will see this in more detail in a later post when we dive deeper into collections.

## Understand functions

### Signatures

In the above code I put the signature of the function in a comment above it. The signature describes what types a function take in and what it returns. So for our function above we have `int -> int`. This means our function takes a single `int` as an argument and then returns an `int`.

A function always has an input and an output. In F# (and all programming languages I know of) a function can have multiple arguments. Say for example we had a `writeToFile` function that took a `boolean` for whether to overwrite the file if it exists, and a `string` with the content of the file. The signature for `writeToFile` would then be `bool -> string -> unit`. Now what is this `unit` type? It was mentioned in the previous post as the type that represents nothing. As already mentioned, functions must always have an input and an output, so if a function has no meaningful value to return, we return `unit`.

Do you see a similarity here? Types define the possible values that are possible. So for a signature `int -> int`, our **domain** is all possible numbers allowed by the type `int` numbers and our **codomain** is also all possible `int` values. Pretty cool right?

### Inference

In the previous function definitions you may have noticed we defined no types but the F# compiler inferred that the type of *x* was `int`. This is because we used the multiplication `(*)` operator on it. Most of the time the compiler does a pretty good job of working out the type. This keeps our code clean from boilerplate cruft. To be sure though, if you are used to a language like Java or C#, this will take a bit of getting used to. My tip is to pay close attention to the signatures. Any IDE will display this all the time or at least on mouse over.

If you prefer to be explicit or in those cases where the compiler needs some help to determine the type, you can easily define the types explicitly.

```fsharp
let f (x:int) = x*x         // define argument type
let f x : int = x*x         // define only return type
let f (x:int) : int = x*x   // define argument type and return type
```

The argument type is specified with `(x:int)`. The parenthesis are needed to disambiguate the argument type from the return type.

Just a quick style note. Mostly in F# code the types are left off unless needed.

I wanted to highlight another way of defining functions, and that is by defining a type signature.

```fsharp
type Unary: int -> int
let increment : Unary = fun x = x + 1
let decrement : Unary = fun x = x - 1
```

> A unary function is one that takes only one argument

So we define a **Unary** function as one that take in a single number and returns a number, and then we have multiple implementations of that type.

## Conclusion

In this post we introduced some of the core ideas behind functions. We learned how to define them and how to read a functions signature. We also touched on what the compiler can do for you by inferring the types, and how you can be explicit about the types.

In the next post we will dive deeper into [Working with Functions and getting them to work for you](/how-to-fsharp-pt-3).

## Resources

1. [Mathematics functions](https://en.wikipedia.org/wiki/Function_(mathematics))
1. [Programming paradigms](https://en.wikipedia.org/wiki/Programming_paradigm)
1. [Function signature](https://fsharpforfunandprofit.com/posts/function-signatures/)