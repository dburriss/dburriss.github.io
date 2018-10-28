---
layout: post
title: "How to F# - Part 7"
subtitle: "Working with collections"
description: "Diving into the super helpful collection functions available to manipulate collections in F#"
permalink: how-to-fsharp-pt-7
author: "Devon Burriss"
category: Software Development
tags: [Functional,F#,.NET]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/books-bg.jpg"
social-img: "img/posts/2018/books-500.jpg"
published: true
---
So after much threatening in past posts, we will finally be diving a little deeper into collections in F#. We will look at a few of the most commonly used functions on the collection modules by manipulating a `list` of people that we randomly generate.
<!--more-->

So lets go through a few common actions you would want to do on a collection. We will use `list` as an example through most of this post but what we learn applies to `array` and `seq` as well. Before we do that though let us briefly touch on the `map` type again.

## All the beautiful people (Creating data)

To work with lists we will need some data. Often data comes in the form of tables we need to join together. We will start simple though. Lets create 2 `map`s with numbers corresponding to the first names for the 1st and second names for the other.

| # | First name    | # | Last name |
|---|---------------|---|-----------|
| 1 | Sue           | 1 | Ali       |
| 2 | Bob           | 2 | Khan      |
| 3 | Neo           | 3 | Jacobs    |
| 4 | Fen           | 4 | Jensen    |
| 5 | An Si         | 5 | Wu        |
| 6 | Jan           | 6 | Lee       |

We will use these to generate a list of people later.

```fsharp
let fNames = [ (1, "Sue"); (2, "Bob"); (3, "Neo"); (4, "Fen"); (5, "An Si" ); (6, "Jan")] |> Map.ofList
let lNames = [ (1, "Ali"); (2, "Khan"); (3, "Jacobs"); (4, "Jensen"); (5, "Wu" ); (6, "Lee")] |> Map.ofList

// Map<int,string> -> Map<int,string> -> int -> string
let generateName fnames lnames i =
    let random = new System.Random(i) //don't new up  Random every time in a real app
    let fo = random.Next(1,6) // get a random number between 1 - 6
    let lo = random.Next(1,6) // get a random number between 1 - 6
    sprintf "%s %s" (Map.find fo fnames) (Map.find lo lnames)

// int -> string
let nameGen = generateName fNames lNames
```

We curry `generateName` with the `map`s of `fNames` (first names) and `lNames` (last names) transforming a function of signature `Map<int,string> -> Map<int,string> -> int -> string` into `int -> string`.

So calling `nameGen` will give us a random name like *"An Ali"* or *"Neo Jenson"*. Firstly we create 2 `map`s created from `list`s of `int * string` tuples using `Map.ofList`. In the `generateName` function we randomly get a first name and last name from the `map`s by using `Map.find` which has the signature of `'Key -> Map<'Key,'T> -> 'T`. Basically given a key and a `map`, it will return the value found at that key. Since we randomly generate the key, we get a random name each time.

## And there was light (Creating a list)

Although we can create a list with `[ expression ]` lets look at the `Map.init` function which has the signature `int -> (int - 'T) -> 'T list`. Lets break this down:

1. `int` - size of the list to create
1. `(int - 'T)` - a function that takes in the current position in the list being generated and returns an instance of type 'T to place at that position
1. `'T list` - the list that will be created of type `'T`

So we want to create a `Person list`. We need a function `int -> Person`. We curry in the `nameGen` to generate a `Person` with a randomly generated name.

```fsharp
type Person = { Id:int; Name:string }
// (int -> string) -> int -> Person
let generatePerson gen i = { Id = i; Name = gen(i) }
// int -> Person
let personGen = generatePerson nameGen

let people = List.init 10 personGen
```

So `people` will be a list of **10** `Person` instances.

```fsharp
[   
    { Id = 0; Name = "Wu Fen" }  
    { Id = 1; Name = "Bob Ali" }  
    { Id = 2; Name = "Fen Jacobs" }  
    { Id = 3; Name = "Bob Jensen" }
    { Id = 4; Name = "An Si Wu" }  
    { Id = 5; Name = "Bob Khan" }
    { Id = 6; Name = "An Si Jacobs" }  
    { Id = 7; Name = "Bob Wu" }  
    { Id = 8; Name = "An Si Ali" }  
    { Id = 9; Name = "Neo Jensen" }
]
```

## These are not the elements you are looking for (Finding an element)

Now that we have a list, lets see how we work with it. A common need while programming is to find an element in a collection.

```fsharp
let bob = people |> List.find (fun p -> p.Name.StartsWith("Bob"))
```

We use `List.find` which has the signature `('T -> bool) -> 'T list -> 'T`. In our case that would be a function `(Person -> bool)` that returns `true` if it is the element you are looking for. Now this is all good and well if there is a "Bob" in the list. But it is a randomly generated collection of names, what if we want to find a specific "Bob" and he isn't in the list?

```fsharp
let bob = people |> List.find (fun p -> p = "Bob Khan")
```

Assuming you do not have a "Bob Khan" in your list, you will get an exception thrown.

> System.Collections.Generic.KeyNotFoundException: An index satisfying the predicate was not found in the collection.

Remember in [a previous post](/how-to-fsharp-pt-6) we dealt with handling cases when there is no data using `option`. Well this is one of those times. Lets use a very similar function to `List.find` called `List.tryFind` that has the signature `('T -> bool) -> 'T list -> 'T option`.

```fsharp
let maybeBob = people |> List.tryFind (fun p -> p.Name = "Bob Khan")
```

> val maybeBob : Person option = Some {Id = 5; Name = "Bob Khan";}
> OR
> val maybeBob : Person option = None

So depending on whether the `list` contains someone named "Bob Khan" the function will return `Some` or `None`.

## Take what you need (Filtering a list)

Sometimes we are not looking for a specific element but multiple elements. Maybe we are looking for elements that match some criteria or are wanting to exclude based on something. Either way we are wanting to filter the collection. For `list`s we use the `List.filter` function which has the signature `('T -> bool) -> 'T list -> 'T list`.

```fsharp
let bobs = people |> List.filter (fun p -> p.Name.StartsWith("Bob"))
```

So given a function that returns `true` if the element should be in the `list`, you will get a new `list` with the matching elements in it.

```fsharp
[   
    { Id = 1; Name = "Bob Ali" }  
    { Id = 3; Name = "Bob Jenson" }  
    { Id = 5; Name = "Bob Khan" }  
    { Id = 7; Name = "Bob Wu" }
]
```

So in my `list`, 4 out of 10 people had a first name of "Bob".

## A change is as good as a holiday (Working with list elements)

Imagine we have our collection of people but a request comes in that the names be in the format *Surname, First Names*.
First things first, lets write a function `leadingLastName` that will take in *"Neo Jensen"* and transform it to *"Jensen, Neo"* and *"An Si Ali"* to *"Ali, An Si"*.

```fsharp
// char -> string -> string[]
let split (sep:char) (s:string) = s.Split([|sep|])

// string -> string
let leadingLastName (name:string) = 
    let lastNameToFront (names:string array) = 
        match names with
        | [||] -> ""
        | [|x|] -> x
        | [|x;y|] -> String.concat ", " ([|y;x|])
        | _ -> [|yield ([Array.last names;","] |> String.concat ""); for i=0 to ((Array.length names)-2) do yield names.[i] |] |> String.concat " "

    name |> split ' '
    |>lastNameToFront
```

This uses `match` to pattern match on the `array`. Lets break it down quickly:

1. `[||] -> ""` - Matches when `array` is empty: return name is empty
1. `[|x|] -> x` - Matches when name is a single element: name is a single name like "Cher"
1. `[|x;y|] -> String.concat ", " ([|y;x|])` - Matches when `array` is 2 elements: name and surname so swaps and adds a ,
1. `_` - this one is quite complex but basically it moves the last element to the front and adds a , after it

Next we will use `leadingLastName` with `List.map` which has the signature `('T -> 'U) -> 'T list -> 'U list`. We have seen `map` (the function not the type) before when we learned about `option`. Although `map` can map from a value to a value of any other type, in that case we went from `string -> string` with name to email. In this case we will also go from `string` to `string`. Just remember you can map to different types.

```fsharp
let withLeadingLName =  people |> List.map (fun p -> {p with Name = (leadingLastName p.Name)})

[
    { Id = 0; Name = "Wu, Fen" }  
    { Id = 1; Name = "Ali, Bob" }  
    { Id = 2; Name = "Jacobs, Fen" }  
    { Id = 3; Name = "Jenson, Bob" }  
    { Id = 4; Name = "Wu, An Si" }
    ...
]
```

We supplied `map` an inline function `(fun p -> {p with Name = (leadingLastName p.Name)})` that takes a `Person` and uses `leadingLastName` to return a new `Person` with the name changed.

## Get it sorted (sorting elements)

Often we care about the order of the elements in a collection. We can use one of the sorting functions to get a new sorted `list` back. `List.sortBy` has the signature `('T -> 'Key) -> 'T list -> 'T list`.

```fsharp
let sorted = withLeadingLName |> List.sortBy (fun p -> p.Name)
[
    { Id = 8; Name = "Ali, An Si" }  
    { Id = 1; Name = "Ali, Bob" }  
    { Id = 6; Name = "Jacobs, An Si" }  
    { Id = 2; Name = "Jacobs, Fen" }  
    { Id = 9; Name = "Jacobs, Neo" }  
    ...
]
```

So we pass it a function to determine what to sort by and then the list and we will get back the sorted list, in this case by the `Name` `string`.

## Family business (Grouping)

What if our next task was to group the people by their last name? Well with the built in `list` functions this is very simple.

```fsharp
// Person -> string
let getLastName person = person.Name |> split ',' |> Array.head
let groupedByLName = withLeadingLName |> List.groupBy getLastName
[
    ("Wu", [{Id = 0; Name = "Wu, Fen";}; {Id = 4; Name = "Wu, An Si";}; {Id = 7; Name = "Wu, Bob";}]);
    ("Ali", [{Id = 1; Name = "Ali, Bob";}; {Id = 8; Name = "Ali, An Si";}]);
    ...
]
```

We use the `List.groupBy` function which has the signature `('T -> 'Key) -> 'T list -> ('Key * 'T list) list`. Lets break that down.

1. `('T -> 'Key)` - a function that will take an element from the `list` and return a key to group by. In our case it should take a `Person` and return their last name.
1. `'T list` - the original list that needs grouping
1. `('Key * 'T list) list` - a list of tuples where the first element of the tuple is the **key** and the second is a list of elements that matched with that key

## Less random

As a final demonstration, lets look at a less used function of `list`. At the beginning of this post we had 2 `list`s with the names. What if we didn't care about it being random? What if we just joined the 1st first name to the 1st last name and continued on like that down the `list`s.

```fsharp
let fNames = [ "Sue"; "Bob"; "Neo"; "Fen"; "An Si" ; "Jan"]
let lNames = [ "Ali"; "Khan"; "Jacobs"; "Jenson"; "Wu"; "Lee"]
let names = List.zip fNames lNames |> List.map (fun (fname,lname) -> sprintf "%s, %s" lname fname)
```

> val names : string list = ["Ali, Sue"; "Khan, Bob"; "Jacobs, Neo"; "Jenson, Fen"; "Wu, An Si"; "Lee, Jan"]

We used the `List.zip` function that takes 2 lists and zips them together into a `list` of tuples made up of an element from the 1st `list` and and element from the second.

## Conclusion

So we finally got to dive into working with collections. In this post you learned how to create, map, sort, group, and even zip a `list`. Remember that the functions we worked with here are also available on the `Array` and `Seq` modules.

In a coming post we will be dealing with error handling.

## Resources

1. [Lists](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lists)
1. [For fun and profit List module functions](https://fsharpforfunandprofit.com/posts/list-module-functions/)

## Credits

1. Background image by [Jack Reichert](https://unsplash.com/@jackreichert)
1. Social image by [Patrik Göthe](https://unsplash.com/@p)