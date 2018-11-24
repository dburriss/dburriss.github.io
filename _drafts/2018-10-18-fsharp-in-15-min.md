---
layout: post
title: ""
subtitle: ""
description: ""
permalink: 2018-10-18-fsharp-in-15-min.md
author: "Devon Burriss"
category: 2018-10-18-fsharp-in-15-min.md
tags: [Functional]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/explore-590.jpg"
published: false
---

## Understanding FSharp

We can assign values in F# using the `let` keyword. Since F# is a functional first language even functions can be assigned with the `let` keyword.

```fsharp
// a function that takes a string and changes it to uppercase
let toUpper (s:string) = s.ToUpper()
// a lowercase string
let lowerTxt = "i like to shout"
// use .NET object oriented approach directly
let upper0 = lowerTxt.ToUpper()
// use the custom function
let upper1 = toUpper lowerTxt
// assign value in more functional way using pipe operator. |> pipes the value on the left into the function parameter of the function on the right
let upper2 = lowerTxt |> toUpper
```

### Commonly used types

First lets touch on some common data types.

#### Simple

`string` represents text in a program. When used in a program a `string` is defined like this with quotations `"Some text in my program"`. We saw usage of it above. It is often prudent to wrap OO .NET string methods in your own since the F# compiler struggles to work out the types of methods in the .NET library. Or any OO style methods for that matter.

`int` defines whole numbers.

`decimal` is a good choice for money when you need cents.

`DateTime` and `DateTimeOffset` represent both a date and time component. The latter incorporates a timezone offset.

`unit` this is a special type that represents nothing. Later we will see how a function that makes some changes like printing some values, then it may not need to return anything. In this case a type `Unit` will be returned.

You can the make your own record type which is the product of simpler types:

#### Record

```fsharp
// define a type called Data
type Data = {
    Name:string
    Age:int
    Date:DateTime
}

//assign to a value
let myData = {
    Name = "Bob"
    Age = 40
    Date = DateTime.UtcNow
}
```

#### Tuple

Another common type is a **Tuple** represented as `Tuple<T1,T2>`. So tuples are kind of like record types without the named fields. In F# we define a tuple type like this `bool * int` and we would create an instance of that type like so `let myTuple = (true,99)`. Tuples are useful as intermediary values between functions.

```fsharp
//create a tuple of type bool * int
let myTuple = (true,99)
// use the fst function to get the first value in the tuple
let b1 = myTuple |> fst
// use the snd function to get the second value in the tuple
let n1 = myTuple |> snd
// use pattern matching to get the values
let (b,n) = myTuple
//val b : bool = true
//val n : int = 99
```

#### Collections

`List` and `Seq` are used when working with a collection of values of the same type. We will be using `Seq` when reading or writing a stream to a file. `List` will be used when collecting a list of data elements in memory to write to the file.

There are plenty of helpful functions that are available for working with both Seq and List. So let's just look at `List` quick.

```fsharp
let ints = [1..10] // int list = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10]
// using a function value to filter
let isEven i = (i % 2) = 0
let evens = List.filter isEven ints // int list = [2; 4; 6; 8; 10]

// using an anonymous function to filter
let evens = List.filter (fun i -> (i % 2) = 0) ints // int list = [2; 4; 6; 8; 10]
```

Above we see the usage of the `filter` function for list. We also are seeing a another way of creating functions in F# using the `fun` keyword. Notice how we don't need to specify the `x` is an `int`. The F# compiler infers this from the usage. This is a super powerful feature of the F# compiler. Type inference. It gives us the safety and support of a statically typed language like C# with the clean syntax of a dynamic language like Python. We will see this more when we dive into functions later.

Let us look at another important `List` function. `map` can be used to transform (or map) every element in a list to another.

```fsharp
let doubled = ints |> List.map (fun x -> x*2) // int list = [2; 4; 6; 8; 10; 12; 14; 16; 18; 20]
let numbersTxt = ints |> List.map string // ["1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "10"]
```

