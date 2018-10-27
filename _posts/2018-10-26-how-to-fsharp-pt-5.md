---
layout: post
title: "How to F# - Part 5"
subtitle: "Making magic with Pattern Matching"
description: "In this post we explore the magic of pattern matching including value deconstruction, match expressions, and active patterns"
permalink: how-to-fsharp-pt-5
author: "Devon Burriss"
category: Software Development
tags: [Functional,F#,.NET]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/hole-bg.jpg"
social-img: "img/posts/2018/rubiks-500.jpg"
published: true
---
In the [previous post](/how-to-fsharp-pt-4) we looked at language features that allowed us to control the flow of our applications. In this post we will look at Pattern Matching, which allows for some very powerful control flow, as well as some neat deconstruction of values.
<!--more-->
In this post we will look at a few ways of deconstructing values and end with an in-depth look at `match` again.

## Deconstructing a tuple

Lets ease into pattern matching by looking at deconstructing a tuple. Remember a tuple is a little like a record except it has no named accessor fields. We have the `fst` and `snd` functions that get the value for you but if you have more than 2 elements in your tuple you are on your own. Lets refresh by looking at an example from [part 1](/how-to-fsharp-pt-1):

```fsharp
//create a tuple of type bool * int
let myTuple = (true,99)
// use the fst function to get the first value in the tuple
let b1 = fst myTuple
// use the snd function to get the second value in the tuple, with pipe forward operator
let n1 = myTuple |> snd
// use pattern matching to get the values
let (b,n) = myTuple
//val b : bool = true
//val n : int = 99
```

Notice how on the last line `let (b,n) = myTuple` we deconstruct the tuple to individual values. This is a form of pattern matching. The pattern on the left matches the pattern of a tuple that is being assigned to it so F# is able to assign the respective elements from the tuple to each of those elements.

```fsharp
let tripleThreat = (true,99,"str")
let (b2,n2,s1) = tripleThreat
```

As you would expect, when you add more elements the pattern on the left needs to match.

## Function arguments

Lets drill into this a bit more. We can use it to assign values while deconstructing a tuple but what if we want to accept a tuple argument into a function and we only care about the deconstructed values.

```fsharp
// bool * int  -> unit
let takeATup1 tup =
    let x = fst tup
    let y = snd tup
    if(x) then printfn "%i" (y + 1) else printfn "%i" (y - 1)

let takeATup2 (x,y) =
    if(x) then printfn "%i" (y + 1) else printfn "%i" (y - 1)

let myTuple = (true,99)
takeATup1 myTuple
takeATup2 myTuple
```

> Output: 100  
> Output: 100

In `takeATup1` we accept the argument as a tuple value. In `takeATup2` we pattern match on it to be able to get straight to its constituent elements. So it is possible to deconstruct a tuple in the argument. Wouldn't it be useful if we could deconstruct other types?

## Sum type

A common pattern in F# is to create specific types to document your code a little better using the type system. Say we had an `int` that uniquely identifies a row in a spreadsheet table. We could just make it an `int`, or we could create a special type to represent what it is. Doing this in F# is super easy. Then whenever we need to get that `int` out to use it, we simply extract it using the same deconstruction technique we saw earlier.

```fsharp
type Id = | RowId of int

let getRow (RowId rid) =
    printfn "%i" rid
    (rid,true)

let i = RowId 1
let row = getRow i
```

> Output: 1

Did you notice how, like with tuples, the pattern matches what is used to construct the value in the first place?

## Product type

As one last example before we switch to `match`, you can do the same kind of deconstruction with record types.

```fsharp

type Person = { Name:string; BirthYear:int }

let p1 = { Name = "Devon"; BirthYear = 2120 }

let sayHello { Name = name; BirthYear = _ } =
    printfn "Hello %s" name

sayHello p1
```

> Output: Hello Devon

Again it looks like we are constructing the value in the argument. One thing of note is that I used the wildcard symbol `_` to show that we don't care about the value of `BirthDate` within the scope of this function.

## Match expression (revisited)

We covered `match` in [part 4](/how-to-fsharp-pt-4) but are going to revisit it with our new-found knowledge of pattern matching.

To dip our toes in lets create a function that takes a `bool` and an `int` and if the first argument is `true`, then it increments the second argument else it decrements it.

```fsharp
// bool -> int -> int
let incDec t n  = 
    match (t,n) with
    | (true,x) -> x + 1
    | (false,x) -> x - 1

printfn "%i" (incDec true 10)
printfn "%i" (incDec false 10)
```

> Output: 11  
> Output: 9

Note how we created a tuple in the input expression of the `match` and then pattern match for the different options.

Before we move on lets highlight some other patterns and features. Let us add 2 more constraints to out function.

1. If the value is `1` we ignore the boolean and just return `1`
1. If the value is less than or equal to `0` we will return `0`

```fsharp
let incDec t n  =
    match (t,n) with
    | (_,1) -> 1
    | (_,x) when x <= 0 -> 0
    | (true,x) -> x + 1
    | (false,x) -> x - 1

printfn "%i" (incDec true 10)
printfn "%i" (incDec false 10)
printfn "%i" (incDec false 1)
printfn "%i" (incDec false -5)
```

> Output: 11  
> Output: 9  
> Output: 1  
> Output: 0

A few things to note here. Firstly, we used the wildcard `_` to indicate that we don't care about the value of the boolean. It will match the first element whether `true` or `false`. Secondly, we used a condition with the `when` keyword. This requires that the pattern is matched AND that the condition is then met. Thirdly, the order matters here. If we had added the 2 new cases at the end of the `match` they would never be hit.

## Active Patterns

Active Patterns is a really cool feature that can be used to simplify the `match` cases by wrapping up some pattern matching into named partitions. I am going to cover partial active patterns here, as I have found them the most useful.

To demonstrate the usage of partial active patterns we are going to code up a little game called FizzBuzz. How it works is you increment numbers saying the number unless:

1. The number is divisible by 3, then you say *Fizz*
1. The number is divisible by 5, then you say *Buzz*
1. The number is divisible by both 3 and 5, then you say *FizzBuzz*

```fsharp
// define partial active patterns
let (|Fizz|_|) i = if ((i%3) = 0) then Some() else None
let (|Buzz|_|) i = if ((i%5) = 0) then Some() else None
// use partial active patterns
let fizzbuzz i = 
    match i with
    | Fizz & Buzz -> printf "Fizz Buzz, "
    | Fizz -> printf "Fizz, "
    | Buzz -> printf "Buzz, "
    | x -> printf "%i, " x
// run fizz buzz for numbers 1 to 20
[1..20] |> List.iter fizzbuzz
```

> Output: 1, 2, Fizz, 4, Buzz, Fizz, 7, 8, Fizz, Buzz, 11, Fizz, 13, 14, Fizz Buzz, 16, 17, Fizz, 19, Buzz,

They are called partial active patterns because in the definition `|Fizz|_|` they have the wildcard `_` that allows for a match to not occur. We indicate that the match happened by returning `Some` and it did not by returning `None`. We will encounter `Some` again in a later post when we tackle handling no data.

Notice how for "FizzBuzz" we used `&` to check it matched both.

I want to point out something that may not be clear. If we wanted to pattern match and deconstruct the value in the match we could do that by sending the value back with `Some(i)`. Then the case would look like this `| Fizz x -> printf "Fizz(%x) " x`.

These take some playing around with to get comfortable with but once you are they are great for cleaning up your `match` and making them more descriptive.

## Conclusion

Today we looked into various ways you can use pattern matching to both get values and branch your application logic. We also explored partial active patterns by writing an implementation of the FizzBuzz game.

## Resources

1. [Pattern Matching on Wikipedia](https://en.wikipedia.org/wiki/Pattern_matching)
1. [Pattern Matching on MS docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/pattern-matching)
1. [Match Expressions for fun and profit](https://fsharpforfunandprofit.com/posts/match-expression/)
1. [Active Patterns](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/active-patterns)

## Credits

1. Social image by[Olav Ahrens Røtne](https://unsplash.com/@olav_ahrens)