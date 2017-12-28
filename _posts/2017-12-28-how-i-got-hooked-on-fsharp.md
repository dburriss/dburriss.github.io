---
layout: post
title: "Why I got hooked on FSharp"
subtitle: "My path to embracing functional programming"
description: "A short overview of my ongoing efforts to learn functional programming, specifically F#"
permalink: how-i-got-hooked-on-fsharp
author: "Devon Burriss"
category: Programming
tags: [Functional]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/tree-bg.jpg"
social-img: "img/posts/2017/footprint.jpg"
published: false
---

I have been asked a few times "how I got started with F#?" as more than a few people have found it difficult. I myself had a few false starts with it. It looked weird, I didn't know where or how to start, it was too different to OO with C style languages, and the tooling just was not as slick. I honestly think a better question is "Why did I start using F#?"
<!--more-->

## The WHY of it

As I have matured as a developer I have come to appreciate coding practices that constrain my options in a way that minimizes potential errors. An infinitely flexible design is also one the allows all possible errors, known and unknown. Constraining future developers to "make illegal states unrepresentable". I sometimes say "code like the future developer on this is an idiot because current you is an idiot and future you will be to". To be clear, I say that to myself, about myself.

In OO we do this with constructors or factories (and hidden constructors), with encapsulation and smart APIs. This is a big part of the guidelines around aggregates in Domain-Driven design (DDD) and keeping the aggregate consistent. We have a lot of patterns and practices in OO that help with this. A LOT! In fact it is quite difficult for new developers to get up to speed with them all. And since they are often struggling with the technical implementation of features they are not worrying too much about the intricacies of the design and whether it leads future developers into the pit of success. We coach, and hopefully with good coaching they learn these things faster than we did through trial and error. I cannot help but wonder if there is a simpler way to get to well designed software than absorbing all these patterns and practices? Note I said simple, not easy.

Functional programming (FP) with its mathematical basis makes some claims about correctness. Correctness is hard to be certain of when global state is constantly in flux. FP revolves around functions, with inputs and outputs, and then same input always yielding the same output (for pure functions).

So basically the WHY can be broken down into 2 points:

- Correctness of the program
- Fewer concepts need to be known to develop maintainable software

I remember reading [this article of Mark Seemann's](http://blog.ploeh.dk/2015/04/13/less-is-more-language-features/) and thinking this seems like a problem I have but I cannot quite relate to his conclusion. As we will see in the next section, it took me 2 years to get to a place where I could read that article and nod my head instead of scratch it.

## The HOW of it

I was not keeping notes so these are the highlights I remember and that I think are important.

Since about 2013 I had trying to learn and apply many of the technical approaches highlighted in DDD. This lead to much more focus on types whose instance state can only be changed in a very controlled way. Not only that but the types are descriptive of the domain and do not try be too reuseable but rather represent very specific use cases.

By the time 2016 rolled around I had heard of the promises FP made and had even "file new project"'ed an F# console application but with very little success. I resolved to give it a better try and started reading through [fsharpforfunandprofit](https://fsharpforfunandprofit.com/books/#downloadable-ebook-of-this-site) and looking at a few [Pluralsight](https://www.pluralsight.com/search?q=F%23&categories=course) videos.

Then I was contacted by [Manning](https://www.manning.com/) to give feedback on an early draft of [Functional Programming in C#](https://www.manning.com/books/functional-programming-in-c-sharp). In it Enrico Buonanno gives a really deep introduction to functional concepts and patterns, showing both the implementation and usage of FP in C#. For me this was quite nice as I could absorb concepts without getting hung up on the syntax of some new programming language. These inspired a series of posts on Honest Types, namely [Honest Argument](http://devonburriss.me/honest-arguments/), [Honest Return Types](http://devonburriss.me/honest-return-types/), and [Better Error Handling](http://devonburriss.me/better-error-handling/).

At work my code started taking on a more functional style in C# and a few of our projects started making use of [Language Extensions](https://github.com/louthy/language-ext). I have a repository demonstrating some use cases [here](https://github.com/dburriss/ElevatedExamples).

By early 2017 I was writing small console apps in F# that would crunch some CSV files, or merge some PDF documents. These were not great and I realized that although I was getting used to F# syntax I was missing something key in how to structure my applications. The penny only dropped when watching a video from Mark Seemann on [Functional architecture - The pits of success](https://www.youtube.com/watch?v=US8QG9I1XW0). Another good one released later is [From Dependency injection to dependency rejection](https://www.youtube.com/watch?v=cxs7oLGrxQ4). Both of these talk about purity and composing applications so the code with dependencies on IO are on the outside. If this sounds like Clean/Onion/Hexagonal Architecture, you are absolutely right.

Now here we are at the end of 2017 and and I have just finished [Domain Modelling Made Functional](https://fsharpforfunandprofit.com/books/#domain-modeling-made-functional-ebook-and-paper) by Scott Wlaschin of [fsharpforfunandprofit](https://fsharpforfunandprofit.com/) fame. It brings together so many deep topics in such an approachable way that it is difficult to compare to any book I have read before. It doesn't assume any knowledge and yet I learned some F#, some FP, and some DDD even though I have read multiple books dedicated to each of these topics. Scott develops a feature from beginning to end in a practical way that distills and teaches the core concepts of these advanced topics without getting bogged down in theory. I realize I am sounding like a fan boy here but I would honestly recommend this book to teach FP and F# OR DDD. It teaches both brilliantly.

This December I posted [my first F# themed blog post] as part of the [FsAdvent Calendar 2017](https://sergeytihon.com/2017/10/22/f-advent-calendar-in-english-2017/). I submitted [my first PR to an F# open source project](https://github.com/giraffe-fsharp/giraffe-template/pull/4) and now I am winding down on my 2nd FP related blog post. I am looking forward to what the next year brings and all I have to learn.


## Further Reading (posts)

1. Mark Seemann has a brilliant posts on how a [language can reduce the potential for errors](http://blog.ploeh.dk/2015/04/13/less-is-more-language-features/)
1. Scott Wlaschin on [learning F#](https://fsharpforfunandprofit.com/learning-fsharp/)

## Further watching (videos)

1. Mark has an excellent talk on [falling into the pit of success](https://www.youtube.com/watch?v=US8QG9I1XW0) and another on [Dependency Rejection](https://www.youtube.com/watch?v=cxs7oLGrxQ4)
1. [Designing with Capabilities](https://vimeo.com/162209391)
1. [Railway oriented programming](https://vimeo.com/113707214)

## Recommended books

1. [Domain Modelling Made Functional](https://fsharpforfunandprofit.com/books/#domain-modeling-made-functional-ebook-and-paper)
1. [fsharpforfunandprofit](https://fsharpforfunandprofit.com/books/#downloadable-ebook-of-this-site)
1. [Functional Programming in C#](https://www.manning.com/books/functional-programming-in-c-sharp)
1. [Real-World Functional Programming](https://www.manning.com/books/real-world-functional-programming)

## Credits

1. Header photo by [John Mark Arnold]https://unsplash.com/@johnmarkarnold)
1. Social photo by [Michał Grosicki](https://unsplash.com/@groosheck)
