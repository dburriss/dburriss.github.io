---
layout: post
title: "Creating a Prometheus parser: Fennel"
subtitle: "A quick tour of using FParsec to write a Prometheus parser"
description: "This post shows some of the results both in text parsing and end Promethean result of creating a parser."
permalink: prometheus-parser-fennel
author: "Devon Burriss"
category: Software Development
tags: [F#,Prometheus,Fennel,FsAdvent]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/dashboard-bg.jpg"
social-img: "img/posts/2020/flame-500.jpg"
published: true
---
A year back I ran into the need for a library that provided a model for creating valid Prometheus log lines. The libraries I looked at sent these metrics for export rather than giving me access to the model or allowing me to create the corresponding log string. I had been wanting to play around with FParsec for a while so this seemed like a perfect opportunity to give it a try.
<!--more-->

> This post is part of [FsAdvent 2020](https://sergeytihon.com/2020/10/22/f-advent-calendar-in-english-2020/).

The result was a library called [Fennel](https://github.com/dburriss/fennel). It can parse Prometheus text to objects, and turn these metric objects into valid Prometheus text.

This was my first time using a library to do a custom parser. In the past when I had needed to parse text I had used a state machine and consumed a character at a time. The idea here is that depending on the state, you expect certain characters to follow. See [here](https://stackoverflow.com/questions/50896567/fsharp-sequence-processing-with-state/50918243#50918243) for an example. It turns out this is not too different to how you use a library like FParsec.
Although there is a bit of a learning curve, and not many resources outside of the docs, using [FParsec](http://www.quanttec.com/fparsec/) was fun. I am sure there are 100 ways I could improve the parser (feedback welcome...preferably be polite) but I am happy with the end result.

## FParsec

This post is not meant to be a tutorial on FParsec but in-case you have never used it, let's look at some of the things it allows you to do.

FParsec gives you a boatload of parsers that can be combined to make a new parser. Parser factory functions like `satisfy` will give you back a `Parser<>` that satisfies the given predicate. The library also gives you some operators. Below `<|>` means try the first parser, if that fails, try the second.

The example below also uses `manyChar2` which uses the first parser for the first char and then the next for all following chars. In this case, because a Prometheus metric name must start with an ASCII letter or an underscore (not a number).

```fsharp
let underscoreOrColon = satisfy (fun c -> c = '_' || c = ':')
let ascii_alpha_numeric = (asciiLetter <|> digit)
let pname = manyChars2 (asciiLetter <|> underscoreOrColon) (ascii_alpha_numeric <|> underscoreOrColon)
```

These parsers can then be combined in other ways. The code below combines the `pname` parser with the "zero or more" whitespace parser but because the period is on the left of the `.>>` operator it takes only the result of `pname` (`.>>` and `.>>.` are available). The `|>>` operator returns a parser that takes the result of the parser to the left and applies the function to the right.

```fsharp
let private metric_name = pname.>> ws0 |>> MetricName
```

This is just a tiny taste of how you can build up a complex parser from simpler ones. Combining these you can start to build up a grammar for your parsers. Next we look at building this further with our Prometheus parser.

## Prometheus parser

As a reminder, the Prometheus format is a text-based format.

```text
# This is a comment but the following 2 have meaning
# HELP http_requests_total The total number of HTTP requests.
# TYPE http_requests_total counter
http_requests_total{method="post",code="200"} 1027 1395066363000
http_requests_total{method="post",code="400"}    3 1395066363000
```

You can read up on the exposition format [here](https://prometheus.io/docs/instrumenting/exposition_formats/).

The model looks like this:

```fsharp
// details and types excluded for brevity
type MetricLine = {
    Name : MetricName
    Labels : Label list
    Value : MetricValue
    Timestamp : Timestamp option
}

type Line =
    | Help of (MetricName*DocString)
    | Comment of string
    | Type of (MetricName*MetricType)
    | Metric of MetricLine
    | Blank
```
You can see the full model on [the GitHub repository](https://github.com/dburriss/fennel/blob/master/src/Fennel/Model.fs)

Any Prometheus log line can be *Help* information, a normal *comment*, *type information*, a metric, or a blank line. From a parsing point of view, I categorize comments, TYPE line, and HELP line all as comments since the `#` is a common first character. This is not reflected in the model.

So let's break down the Prometheus text and how it relates to the model above.

1. A line in some Prometheus text can be *blank* for a Prometheus log *line*
1. A Prometheus log line can be a *comment* or a *metric*
1. A comment can be a just a normal *comment*, *TYPE* information, or *HELP* information
1. A *metric* requires a *name* and *value*
1. A metric can optionally have *labels* and a *timestamp*

Let's look at a few select parsers and see how they match with our description above. We will focus on the comment line of TYPE and how it fits in.

```fsharp
// TYPE from point 3
let typeLine = (``TYPE``>>.metric_name.>>.metric_type) |>> Line.Type
let comment = comment_prefix >>.ws0 >>.(typeLine <|> helpLine <|> commentLine)
// Point 2
let line = ws0 >>.(comment <|> metric)
// Point 1
ws0 >>.(line <|> emptyLine)
```

## Fennel

So that was a little under the hood of Fennel. What does it look like from the outside?

We can turn Prometheus log text into strongly typed models.

```fsharp
open Fennel

let input = """
# Finally a summary, which has a complex representation, too:
# HELP rpc_duration_seconds A summary of the RPC duration in seconds.
# TYPE rpc_duration_seconds summary
rpc_duration_seconds{quantile="0.01"} 3102
rpc_duration_seconds{quantile="0.05"} 3272
rpc_duration_seconds{quantile="0.5"} 4773
rpc_duration_seconds{quantile="0.9"} 9001
rpc_duration_seconds{quantile="0.99"} 76656
rpc_duration_seconds_sum 1.7560473e+07
rpc_duration_seconds_count 2693
"""

let lines = Prometheus.parseText input
```

Each of these lines can match a specific line type:

```fsharp
match line with
| Help (name, doc) -> printfn "Help line %A" (name, doc)
| Comment txt -> printfn "Comment line %s" txt
| Type (name, t) -> printfn "Type line %A" (name, t)
| Metric m -> printfn "Metric line %A" m
| Blank -> printfn "Blank line"
```

And we can create an object that represents a Prometheus log line.

```fsharp
open Fennel

let prometheusString = Prometheus.metric "http_requests_total" 1027. [("method","post");("code","200")] DateTimeOffset.UtcNow
```

## Conclusion

So that was a little peek into creating my first parser. 
Have you used FParsec? If not, was this helpful?  
Do you have plenty of experience with it? What can I improve?  
Leave a comment or create an issue or PR on the repo.

<span>Photo by <a href="https://unsplash.com/@_ggleee?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText">Gleb Lukomets</a> on <a href="https://unsplash.com/s/photos/flame?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText">Unsplash</a></span>