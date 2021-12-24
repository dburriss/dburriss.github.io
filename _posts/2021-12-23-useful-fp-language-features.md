---
layout: post
title: "Useful FP language features"
subtitle: "Making functional programming more productive"
description: "A review of some language features like immutability and algebraic data types that work well in a functional style of programming."
permalink: useful-fp-language-features
author: "Devon Burriss"
category: Software Development
tags: [F#,Functional,Programming]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/superman-bg.jpg"
social-img: "img/posts/2018/hammer-500.jpg"
published: true
---
In a [previous post](/what-is-fp) we looked at the big ideas of functional programming. In this post we will look at some features that are often associated with functional programming but that I do not think are core to it.
<!--more-->
Some of these are conflated with functional programming but it turns out that the only language feature needed for functional programming is support for higher-order functions.

## Immutable data

To work with pure functions, you need to be careful not to change the underlying state of you application. This includes the input to your functions. It is useful if your language can enforce this.

I was presenting to a group of Javascript and C# developers a few weeks ago and I showed the following C# snippet of code.

```csharp
// what does this return?
var two = 1 + 1;
return two++;
```

Now maybe this is a bit unfair but I think it highlights the problem of reasoning about mutable state as statements are executed. When I polled the audience on this it seemed about a 50/50 split between answers of 2 and answers of 3. If anything, more answers of 3. If you are not sure, it turns out the number 2 is returned. Any subsequent references to `two` would reference the value 3.

Now granted, the `++` operator is not the most intuitive and you need to know the behaviour expected depending on what side of the variable it is place. It is useful in illustrating how state can change in ways we might not anticipate.

In the F# example below, you see that a value is immutable. Once it's value is set, it cannot be changed.

```fsharp
let two = 1 + 1
//let two = 3 // will not compile
//let two <- 3 // will not compile
```

Once you have immutable values, it is important to have an easy way to create new values from old ones. An often overlooked area here is having good tools for working with immutable collections.

```fsharp
let stock = [ ("chicken", 20);("grain", 50);("potatoes", 30) ] |> Map.ofList
// create a new map from an existing one
let newStock = stock |> Map.change "chicken" (fun vOpt -> vOpt |> Option.map (fun v -> v - 1))
```

Above we see that rather than changing the value in the map, a new map is returned with the changed value.

### Benefits

- Easier to reason about
- Fewer bugs due to unexpected state changes
- Easier parallel processing

## Algebraic data types

Algebraic data types are comprised of **product** types and **sum** types. 

Sidebar: I am not the person to be trying to explain Type Theory. I am not even sure if there exists a formal definition of class and how it relates to a type (in a language agnostic way). If you are an OO programmer think of a type as a concrete class. So `Nullable<T>` is a class, `Nullable<int>` is a type and `Nullable<decimal>` is another type. My current thinking of a class is as a parameterized factory for a type, if it is generic. If not they can be considered equivalent. Experts, let me know in the comments all the ways this is wrong :)

**Product types** are either records or tuples which in OO languages are common data structure types.

```fsharp
type IntAndBool = {
    I : uint
    B : bool
}

let p = { I = 0u ; B = true }

// range of possible values
printfn "product %i" (((UInt32.MaxValue |> int64) + 1L) * (2L)) // range of uint * range of bool
// product 8589934592
```

Giving as a total possible range of 8589934592 combinations, found by multiplying the possible number of states in each field.

So I bet you can guess where **sum types** get there name from now...

**Sum types** are known by many names and appear primarily in functional-first languages (tagged union, discriminated union, choice type, to name a few). The only OO leaning language I personally know that has something like **sum types** is TypeScript's Union types.

These types allow us to define types that can be something, or something else. An example will illustrate this best.

```fsharp
type IntOrBool = I of uint | B of bool

let s = B true

(((UInt32.MaxValue |> int64) + 1L) + 2L)
printfn "sum %i" (((UInt32.MaxValue |> int64) + 1L) + (2L)) // range of uint + range of bool
// sum 4294967298
```

An instance of `IntOrBool` can be either one type or the other. There is no need to constrain these to combining simple types though. We can model using more complex types.

```fsharp
type PostalCode = string
type Address = { 
    HouseNumber : int
    HouseNumberOpt : char option
    StreetName : string
    City : string
    PostalCode : PostalCode
}
type EmailAddress = string
type PhoneNumber = string
type ContactMethod = Email of EmailAddress | Post of Address | Phone of PhoneNumber
```

Here you see the `ContactMethod` type can be `EmailAddress` OR `Address` OR `PhoneNumber`. This gives a far more rich and intuitive way of modelling a domain.

A language that supports **sum types** typically provides elegant ways of dealing with 2 prickly issues in programming.  
Too often the absence of something is represented by `null`. " The billion dollar mistake yada yada...". 
In functional languages the approach is to use a sum type, usually called `Option` or `Maybe`.

```fsharp
let noValue = None
let someValueThatCouldBeNone = Some 42
printfn "is equal? %b" (noValue = someValueThatCouldBeNone)
// is equal? false
```

A similar approach can be taken to exceptions. Instead of throwing an exception that is hopefully handled somewhere, we return from the function that it was possible for an exception to have occurred.

```fsharp
let success = Ok 42
let error = Error "Something went wrong calculating the meaning of life"
printfn "is equal? %b" (noValue = someValueThatCouldBeNone)
// is equal? false
```

> Note: This could be the point where some might be wondering where I am going to start throwing the word Monad. This article will not. Monad, monoid, etc. are patterns as far as I am concerned. Their origins may be far more formal than the observational origins of OOP patterns like Vistor, or Strategy, but they are patterns none the less (in my opinion). The are no more necessary for FP than patterns are for OOP. Using them well can improve your code. Using them poorly can make it overly complicated.

### Benefits

- They should be immutable
- They should have value equality
- More powerful modelling options without resorting to inheritance

## Pattern matching

The final language feature I will point out is pattern matching. This is making it's way into C# now but for me the combination of pattern matching with **sum types** is what I miss most when working in a language that does not support algebraic types.

```fsharp
let calculateMeaning() =
    if ((Random()).Next() % 2) = 0 then Ok 42
    else Error "Something went wrong calculating the meaning of life"

match calculateMeaning() with
| Ok nr -> printfn "The answer to life is %i" nr
| Error err -> printfn "%s" err
```

When calculating the meaning of life, the returning result will be of type `Result<int,string>`. We can `match` on this where we handle each case that is possible. If you have a statically typed language the compiler can tell you when your match is not covering every case.

If working with `Option` or `Result` sounds interesting to you, I suggest checking out [Railway oriented programming](https://fsharpforfunandprofit.com/rop/).

### Benefits

- Often results in easier to understand control flow
- In some languages, the compiler can tell you if all possibilities are matched against

## Conclusion

In this post we covered a few language features that are nice to have for making you development experience using functional programming productive. These support the ideas of FP and make it faster to write code that is bug free. This post was mostly about addressing things that where not in the [previous post](/what-is-fp). Finally, monads, etc. were not covered at all, since I consider them patterns. Although they are intimately connected with FP, I do not think they are strictly necessary to say you are writing code using the principles of FP. 