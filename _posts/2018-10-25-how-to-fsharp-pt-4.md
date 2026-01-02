---
layout: post
title: "How to F# - Part 4"
subtitle: "Making decisions with control flow"
description: "In part 4 we look at how we make decisions in our code to do one thing or another and how to loop over collections"
permalink: how-to-fsharp-pt-4
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2018/pipes-500.jpg"
published: true
topics: [platforms-runtime]
keywords: [Software Development, Functional, F#, .NET]
---

---
In the [last post](/how-to-fsharp-pt-3) we finished off our dive into functions. In this post we will look at control flow. How do we make a branching decision? How do we loop through something until some condition is met?
<!--more-->
I am going to try keep this post short. The reason for this is that although you will invariably need to use control flow expressions in your code, they are stylistically not very functional and there are usually more functional ways to achieve the same thing. We explore those more functional techniques in this and future posts.

## If then else

Other than`match` (covered later), `if` is probably the next most useful control flow expression we will touch on in this post. The `if` expression takes a `bool` and if `true` proceeds with the `then` body. Usually there is an `else` and we will go through when that is necessary.

```fsharp
let b = true
if (b) then printfn "Is true" else printfn "Is false"
```

The above will print out *Is true*, and not print *Is false*.  
Maybe we don't want to print out anything if the value is `false`. We can do this:

```fsharp
let b = true
if (b) then printfn "Is true"
```

Now if you changed `let b = false`, nothing would be printed.

What if we wanted to **return a value** based on some condition though without an `else`?

```fsharp
let v = if(b) then 1 // <- Error: This 'if' expression is missing an 'else' branch.
```

At least the error message is pretty clear about what the problem is. With the print example we were returning `unit` so it didn't matter if nothing was returned. Here the expression has to return a value because we are assigning that value.

```fsharp
let v = if(b) then 1 else 0
```

So depending on whether `b` is `true` or `false`, `v` will have a value of `1` or `0` respectively.

Lets take a look at something a bit more complex:

```fsharp
let divideBy d n = n/d
let numerator = 10
let denominator = 2

let j = if(denominator <> 0) then 
            printfn "Dividing by %i, not 0" denominator
            let x = numerator |> divideBy denominator
            printfn "The answer is %i" x
            x
        else
            printfn "Dividing by 0"
            0
```

> Note that we don't have to assign this to a value, here `j` but it would be pretty pointless to return a value and not use it. The compiler will give you a warning at this point *The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.*  
> This is asking us to call `ignore()` in each branch of the `if` expression.

We can have multiple lines in either branch, organized by indentation. Just like functions the last expression is what is returned as the value of the `if-else` expression for each branch.

### Scope

In the previous post I mentioned scope. This is a good opportunity to demonstrate scope. Check out the assigning of the `denominator` value below.

```fsharp
let divideBy d n = n/d
let numerator = 10
let denominator = 0

if(denominator <> 0) then 
    printfn "Dividing by %i, not 0" denominator
    let x = numerator |> divideBy denominator
    printfn "The answer is %i" x
    x
else
    printfn "Dividing by 0"
    let denominator = 1
    printfn "Instead by %i, not 0" denominator
    let x = numerator |> divideBy denominator
    printfn "The answer is %i" x
    x

printfn "Denominator is %i" denominator
```

The above prints out the following:

```text
Dividing by 0
Instead by 1, not 0
The answer is 10
Denominator is 0
```

Notice here how we set a value for `denominator` within the `else` branch that shadows the outside one. Once we are back to the scope outside the if, `denominator` is back to `0`, even though it was set to `1` in the `else` branch. It was `1` for the scope of the `else` branch of the expression as it was set in that scope.

### If / elseif / else

It is (maybe) worth mentioning that you can have more than 2 branches by using `elif`.

```fsharp
if(x = 1) then printfn "x is 1"
elif (x = 2) then printfn "x is 2"
else printfn "x is not 1 or 2"
```

We will briefly cover `match` next and even when **if / else** seems a cleaner solution, once you are using `elif` you almost certainly should be using `match` instead.

## Match

We will hopefully cover pattern matching in more detail in a later entry but no coverage of functional control flow is complete without `match`.

Lets re-write the previous example using `match`:

```fsharp
match x with
| 1 -> printfn "x is 1"
| 2 -> printfn "x is 2"
| _ -> printfn "x is not 1 or 2"
```

Note that `_` is a catch-all, like else is. 
This is a much cleaner and more functional way to do control flow. A nice benefit here is that the compiler gives you a warning if you are not matching exhaustively on all options of the matched value.

We will hopefully circle around to `match` again when covering pattern matching as `match` is far more powerful than demonstrated here.

[Microsoft docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/match-expressions)

## for..in

If you need to loop through an entire collection and do something you could use the `for pattern in enumerable-expression do body-expression` syntax. This is like `foreach` in many other languages. Lets see what that looks like:

```fsharp
let numbers = [1..10]
for x in numbers do
    printf "%i " x
```

> Output: 1 2 3 4 5 6 7 8 9 10

See how you can easily create a range of values using `start..finish` syntax. We use this to define `numbers`. Then for each element of the list we print the value which is in `x`.

We will hopefully cover collections in an upcoming post but for interest sake lets see how this would be done in a more functional way.

```fsharp
let numbers = [1..10]
numbers |> List.iter (printf "%i ")
```

Unsurprisingly the functional approach is to call the `iter` function on the `List` module. This `iter` function has the signature `('T -> unit) -> 'T list -> unit`. Lets break that down:

- `('T -> unit)`: a function defining the action to take for each element in the list
- `'T list`: the list to iterate through
- `unit`: returns unit so this function is designed to iterate through a function and do something, not return a value

There are many more functions for working with lists in the `List` module and matching ones for `Array` and `Seq`.

[Microsoft docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/loops-for-in-expression)

## for..to

While `for..in` is for iterating over a collection, `for..to` allows you to iterate from a start value to another. This is like a `for` loop in other languages.

```fsharp
let ns = [|1..10..100|]
for i=0 to ((Array.length ns)/2) do
    printf "%i " (Array.get ns i)
```

> Output: 1 11 21 31 41 51

In our example we have an array that has numbers 1 to a 100 in increments of 10. We only iterate through half the list.

[Microsoft docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/loops-for-to-expression)

## while

What if we want to iterate until a certain condition is true? The following code gets a random number until that number is `7`.

```fsharp
let random = new System.Random()
let aNumber() = random.Next(1,10)
let mutable n = 0
while (n <> 7) do
    printf "%i " n
    n <- aNumber()
```

> Output: 0 9 9 1 6 5 2 2 6 6 2 6 6 1 2 3 6 8 8 1 3 2 2

We kept going through the `while` loop until `aNumber()` returned `7`.

[Microsoft docs](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/loops-while-do-expression)

## Conclusion

In this post we looked at ways to represent branching logic and ways to iterate over values. Remember that much of this is a very imperative approach and as such is not used a lot in the function paradigm. We looked at some functional techniques for dealing with branching and looping and will continue this in future articles. Next up we look at [Pattern Matching](/how-to-fsharp-pt-5).

## Credits

1. [Crystal Kwok](https://unsplash.com/@spacexuan)

