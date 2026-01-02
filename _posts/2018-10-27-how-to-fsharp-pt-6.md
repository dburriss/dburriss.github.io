---
layout: post
title: "How to F# - Part 6"
subtitle: "Handling data that is not there"
description: "In Part 6 we explore ways of handling cases where data is not there with Maybe, and how to wrap null into this way of working with data"
permalink: how-to-fsharp-pt-6
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/hole-bg.jpg"
social-img: "img/posts/2018/frame-500.jpg"
published: true
topics: [platforms-runtime]
keywords: [Software Development, Functional, F#, .NET]
---

---
Sometimes when dealing with data, the value you are expecting does not exist. Functional programming has a common abstraction for dealing with this called **Maybe**. In F# this abstraction is known as `option`.
<!--more-->
Rather than just diving into the functional way of handling no data lets briefly dive into how non-functional languages typically handle the absence of data, namely `null`.

## What is the problem with null?

So what problem are we solving by abstracting what it means to have data or not? Well lets look at how things are typically handled in most popular languages. In languages like Java, C#, and Javascript `null` represents the intentional absence of any object. So why is this a problem? Firstly, `null` carries no information about the type of data that was expected. Was it a missing `string` or a `Person` object? If `null` is all you have, you by definition have NOTHING! The other problem is in the handling of it. You need to explicitly handle any case where a value may be `null`.

```csharp
// Problem 1: Need to check for null
if(!string.IsNullOrEmpty(email))
{
    SendEmail(email);
} else ...
```

This means your code can become littered with `null` checks and if you forget to check and a `null` sneaks through, your code will throw some kind of `NullReferenceException`.

```csharp
// Problem 2: If you do not check for null your application can blow up
var email = (firstname.ToLower()) + "@acme.com"; 
```

If `firstname` is `null`, this statement will throw an exception and possibly crash our application.

The strategies for mitigating these problems are to try catch all `null`s at the boundaries of your application and to use the [Null Object/Special Case](https://martinfowler.com/eaaCatalog/specialCase.html) pattern. We won't go into these but my main criticism is the noise it adds to the code.

## Maybe this is here

The nice thing about the `Maybe` abstraction is it is generic, unlike the **Special Case** and in general can be much more elegant, saving you from repeatedly checking for `null`.

As mentioned before, in F# the **Maybe** abstraction (known as a Monad in functional programming theory) is an `option`. To see how it works we are going to define a function that takes a name as `string option` and turns it into an email.

First, lets briefly discuss what `option` actually is. `option` can have one of 2 values : **Some of 'T** OR **None**. We can optionally have some value of type `'T`, else we will have `None`.

Below we see how we define a value with `Some` or `None`

```fsharp
let fname1 = Some "Brandon"
let fname2 = None
//string option -> string option
let makeEmail name = Option.map (fun n -> sprintf "%s@acme.com" n) name
let email1 = makeEmail fname1
let email2 = makeEmail fname2
```

> val email1 : string option = Some "Brandon@acme.com"
> val email2 : string option = None

`Option.map` has a signature of `('T -> 'U) -> 'T option -> 'U option`.

1. `('T -> 'U)` - a function that maps from `'T` to `'U`. This is a generic function so in our case it is a function of `string -> string`
1. `'T option` - the input value to map. In our case `'T` will be `string` that is the name
1. `'U option` - the return value of type `'U` will be the email `string`

## Alternatives

What if we wanted to have a fallback email incase no name was supplied? That is simple enough:

```fsharp
//string option -> string option
let makeEmail name =
    name
    |> Option.orElse (Some "info")
    |> Option.map (fun n -> sprintf "%s@acme.com" n)
```

We have changed to a pipeline style now where the `string option` is piped through `Option.orElse` which. If the value is `Some` it passes through, if it is `None` it gets the value of `Some("info")`.

Running again we would get the following value for `email2`:

> val email2 : string option = Some "info@acme.com"

## Handling null

What if we are getting values from a database but always wrapping them in `Some`. Then we would be getting values of `Some(null)`. We could convert the `Some(null)` to `None` using `Option.bind`. This has a signature `('T -> 'U option) -> 'U option`. So we would pass it a function of `string -> string option`, which you can see below is the `Option.ofObj` function.

```fsharp
//string option -> string
let makeEmail name =  
    name
    |> Option.bind Option.ofObj
    |> Option.orElse (Some "info")
    |> Option.map (fun n -> sprintf "%s@acme.com" n)
```

Finally, what if we are dealing with data that comes from a C# library and we had not wrapped them in `Some`? Values could be `null`. Lets ease out `makeEmail` constraints a bit and just accept `string`, we will then transform it directly to an `option` type. Unfortunately since databases and other languages make `null` an acceptable value we often do still have to deal with it when stepping outside our process.

```fsharp
//string -> string
let makeEmail name = 
    let sanitizeString (s:string) = null |> (fun x -> if (box x = null) then None else Some(x))
    name
    |> Option.bind sanitizeString
    |> Option.orElse (Some "info")
    |> Option.map (fun n -> sprintf "%s@acme.com" n)
    |> Option.get
```

In the above example I also then returned the contained values, so a `string` instead of `string option`.

Now at this point you might ask what is the point of using option and I would tend to agree. This is after all, demo code. I just wanted to point out how to sanitize a possible `null` value and then use `Option.get` to get the `'T` value. In this case `string`. `get` will throw a `ArgumentException` if passed a `None`.

## Conclusion

We really just scratched the surface of the functions available on `Option` but I hope you have seen how it can be used to represent the absence of data. Although when dealing with the outside world (outside your application process) you are still forced to think about the possibility of `null`, `Option` has some major advantages over its OO counterparts. For one there is a lot less branching logic. The `Option` functions will often just handle `None` elegantly. This is a particular challenge of the **Special case** approach which requires you to think about a specific implementation for every type that can be `null` and think about what a no operation means.

The most important function to understand here though would have to be `map`. We will see `map` over and over in different modules. It allows you to operate in the abstraction you are in without leaving that abstraction but still manipulate the data contained within.

Next up we will finally be [diving into collections](/how-to-fsharp-pt-7).

## Resources

1. [Null Values](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/values/null-values)
1. [Option](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/options)

