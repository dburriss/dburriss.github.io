---
layout: post
title: "How to F# - Part 8"
subtitle: "Handling Errors Elegantly"
description: "Elegant exception handling in F#"
permalink: how-to-fsharp-pt-8
author: "Devon Burriss"
category: Software Development
tags: [Functional,F#,.NET]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2018/broken-500.jpg"
published: true
---
Even with all the pure functions we could ask for, eventually our applications are going to have to interact with the unpredictable outside world. Also, sometimes we just mess up. In this post we look at ways of dealing with errors in our applications.
<!--more-->

## Throwing our toys out the pram

In South Africa we say "Throwing your toys out the cot" but it is the same. When a child is upset, they tend to throw whatever they have in hand to express their distress. When you cannot communicate your intent in another way, this is how you get your parents' attention.

With that backdrop lets introduce `Exception`s. When an error occurs, the normal execution of the application stops and an error is raised as an object that contains information about the error that occurred. Exceptions can happen for example when reading from a file that is not where you expect it to be.

You can also raise exceptions yourself.

```fsharp
open System
// int -> int
let doublePositiveNumber x =
    if(x <0) then raise (new ArgumentException("Argument must be positive number"))
    else x*2

let y = doublePositiveNumber 2 // val y : int = 4
let z = doublePositiveNumber -1 // ERROR: System.ArgumentException: Argument must be positive number
```

Where `ArgumentException` (which is in the `System` namespace) is a type that inherits from `SystemException` which inherits from `Exception`. We haven't covered object-oriented topics in this series but basically that means that `ArgumentException` inherits features from `SystemException` which inherits from `Exception`. All errors that occur during application execution inherit from `Exception`. We will see an implication of this later when we explore ways of handling exceptions.

F# also provides a really easy way to raise an exception with a string message using `failwith`.

```fsharp
let doublePositiveNumber x =
    if(x <0) then failwith "Argument must be positive number"
    else x*2
```

## Custom exceptions

In F# defining custom exceptions is simple (especially compared to C#). Lets define a custom exception of type `MustBePositiveException` that takes a tuple of type `string * int`.

```fsharp
exception MustBePositiveException of string * int
let doublePositiveNumber x =
    if(x <0) then raise (MustBePositiveException("Argument must be positive number",x))
    else x*2
```

We will see soon how we can handle exceptions that occur.

## Handling exceptions

The semantics of handling exceptions is that we try do something with the possibility of one or more exceptions occurring. Lets look at an example.

```fsharp
open System
let z = try
            doublePositiveNumber -1
        with
        | :? Exception as ex -> printfn "ERROR: %s" ex.Message; 0 // don't do this
```

We `try` execute `doublePositiveNumber` and when it fails it falls though to the `with` part of the expression. Here we pattern match on the type using `:? Exception` and return `0` after printing the exception `Message`.

So we come to out first tip on exception handling.

> TIP 1: Only handle exceptions you are expecting. Let the exceptional cases bubble up.

What does this mean in practice? It means you should be more precise than handling `Exception`. Usually we want to do something drastic (like crash the application or cancel processing that HTTP request) if something happened that we did not cater for at all.

Remember that `Exception` is the type that just about any exception will inherit from, so by adding that as the type to handle, we effectively catch EVERY exception.

Lets see how we can 

```fsharp
// int
let z = try
            doublePositiveNumber -1
        with
        | MustBePositiveException(msg,nr) -> printfn "ERROR with number %i: %s" nr msg; 0
        | NumberTooLarge(msg,nr) -> printfn "ERROR with number %i: %s" nr msg; Int32.MaxValue
```

> TIP 2: If you can do something meaningful when an error occurs handle it as close to the exception source as possible.

So we are now being more precise about handling `MustBePositiveException`, which is better.  
NOTE: If we were raising an error using `failwith` we would handle with `| Failure(msg) -> printfn "%s" msg`.

## Handling expected exceptions

So in the previous example we were catching the `MustBePositiveException` exception and after printing returning `0`. Is this really a good default behavior? Maybe `-1`? This is hardly elegant or intent revealing. F# provides a functional solution to this problem in the form of `Result`. `Result` is similar to `Option` and `List` in that it provides an abstraction for dealing with a problem that takes a specific pattern. The result of a function call that can fail is either success, or a failure in some way. Lets change our calling code to return this `Result` type.

```fsharp
// Result<int,exn>
let z = try
            Ok (doublePositiveNumber -1)
        with
        | MustBePositiveException(msg,nr) as ex -> Error(ex)
```

So we call `Ok` with the result if the call succeeds and `Error` if it throws an exception. Note the signature of the return type is `Result<int,exn>`. The first generic parameter is an `int` for the successful case and the second is of type `exn`, an F# exception. If we had instead just send back the exception message with `Error(msg)` the return type would have been `Result<in,string>`.

## Working with Result

Lets take a look at a complete example and step through it.

1. We define our function that throw an exception
2. We call the function within a `try` expression
3. We handle the `Result` with pattern matching

```fsharp
let doublePositiveNumber x =
    if(x <0) then failwith "Argument must be positive number"
    else x*2

let safeDoublePositiveNumber x = 
        try
            Ok (doublePositiveNumber x)
        with
        | Failure(msg) -> Error(msg)

let z = safeDoublePositiveNumber -1

match z with
| Ok i -> printfn "The answer is %i" i
| Error msg -> printfn "ERROR: %s" msg
```

> Output: ERROR: Argument must be positive number

This leads to our third tip.

> TIP 3: In the majority of cases you cannot do anything about the exception at the source. Return `Result` for any expected exceptions and let the calling code decide what to do.

We could of course remove the need for `safeDoublePositiveNumber` by never throwing the exception in the first place.

```fsharp
let doublePositiveNumber x =
    if(x < 0) then Error "Argument must be positive number"
    else Ok (x*2)
```

Our final tip.

> TIP 4: Rather than raising exceptions for non-exceptional cases, instead just return `Result`.

## Conclusion

This was a brief introduction to exception handling. There are still more concepts to learn here so I do encourage you to go through the links in the resources section if you would like to learn more. You might want to look into `finally`, which allows execution of code regardless of the `try` succeeding or not.

Once you are comfortable with the concepts here I also suggest looking at the Railway oriented programming link in the resources.

To review the tips:

1. Only handle exceptions you are expecting. Let the exceptional cases bubble up.
1. If you can do something meaningful when an error occurs handle it as close to the exception source as possible.
1. In the majority of cases you cannot do anything about the exception at the source. Return `Result` for any expected exceptions and let the calling code decide what to do.
1. Rather than raising exceptions for non-exceptional cases, instead just return `Result`.

So don't be a child. Communicate your errors back rather than throwing your exceptions out the functions (that metaphor aged badly).

Next in the series we will be looking at a common occurrence in software development. Working with a database.

## Resources

1. [Throwing your toys out the pram](https://en.wikipedia.org/wiki/Wikipedia:Don%27t_throw_your_toys_out_of_the_pram)
1. [Inheritance](https://en.wikipedia.org/wiki/Inheritance_(object-oriented_programming))
1. [Microsoft docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/exception-handling/)
1. [Further reading for fun and profit](https://fsharpforfunandprofit.com/posts/exceptions/)
1. [Railway oriented programming](https://fsharpforfunandprofit.com/rop/)

## Credits

1. Social image by [Chuttersnap](https://unsplash.com/@chuttersnap)