Notice how in this example we are using the pipe operator (|>). When using `map` we get a new list with the values being the result of applying the supplied function to each element of the original list.

#### Functions

The final type worth mentioning now, and it is quite important, is *functions* as types. In F# a function can be passed around like any other value.

```fsharp
// int -> string
let intToString i = i |> string
//^       ^     ^          ^
//keyword |     |          |
//      value   |          |
//       function argument |
//                  operator function
printfn "%s" (10 |> intToString)
// "10"
```

So this is a very specific function that takes an `int` and converts it to a `string`. In the function body (*everything after the =*) the *`string`* here is actually a operator function takes a value and converts it to a `string` type.

Something that is very important when using F# is being able to understand the type signature of the functions. The `intToString` function above then takes in an `int` and returns a `string`. In F# this type signature for the function is represented as `int -> string`.

What if we had multiple parameters? Look at this function `let add x y = x + y`. If we are adding `int`s it would have the signature `int -> int -> int`. This is read as "it takes an `int`, takes another `int`, then returns an `int`". We can be explicit about the type signatures of functions.

```fsharp
// define a type for a function that takes an int and returns an int
type B = (int -> int)
let times2:B = fun i -> i*2

// it is possible to be explicit with the function without a type
let times3 (i:int) : int = i*3

// Nothing magic about the fun keyword, compiler still works types out implicitly
let time10 i = i*10

//or
let time11 = fun i -> i*11
// all signatures are `int -> int`
```

The final part of function signature is generics. Whether explicit of implicit, the function work with specific types dictated by the signatures. We can make functions or function types more generic. In F# we mark a generic type with `'` eg. `'a`.
So if we have a function signature of `'a -> 'a` we are saying "it takes type `a` and returns type `a`". Our `times3` function could fit into this, as well as our earlier `toUpper`. Both return the same type as they take in eg. `int -> int` and `string -> string`.

```fsharp
type B<'a> = ('a -> 'a)
let toUpper':B<string> = fun s -> s.ToUpper()
let times2:B<int> = fun x -> x*2
```

Let's define a slightly more complicated example. We are going to define a type the defines what a print function should look like. It should have a function for formating a value to a string and the value.

```fsharp
// a type that takes a generic parameter. It is a function that take another function as its first parameter `'a -> string`
// takes a value 'a
// returns unit
// ('a -> string) -> 'a -> unit
let print<'a> fmt (v:'a) = v |> fmt |> printfn "%s"
// A formatter for int
let intFormater x = x |> sprintf "An integer %i"
// A formatter for DateTime
let dateFormater (x:DateTime) = x |> sprintf "A date %A"
// print an int
print intFormater 10 // An integer 10
// print a DateTime
print dateFormater System.DateTime.UtcNow // A date 29/09/2018 16:11:32
// try print a DateTime using the inFormatter gives an error
print intFormater System.DateTime.UtcNow // error FS0001: This expression was expected to have type 'int' but here has type 'DateTime'
```

So hopefully you are a little more comfortable with functions now. Let us demonstrate a characteristic of F# functions. You can curry F# functions. This means if you have a function with multiple parameters you can apply an argument value and receive a new function with that value. Let's look at an example and then talk through it.

So `print` has 2 parameters. `('a -> string) -> 'a -> unit`

1. `('a -> string)` it takes a function for formatting `'a` to a `string`
1. `'a` is the value to print
1. `unit` because the function prints out the value so doesn't return a value for use

If we apply the first value only, we get back a new function where `'a` is not no longer generic but an `int`.

```fsharp
// ('a -> string) -> 'a -> unit
let print<'a> fmt (v:'a) = v |> fmt |> printfn "%s"
// (int -> string)
let intFormater x = x |> sprintf "An integer %i"
// Now lets curry print to a new function printInt
// (int -> unit)
let printInt = print intFormater
printInt 99 // An integer 99
printInt 200 // An integer 200
```

So we were able to create a new more specialized function `printInt` by currying `print` with `intFormatter`.
