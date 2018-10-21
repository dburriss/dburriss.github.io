---
layout: post
title: "How to F#"
subtitle: "Part 1: Assigning and types"
description: "In part 1 of this series we will look at assigning values and at some of the common types used when programming"
permalink: how-to-fsharp-pt-1
author: "Devon Burriss"
category: Software Development
tags: [Functional,F#,.NET]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/todo-bg.jpg"
social-img: "img/posts/2017/apple.jpg"
published: true
---
Over the last few weeks I have been showing various people with different levels of programming experience how to use F#. This post is the first in a series on the basics of programming with F#. In this one we cover assigning values and the different types those values can take on.

## Introduction

F# is a functional first language that allows for interoperation with the rest of the .NET ecosystem. This means you can use it mixed in a solution with other .NET languages like C# and VB, you can use all available Nuget packages, and you can reuse your knowledge of the existing base class library (BCL) if you are already a .NET developer. To achieve this interoperation F# allows you to program in an object oriented paradigm if you would like but it will often feel a bit clunky compared to the functional paradigm. Hence the "functional first".

## Warning

I will be covering a lot of ground in as concise a way as possible. In a lot of ways this is "just enough to be dangerous". That been said learning a programming language is much like learning a spoken language. The best thing you can do is use it even if you feel stupid doing so.

## Syntax

Depending on your background, one of the first things to stand out with F# is its lack of curly braces. F# uses the whitespece indentation to determine the scope of something. We will see this in a future article when we deal with functions.

### Assigning value

We can assign values in F# using the `let` keyword. Since F# is a functional first language even functions can be assigned with the `let` keyword. Do not worry too much about what the examples below mean, some of it will be covered later.

```fsharp
// the number 1 assigned to i
let i = 1
// a string assigned to `lowerTxt`
let lowerTxt = "i like to shout"
// assign a function that takes a string and changes it to uppercase
let toUpper (s:string) = s.ToUpper()
// assign value from the result of a function
let upperTxt = toUpper lowerTxt
```

In other languages these values would often be called *variables*. In F# values are immutable (cannot be changed), so they are not variable. You have to mark values that you want to be mutable with the `mutable` keyword but I recommend not doing this unless you are performance tuning. Usually it is a sign you are not doing things functionally.

### Commonly used types

First lets touch on some common data types. Types define the type of data we can store in a value.

#### Simple

`string` represents text in a program. When used in a program a `string` is defined like this with quotations `"Some text in my program"`. We saw usage of it above. It is often prudent to wrap OO .NET string methods in your own since the F# compiler struggles to work out the types of methods in the .NET library. Or any OO style methods for that matter.

`int` defines whole numbers.

`decimal` is a good choice for money when you need cents.

`DateTime` and `DateTimeOffset` represent both a date and time component. The latter incorporates a timezone offset.

`unit` this is a special type that represents nothing. Later we will see how a function that makes some changes like printing some values, then it may not need to return anything. In this case a type `Unit` will be returned.

#### Complex

Sometimes you want to capture data together in a way that pulls together simple types to represent a single coherent idea. In F# we can use classes as in C# but sticking with simplicity and immutability, a better option is F#'s record type.

```fsharp
// define a type
type Person = {
    Name:string
    Birth:DateTime
}

// create an instance of that type
let devon = {
    Name = "Devon"
    Birth = DateTime.Parse("2121/01/01")
}
```

Above we defined a new type called `Person` and then created an instance of that type assigned to a value called `devon`.
Even though a value is immutable, F# does provide an easy way to create a new value from an old value while updating the fields.

```fsharp
let devonBurriss = { devon with Name = "Devon Burriss" }
```

#### Tuple

Another common type in functional programming is a **Tuple**. A simple tuple is represented as `Tuple<T1,T2>`, meaning it has 2 values inside of type `T1` and `T2`. So tuples are kind of like record types without the named fields. In F# we define a tuple type like this `bool * int` and we would create an instance of that type like so `let myTuple = (true,99)`. Tuples are often useful as intermediary values between functions.

```fsharp
//create a tuple of type bool * int
let myTuple = (true,99)
// use the fst function to get the first value in the tuple
let b1 = fst myTuple
// use the snd function to get the second value in the tuple
let n1 = myTuple |> snd
// use pattern matching to get the values
let (b,n) = myTuple
//val b : bool = true
//val n : int = 99
```

#### Collections

Dealing with a collection of elements of the same type is a common occurrence in programming. Whether a sequence of numbers or a list of people, you need a way to work with them. Although you can of course use the .NET collection types in F#, F# has some built-in types that make it easier to interact with collections in a more functional way. These types are `List`, `Array`, and `Seq`. Most of the functions for dealing with all these types are shared across all of them.

Examples:

```fsharp
let lstFst = List.head [1;2;3] // 1
let arrFst = Array.head [|1;2;3|] // 1
let seqFst = Seq.head (seq { yield 1; yield 2; yield 3})
```

As you can see, the same function is available for getting the first element of the collection on each of the relevant modules.

So why are there 3 different collection types that seem so similar?

`list` is the go to collection for me when working with in-memory data. It is an immutable collection so encourages functional best practices. This data structure is optimized for iterating through it and accessing the first element of the list (under the hood it is a linked list). Being a native F# data structure it allows superior pattern matching compared to other data types. This is actually quite common in functional programming where we often interact with a list as `head::tail` (pattern matching) where head is the first element in a list and tail is the rest of the list.

`array` is a good choice if you need random access to elements in the collection. Is an alias for *BCL* `Array`.

```fsharp
let j = Array.get [|1;2;3|] // val: j = 2
```

`seq` is a lazily evaluated collection and so can represent an infinite list. This can be memory saving as each element is evaluated as needed. Is an alias for the *BCL* `IEnumerable`.

Two other F# data structures worth mentioning now is `map` and `ResizeArray`. `map` gives us a key-value dictionary that is often quite useful as a lookup:

```fsharp
let funcFirstLangs = Map.ofList [("csharp",false);("fsharp",true)]
let isFuncFirst = Map.find "fsharp" funcFirstLangs // val: isFuncFirst = true
```

`ResizeArray` is usually of interest when working with C# as it is an alias for `List`.

#### Discriminated Unions

The last type I want to touch on is Discriminated Unions (DU or sum types). DUs allow you to define a type which may be one of many types. Let me try explain by example.

```fsharp
type Rating =
| Skipped
| RemindLater of DateTime
| JustVote of int
| VoteWithComment of int*string

let vote = VoteWithComment (5,"This is the best application every!!!! Worth every cent!")
```

Here we are defining a **DU** type `Rating` that represents a rating of a mobile application. Although each of the 4 cases contains different information, any case will be of type `Rating`. We will explore this more in a later post when we tackle pattern matching.

## Conclusion

So that is the end of the first entry into how to use F# We covered how to assign values and took a whirlwind tour of some of the different types that those values could be. In future installments we will dive into some more advanced topics of working with these values as well as explore the idea of functional programming. I hope you found this interesting and are excited for the next installment. If anything was unclear I would really appreciate your feedback so I can improve this for the next reader who may come along.

## Resources

1. [Learn F# resources](https://fsharp.org/learn.html)
1. [Cheatsheet](http://dungpa.github.io/fsharp-cheatsheet/)
1. [fsharp for fun and profit](https://fsharpforfunandprofit.com/posts/list-module-functions/)
1. [Algebraic data types](https://en.wikipedia.org/wiki/Algebraic_data_type)