---
layout: post
title: "Converting between F# and C# types"
subtitle: "Breaking down conversions between C# and F# collections and functions"
description: "In this post we look at converting between F# collections list Seq, List, and Array and common C# collections and interfaces."
permalink: converting-fsharp-csharp
author: "Devon Burriss"
category: Software Development
tags: [F#,C#, Collections]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/stuff-bg.jpg"
social-img: "img/posts/2020/fsharp512sharp.png"
published: true
---
Every now and again in F# you run into needing to convert a `Seq` to something like `IList<>`. Depending on how often you do this, and if you are like me, you will need to search for this or try different things for longer than you would care to admit. So if nothing else, here I am capturing for myself how to tackle some of these conversions.
<!--more-->

## TL;DR

For the sake of this post being a reference post, I am going to post this class which captures a lot of the conversions. Here I try and capture the C# type as the type most closely related to an F# type, if that makes sense. In most cases, this is an `'T array`, since this is equivalent to a `'T []` array in C#. I do encourage you to read the rest of the article at least once, as I will try break down the types a bit so in the future it should be easier to figure out the conversions yourself.

For many of these, you will need to convert to `seq` and then to the F# type you want to work with. If that is not acceptable perhaps do it yourself with a loop. 

| From | To | Conversion |
| -- |:--:| --:|
| `IEnumerable<int>`  | `int seq`         | alias for                           |
| `List<int>`         | `ResizeArray`     | alias for                           |
| `IEnumerable`       | `seq`             | `Seq.cast`                          |
| `IEnumerable`       | `int array`       | `Seq.cast |> Seq.toArray`           |
| `IEnumerable`       | `int list`        | `Seq.cast |> Seq.toList`            |
| `ICollection<int>`  | `int seq`         | `:> seq<_>`                         |
| `IList<int>`        | `int seq`         | `:> seq<_>`                         |
| `int []`            | `int array`       | alias for                           |
| `System.Array`      | `obj seq`         | `System.Linq.Enumerable.OfType<obj>`|
| `seq`/`array`/`list`| `ResizeArray`     | `ResizeArray` ctor                  |
| `int seq`           | `IEnumerable`     | `:> IEnumerable`                    |
| `int array`         | `ICollection<int>`| `:> ICollection<int>`               |
| `ResizeArray`       | `ICollection<int>`| `:> ICollection<int>`               |
| `ResizeArray`       | `IList<int>`      | `:> IList<int>`                     |
| `ResizeArray`       | `int seq`         | `:> seq<_>`                         |
| `ResizeArray`       | `int array`       | `.ToArray()` instance method        |
| `f: unit -> int`    | `Func<int>`       | `Func<int>(f)` ctor                 |
| `Func<int>`         | `unit -> int`     | `fun () -> f.Invoke()`              |

> For those that can be cast with `:> seq<_>` like `ICollection<>` and `IList<>` you can use directly with the `Seq` module functions like `toList`, since those interfaces implement `IEnumerable<>`.

### Example

```fsharp
// This is for demonstration purposes only
type CSharpyType() =
    // seq<int>
    let mutable enumerableTProp = Seq.empty
    // seq<obj>
    let mutable enumerableProp = Seq.empty
    // int []
    let mutable arrayTProp = Array.empty
    // obj []
    let mutable arrayProp = Array.empty
    // int list
    let mutable listTProp = ResizeArray()
    // int []
    let mutable ilistTProp  = Array.empty
    // int []
    let mutable icollectionTProp = Array.empty
    // unit -> DateTimeOffset
    let mutable dtFun = fun () -> System.DateTimeOffset.UtcNow
    // Convert between expressions: http://www.fssnip.net/ts/title/F-lambda-to-C-LINQ-Expression
    
    member _.IEnumerableTProp 
        with get() : System.Collections.Generic.IEnumerable<int> = enumerableTProp
        and set(v : System.Collections.Generic.IEnumerable<int>) = enumerableTProp <- v 
    
    member _.IEnumerableProp 
        with get() : System.Collections.IEnumerable = enumerableProp :> System.Collections.IEnumerable
        and set(v : System.Collections.IEnumerable ) = enumerableProp <- v |> Seq.cast

    member _.ArrayTProp 
        with get() : int[] = arrayTProp
        and set(v  : int[]) = arrayTProp <- v 

    member _.ArrayProp   
        with get() : System.Array = arrayProp :> System.Array
        and set(v : System.Array) = arrayProp <- v |> System.Linq.Enumerable.OfType<obj> |> Seq.toArray

    member _.ListTProp 
        with get() : System.Collections.Generic.List<int> = listTProp
        and set(v : System.Collections.Generic.List<int>) = listTProp <- v

    member _.ICollectionTProp 
        with get() : System.Collections.Generic.ICollection<int> = icollectionTProp :> System.Collections.Generic.ICollection<int>
        and set(v : System.Collections.Generic.ICollection<int>) = icollectionTProp <- v |> Seq.toArray

    member _.IListTProp 
        with get() : System.Collections.Generic.IList<int> = ilistTProp :> System.Collections.Generic.IList<int>
        and set(v : System.Collections.Generic.IList<int>) = ilistTProp <- v |> Seq.toArray

    member _.FuncProp 
        with get() : System.Func<System.DateTimeOffset> = System.Func<System.DateTimeOffset>(dtFun)
        and set(f : System.Func<System.DateTimeOffset>) = dtFun <- fun () -> f.Invoke()
```

## Breakdown

Well done for pushing past just copy pasting the code you need from above. We will go through the F# types and see what interfaces they implement, as well as if they have corresponding types in .NET BCL types.

### System.Collections.Generic.IEnumerable<_>

So as a `type`, [`seq<'T>` is an alias for `IEnumerable<'T>` in FSharp.Core](https://github.com/fsharp/fsharp/blob/3bc41f9e10f9abbdc1216e984a98e91aad351cff/src/fsharp/FSharp.Core/prim-types.fs#L3287).

```fsharp
// FSharp.Core
type seq<'T> = IEnumerable<'T>
```

If you are just getting started with F#, you may have noticed that it can be a lot more particular about it's types than C#. It can be easy to forget that this actually works. You can assign `'a list` or `'a array` to a `seq`.

```fsharp
let mutable ss = seq { 1; 2 }
ss <- [1;2]
ss <- [|1;2|]
```

This is because `seq` is `IEnumerable<'T>` and `'a list` and `'a array` implement `IEnumerable<'T>`

```fsharp
// FSharp.Core
type List<'T> = 
       | ([])  :                  'T list
       | (::)  : Head: 'T * Tail: 'T list -> 'T list
       interface System.Collections.Generic.IEnumerable<'T>
       interface System.Collections.IEnumerable
       interface System.Collections.Generic.IReadOnlyCollection<'T>
       interface System.Collections.Generic.IReadOnlyList<'T>
```

Finally, `'a array` is the same as an array in C#, and so also implements `IEnumerable`.

As it turns out, this gets us a very long way with interacting with C#, since `IEnumerable` and `IEnumerable<'T>` is pretty ubiquitous.

```fsharp
let csharp = CSharpyType()
csharp.IEnumerableTProp <- seq { 0..10 }
csharp.IEnumerableTProp <- [0..10]
csharp.IEnumerableTProp <- [|0..10|]
```

So, working with `IEnumerable<'T>` in F# is as simple as using `seq`.

### System.Collections.IEnumerable

For working with [System.Collections.IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable?view=netcore-3.1) we can make use of a function on the `Seq` module, `Seq.cast`. This takes an `System.Collections.IEnumerable` and turns it into a `seq`. Now it is in a more natural form for working with in F#.  
In terms of assigning, `'a seq`, `'a list`, and `'a array` can be assigned to it, since they all implement `IEnumerable`.

```fsharp
let csharp = CSharpyType()
csharp.IEnumerableProp <- seq { 0..10 }
csharp.IEnumerableProp <- [0..10]
csharp.IEnumerableProp <- [|0..10|]
```

It is worth noting we can also just use them in the usual constructs like:

```fsharp
for i in (csharp.IEnumerableProp) do 
    printfn "i: %A" i
```

### int []

With a typed array, we can just use `'T array` [since they are the same](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/arrays).

```fsharp
let csharp = CSharpyType()
csharp.ArrayTProp <- [|0..10|]
//csharp.ArrayTProp <- seq {0..10} // Compile error: This expression was expected to have type 'int []' but here has type 'seq<int>'
```

Make use of whatever you need from the `Array` module.

### System.Array

The above is still true when just using `System.Array`.

```fsharp
let csharp = CSharpyType()
csharp.ArrayProp <- [|0..10|]
```

When trying to assign an instance of this type to an F# value you will need to give it a `Type`. This can be done using a static method out of `Linq` to get us an `IEnumerable<'T>` ie `seq`, `arr |> System.Linq.Enumerable.OfType<obj>`. From there you can make use of the functions in the `Seq` module.

### System.Collections.Generic.List<_>

So it can be confusing initially since `list` in F# is not the same as `List<>` in C#. The equivalent of a [C# list in F# is `ResizeArray`](https://github.com/fsharp/fsharp/blob/3bc41f9e10f9abbdc1216e984a98e91aad351cff/src/fsharp/FSharp.Core/prim-types.fs#L3129).

```fsharp
// FSharp.Core
type ResizeArray<'T> = System.Collections.Generic.List<'T>
```

You can convert F# types to a `ResizeArray`.

```fsharp
csharp.ListTProp <- [0..10] |> ResizeArray
csharp.ListTProp <- [|0..10|] |> ResizeArray
csharp.ListTProp <- seq { 0..10 } |> ResizeArray
```

And of course remember that `List<'T>` implements `IEnumerable<'T>` and `ICollection<'T>`, which we will look at next.

### System.Collections.Generic.ICollection<_> & IList<_>

Remember that [array](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/generics-and-arrays) and [`List<'T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netcore-3.1) aka `ResizeArray` already implement `IEnumerable<'T>`, `ICollection<'T>`, and `IList<'T>`.

```fsharp
csharp.ICollectionTProp <- [|0..10|]
csharp.ICollectionTProp <- [|0..10|] |> ResizeArray

csharp.IListTProp <- [|0..10|]
csharp.IListTProp <- [|0..10|] |> ResizeArray
```

### ResizeArray

One thing you might be left wondering is converting from a ResizeArray, back to more natural F# types.

```fsharp
let resizeArr = [0..10] |> ResizeArray
let xs = resizeArr :> seq<_> // Implements IEnumerable<T> so we can just cast
let arr = resizeArr.ToArray() // ResizeArray / List<T> has a `ToArray` method. This is an O(n) activity.
let lst = xs |> Seq.toList // Once we have a seq, we can use Seq functions
```

### Bonus: System.Func<_>

Another kind of conversion I often find myself doing when working with C# APIs is with `Func` and F# functions. Converting a F# function to a `Func` is as simple as passing it into the `Func` constructor if need be. We can often simply assign a F# function to a `Func` and the compiler will handle that.

```fsharp
csharp.FuncProp <- (fun () -> System.DateTimeOffset.UnixEpoch)
let f = fun () -> csharp.FuncProp.Invoke()
```

## Conclusion

So that is my potted run through of converting between F# and C# types. This was meant to be more of a reference than a post that teaches or tells a story so I hope the lack of continuity was not too off-putting.