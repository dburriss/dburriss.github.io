---
layout: post
title: "How to F# - Part 3"
subtitle: "Working with Functions and getting them to work for you"
description: "In part 3 we will dive deeper into functions exploring different techniques for working with them"
permalink: how-to-fsharp-pt-3
author: "Devon Burriss"
category: Software Development
tags: [Functional,F#,.NET]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/vents-bg.jpg"
social-img: "img/posts/2018/hammer-500.jpg"
published: true
---
[Previously](/how-to-fsharp-pt-2) we began exploring some theory behind functions. In this post we will look at practical techniques for working with functions.

## Working with functions

Using functions is unsurprisingly the bread and butter of functional programming, let us see if we can define a slightly more complex function without butting into too many new concepts. We are going to define a function that cleans up an input `string` and then saves it to disk.

```fsharp
// some helper string functions
// string -> string
let trim (s:string) = s.Trim()

// string -> string -> unit
let write path content =
    let sanitized = trim content
    File.WriteAllText(path, sanitized)

// use the write function
write "/path/to/file.txt" "Some text to write to file"
```

This is our first multi-line function and let us go through a few things that may not have been immediately obvious from the single line function. Firstly, note that the body of the function is defined by the indent. For the function the size of the indent does not matter, as long as it is the same throughout the scope. We will dive into this a bit more when we touch on scope in a later post on control flow. Secondly, the value of the last expression is what is returned from the function, in this case `unit`. You didn't need to explicitly use `return` like in many other languages. This is because functions ALWAYS return something so the compiler can assume that the last expression result is the return.

A big part of the flexibility of functional programming comes from being able to easily tie functions together in interesting ways to build up more complex functionality. Let us apply this idea to the `write` function. We are going to pass a function into the `write` function that will do the sanitization, thus allowing the client of the function to decide what "sanitized" means.

```fsharp
// ('a -> string) -> string -> string -> unit
let write sanitizer path content =
    let sanitized = sanitizer content
    File.WriteAllText(path, sanitized)

// use the write function
write trim "/path/to/file.txt" "Some text to write to file"
write (fun (s:string) -> s.Substring(0, 140)) "/path/to/file.txt" "Some text to write to file"
```

See how we just passed the `trim` function in as an argument? This of course could be any function as we see in the second usage.
Ok but this signature `('a -> string) -> string -> string -> unit` is getting a bit more hairy, so lets break it down. `('a -> string)` is the signature for the `sanitizer` function we are now passing into the `write` function. The F# compiler has inferred that the function doesn't need to be of type `string -> string` for our `write` function to work. As long as the `sanitizer` function returns `string`, the input can be of any type. This is a generic parameter then and in F# a generic parameter is indicated with a leading `'`. So `('a -> string)` indicates a function that takes any type and returns a `string`. The rest of the signature `string -> string -> unit` then remains the same representing the *path*, *content*, and return value type.

### Currying

Now is the time to introduce *currying*. This has nothing to do with food but instead is a technique named after [Haskell Curry](https://en.wikipedia.org/wiki/Haskell_Curry). *Currying* is the technique of taking a function that takes multiple arguments and evaluating it as a sequence of single argument functions. If that doesn't make sense, don't worry, it is easier to understand from examples.

We made our `write` function more flexible by allowing for a `sanitizer` function to be passed in but now every time we want to use it we need to supply that sanitizer function. What if in an area of my code *sanitizing* always means *trim* the string? What if it was expected that we always do this before saving a `string` to disk? Well then we can define a new function by *currying* `write` with an argument.

```fsharp
// string -> string -> unit
let sanitzedWrite = write trim
```

Now we have a new function `sanitzedWrite` with the `trim` function baked in.  
Note how we are back to our previous signature of `string -> string -> unit` just like before we introduced the `sanitizer` argument. We are able to optimize for our needs and still leave options open for when `write` is needed without the `trim`. Let us look at that case next.

### Identity

This seems like a good time to introduce a concept whose value may not be immediately obvious. It is the idea of *identity*. I will not go into any theory on monoids, monads, or any category theory, there is an [awesome series from Mark Seemann that covers this](http://blog.ploeh.dk/2017/10/04/from-design-patterns-to-category-theory/). Suffice to say *identity* is a function that does nothing. 

The easiest way to explain *identity* is with examples:

1. The *identity* for addition is 0 : 5 + 0 = 5
1. The *identity* for multiplication is 1 : 2 * 1 = 2
1. The *identity* for `string` is "" : "hello" + "" = "hello"

In F# **identity** is defined by the function `id` which has the signature `'a -> 'a`. "So what"? you may ask. How could something that does nothing ever be useful? Well thankfully we have a useful example at hand already (it is almost like I planned it).

Imagine we have another section of our code that needs to write content to a file but has no rules about sanitization. It just needs to write the content as is.

```fsharp
// string -> string -> unit
let justWrite = write id
```

Of course we could have just put in our own function `fun x -> x` in there but this is actually quite a common situation when you are passing functions around to extend functionality, so a functional language like F# provides an easy way to do this.

### Piping

Hopefully now you are starting to feel a bit more comfortable with F# functions. One thing you will start noticing about functional code is the way it tends to flow. When everything has an input and an output, you tend to start organizing your code into these workflows that chain functions together. This can lead to some really readable code once you wrap your head around the idea. This is made possible by an operator in the language that allows you to do this in a really interesting and useful way. It is the *forward pipe operator* `|>` which passes the result of the function on the left to the function on the right.

Again let us look at some examples to try clarify. I will give multiple examples, first without `|>`, followed by with.

```fsharp
// trim a string
let trimmed1 = trim " some text "
let trimmed2 = "  some text " |> trim

// get first value of a tuple
let name1 = fst ("Devon",37)
let name2 = ("Devon",37) |> fst
```

So what does this have to do with pipelines? Let us try use this to chain a workflow together.

```fsharp
Console.ReadLine()              // read a line in from the console
|> toUpper                      // convert the string to uppercase
|> trim                         // trim the string
|> justWrite "/to/some/file.txt"// write it without trim since we already trimmed
```

Above you see a workflow where the input from the previous step is used as the argument to the following. We read in some `string`, uppercase it, trim it, and then write it to file. I think that is some pretty descriptive code, don't you?

Note that *currying* comes in quite useful when wanting to use `|>` since you need the result of the function to line up with that of the function argument to the right of the `|>`.

### Composition

Another concept that will seem very similar is composing functions together with the *forward composition operator* `>>`. This operator allows you to take a function whose output matches the input of another function and compose those 2 together to for a new function.

```fsharp
// int -> int
let inc x = x + 1

// int -> string
let intToString (x:int) = x |> string

// int -> string
let incrementedString = inc >> intToString

1 |> incrementedString // val it : string = "2"
```

So if we applied this to our previous workflow we could summarize the middle step:

```fsharp
let prepareString = toUpper >> trim

Console.ReadLine()
|> prepareString
|> justWrite "/to/some/file.txt"
```

### Interop with .NET OO style

You may have noticed a few signatures of functions that seemed to look a little different. When using the .NET library, it can look a little different to the functional first code. This is because the .NET BCL is an object-oriented (OO) code base. F# can talk to it fine but it is a different paradigm. For example you can see when calling `File.WriteAllText(path, content)` it looks a lot like how it would look in C#. Another thing you may have noticed is that when defining functions that work with `string`s I usually am explicit about the type in the signature eg. `let trim (s:string) = s.Trim()`. This is because F# can need to help inferring the type when dealing with objects of types coming from the OO side of .NET. `string` seems to be the most common offender here. It is something to keep in mind. When dealing with `string` or other types from the .NET BCL it is often worth writing little functional wrappers around them like you see with `trim`.

## Conclusion

To close off this post I wanted to mention something important to consider when writing your own functions, and that is the idea of *purity*. A **pure** function is a function that has no internal dependencies that could change the output. As an example, our `trim` function `let trim (s:string) = s.Trim()` will give the same output for the same input every single time. Compare this to `File.ReadAllText("/path/to/file.txt")`. With `ReadAllText` the file could change at any time if the underlying file contents changes even though the same path was used as input. This is NOT a pure function.

Pure functions are easier to reason about and easier to test and so should be favoured. In the example above we pushed our impure functions to the beginning and end of the workflow and had our pure functions in the middle. This is generally a good pattern to follow.

So we covered quite a lot in this post and there is plenty more that could be said about functions but I think you have enough now to start working with them yourself. Didn't I tell you it would be `fun`? As always I appreciate any suggestions or questions, and please share this series with anyone you think might get value from it.

The next article will be out soon on Control Flow.

## Resources

1. [Symbol reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/symbol-and-operator-reference/)

## Credits

1. Social image by [Markus Spiske](https://unsplash.com/@markusspiske)