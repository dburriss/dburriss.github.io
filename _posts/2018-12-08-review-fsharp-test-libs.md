---
layout: post
title: "Review: F# unit testing frameworks and libraries"
subtitle: "A review of XUnit, FsUnit, Unquote, and Expecto"
description: "A review of XUnit, FsUnit, Unquote, and Expecto as unit testing libraries"
permalink: review-fsharp-test-libs
author: "Devon Burriss"
category: Software Development
tags: [F#,Testing,TDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/silhouette-bg.jpg"
social-img: "img/posts/2018/frame-500.jpg"
published: true
---
In this post I go through a few of the available assertion libraries and 2 test runners. We will look at running options, assertion style, and the clarity of the error messages.
<!--more-->

> This post is part of FsAdvent 2018. 

## Introduction

Before we get into reviewing some different options, let me introduce the the libraries and frameworks up for review and the criteria I will be looking at. One criteria you may expect here is speed. I will make some small observations on this at the end but I didn't see enough difference that I think it should be a factor.

### Frameworks

We will be looking at 2 frameworks: XUnit and Expecto. Some may disagree with me labeling them as frameworks. That is fine but it is useful to distinguish that both have components that allow you to write tests and hand that over to .NET tooling or Visual Studio to then run those tests. This is in contrast to the assertion libraries that are focused on the actual assertion of the outcome of a test.

#### XUnit

XUnit is a popular unit testing tool in the .NET space. It will be the baseline for a lot of the comparisons and is also necessary for the assertion libraries, as they are not test runners. 

#### Expecto

Expecto is a F# testing framework that does a lot. It has an API for running tests, test adapters for runners, assertions, performance tests, and integration with FsCheck for property based testing. In this post we will only be looking at the basic features of setting up tests and the assertions.

### Assertion libraries

Both **XUnit** and **Expecto** come with their own assertions. We will be looking at 2 other assertion libraries with different approaches. **FsUnit** brings an fun style to assertions that many like and **Unquote** makes use of a cool F# language feature to give detailed error messages.

### Criteria

When reviewing or comparing anything it is useful to have a concrete list of attributes that are compared.

- **Setup:** what options there are in terms of getting up and running
- **Style:** test setup style for the frameworks as well as the assertion style
- **Messages:** format of the error messages and comment on ease of parsing as well as the amount of detail in the message
- **Runners:** Running from Visual Studio and command line as well as filtering tests

## Review

So let's get into the comparison...

### Setup

The project used to test out the examples is [here on Github](https://github.com/dburriss/FsharpUnitTestFrameworks).

<div class="container">
  <div class="row hidden-xs hidden-sm">
    <div class="col-md-2"></div>
    <div class="col-md-5"><h4>XUnit</h4></div>
    <div class="col-md-5"><h4>Expecto</h4></div>
  </div>

  <div class="row">
    <div class="col-md-2"><b>Templates</b></div>
    <div class="col-md-5">
      <p> 
        .NET Core templating comes standard with an xUnit template. Visual Studio also has built in templates for XUnit.<br/>
        <code>dotnet new xunit -lang F#</code>
      </p>
    </div>
    <div class="col-md-5">
      <p> 
        You can install the Expecto template<br/>
        <code>dotnet new -i Expecto.Template::*</code><br/>
        <code>dotnet new expecto -n PROJECT_NAME -o FOLDER_NAME</code>
      </p>
    </div>
  </div>

  <div class="row">
    <div class="col-md-2"><b>Nuget</b></div>
    <div class="col-md-5">
      <ul> 
        <li><a href="https://www.nuget.org/packages/xunit/">xunit</a></li>
      </p>
    </div>
    <div class="col-md-5">
      <ul> 
        <li><a href="https://www.nuget.org/packages/Expecto/">Expecto</a></li>
      </ul>
    </div>
  </div>
  
  <div class="row">
    <div class="col-md-2"><b>VS Adapter</b></div>
    <div class="col-md-5">
      <ul> 
        <li><a href="https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/15.9.0">Microsoft.NET.Test.Sdk</a></li>
        <li><a href="https://www.nuget.org/packages/xunit.runner.visualstudio/">xunit.runner.visualstudio</a></li>
      </ul>
    </div>
    <div class="col-md-5">
      <ul> 
        <li><a href="https://www.nuget.org/packages/YoloDev.Expecto.TestSdk/">YoloDev.Expecto.TestSdk</a></li>
      </ul>
    </div>
  </div>

</div>

The only issue I had was discovering I had to use *YoloDev.Expecto.TestSdk* to get Visual Studio integration working instead of *Expecto.VisualStudio.TestAdapter* (as suggested in the documentation). Easy enough to discover by generating an example project using the template. So not much between them here other than XUnit being available out the box.

### Style

Let's look at how we setup a test in both XUnit and Expecto and then we will look at the assertion styles.

#### Test setup

**XUnit** looks for the `[<Fact>]` or `[<Theory>]` attribute on a function and will run that as a test.

```fsharp
[<Fact>]
let ``toEmail with bob gives bob [at] acme [dot] com`` () =
    let name = "bob"
    let expected = "bob@acme.com"
    let actual = toEmail name
    Assert.Equal (expected, actual)
```

> F# allows us to use the double &#96; to name a function with some special characters in it.

So we have the `[<Fact>]` attribute and a function with our test. 

Let's compare this to **Expecto** setup.

```fsharp
[<Tests>]
let aTest =
  test "toEmail with bob gives bob [at] acme [dot] com" {
      let name = "bob"
      let expected = "bob@acme.com"
      let actual = toEmail name
      Expect.equal actual expected "emails did not match"
  }
```

Expecto uses the `[<Tests>]` attribute to mark a value that contains tests, where the tests are defined in a F# computation expression called `test`.

Although this might seem quite similar, it is in fact quite different. This becomes more apparent if we have multiple tests. Where XUnit is just more functions with the attribute on, Expecto treats the tests more like data.

```fsharp
[<Tests>]
let emailtests = 
    testList "Email tests" [
            
        test "toEmail with null gives info [at] acme [dot] com" {
            let name = null
            let expected = "info@acme.com"
            let actual = toEmail name
            Expect.equal actual expected "emails did not match"
        }

        test "toEmail with bob gives bob [at] acme [dot] com" {
            let name = "bob"
            let expected = "bob@acme.com"
            let actual = toEmail name
            Expect.equal actual expected "emails did not match"
        }
    ]
```

Now we are defining our tests in a `List` given to a `testList`. Expecto [has an almost overwhelming number of ways to organize tests](https://github.com/haf/expecto#writing-tests). XUnit is simple and straightforward but if you find yourself wanting to take more control of how tests are organized, Expecto might be just what you want. This becomes even more important if you are using it to do property-based testing, performance tests, etc.

#### Assertions

Next we will look at the style of the assertions used by each library. 

<div class="container">
  <div class="row hidden-xs hidden-sm">
    <div class="col-md-3"><h4>XUnit</h4></div>
    <div class="col-md-3"><h4>FsCheck</h4></div>
  </div>

  <div class="row">
    <div class="col-md-3"><b class="visible-xs-block visible-sm-block">XUnit</b><code>Assert.Equal (expected, actual)</code></div>
    <div class="col-md-3"><b class="visible-xs-block visible-sm-block">FsUnit</b><code>actual |> should equal expected</code></div>
  </div>

   <div class="row hidden-xs hidden-sm">
    <div class="col-md-3"><h4>Unquote</h4></div>
    <div class="col-md-3"><h4>Expecto</h4></div>
  </div>

  <div class="row">
    <div class="col-md-3"><b class="visible-xs-block visible-sm-block">Unquote</b><code>test <@ actual = expected @></code></div>
    <div class="col-md-3"><b class="visible-xs-block visible-sm-block">Expecto</b><code>Expect.equal actual expected "null should be None"</code></div>
  </div>
</div>

**XUnit** is pretty standard if you come from an OO background, and it's OO roots are really showing here. Other than that it is easy enough to understand. XUnit's `Assert` static class contains a stack of useful assertion methods on it and since XUnit is very popular in the .NET space, it is easy finding answers.

**FsUnit** is for those that like a more fluent style (FP version) of defining assertions. If you are a C# developer and love the style of [FluentAssertions](https://fluentassertions.com/), then you may want to try this out. Honestly, I am not a fan of FluentAssertions library for its assertion style, I am a fan for its helpful error messages. In OO I prefer the more succinct XUnit style but use FluentAssertions because of its error messages. So if this is a style that appeals to you, try it out!

**Unquote** is slightly different as it uses F# quoted expressions (using `<@ expression @>`) to evaluate a plain statically typed F# expression and give detailed failure messages based on that evaluation. We will take at what this means for the error messages in the next section. There are some [assertion helpers](https://github.com/SwensenSoftware/unquote/wiki/UserGuide#assertions) but mostly you just write plain old F# expressions.

**Expecto** has its own assertion module `Expect` which has a bunch of functions available for asserting behavior. This is much akin to XUnit's `Assert` class except it doesn't carry the same OO legacy and so is much more functional in feel.

### Error message

Although a fan of TDD I prefer testing from the boundary of my application and only going as deep as needed. The less your clients (including your tests) know about the internals of your code, the more free you are to make changes without breaking any API contracts. So then error messages from your application become very important, and the more helpful your assertions are at surfacing this the better.

```fsharp
// XUnit / FsUnit / Unquote
[<Fact>]
let ``toEmail with bob gives bob [at] acme [dot] com`` () =
    let name = "bob"
    let expected = "bob@acme.com"
    let actual = toEmail name
    // XUnit
    Assert.Equal (expected, actual)
    // FsUnit
    actual |> should equal expected
    // Unquote
    test <@ actual = expected @>

// Expecto
test "toEmail with bob gives bob [at] acme [dot] com" {
    let name = "bob"
    let expected = "bob@acme.com"
    let actual = toEmail name
    Expect.equal actual expected "bob should be Some bob"
}
```

<div class="container">
  <div class="row hidden-xs hidden-sm">
    <div class="col-md-3"><h4>XUnit</h4></div>
    <div class="col-md-3"><h4>FsCheck</h4></div>
    <div class="col-md-3"><h4>Unquote</h4></div>
    <div class="col-md-3"><h4>Expecto</h4></div>
  </div>

  <div class="row">
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">XUnit</b>
Message: Assert.Equal() Failure
          ↓ (pos 0)
Expected: bob@acme.com
Actual:   info@acme.com
          ↑ (pos 0)
    </p></div>
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">FsUnit</b>
Message: FsUnit.Xunit+MatchException : Exception of type 'FsUnit.Xunit+MatchException' was thrown.
Expected: Equals "bob@acme.com"
Actual:   was "info@acme.com"
    </p></div>
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">Unquote</b>
"info@acme.com" = "bob@acme.com"
false
    </p></div>
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">Expecto</b>
Message: 
emails did not match.
Expected string to equal:
bob@acme.com
↑
The string differs at index 0.
info@acme.com
↑
String does not match at position 0. Expected char: 'b', but got 'i'.
    </p></div>
  </div>
</div>

So far there is very little between them. **Unquote** does stand out as different to the others. **FsUnit** has a bit more noise before the important part and doesn't point out the index of where things go wrong. That little detail could be helpful in spotting a small *tpyo* but other than that is not too significant.

Let's look at something with a functional concept in like `option`.

```fsharp
// XUnit / FsUnit / Unquote
[<Fact>]
let ``sanitize with bob gives Some bob`` () =
    let name = "bob"
    let expected = Some name
    let actual = Data.sanitize name
    // XUnit
    Assert.Equal (expected, actual)
    // FsUnit
    actual |> should equal expected
    // Unquote
    test <@ actual = expected @>

// Expecto
test "sanitize with bob gives Some bob" {
    let name = "bob"
    let expected = Some name
    let actual = Data.sanitize name
    Expect.equal actual expected "bob should be Some bob"
}
```

<div class="container">
  <div class="row hidden-xs hidden-sm">
    <div class="col-md-3"><h4>XUnit</h4></div>
    <div class="col-md-3"><h4>FsCheck</h4></div>
    <div class="col-md-3"><h4>Unquote</h4></div>
    <div class="col-md-3"><h4>Expecto</h4></div>
  </div>

  <div class="row">
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">XUnit</b>
Message: Assert.Equal() Failure
Expected: Some(bob)
Actual:   (null)
    </p></div>
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">FsUnit</b>
Message: FsUnit.Xunit+MatchException : Exception of type 'FsUnit.Xunit+MatchException' was thrown.
Expected: Equals Some "bob"
Actual:   was null
    </p></div>
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">Unquote</b>
None = Some "bob"
false
    </p></div>
    <div class="col-md-3"><p>
    <b class="visible-xs-block visible-sm-block">Expecto</b>
Message: 
bob should be Some bob. Actual value was &lt;null&gt; but had expected it to be Some "bob".
    </p></div>
  </div>
</div>

Again most are similar but **Unquote** begins to shine. All the other libraries print `None` as `null`.

### Runners

If you are using Visual Studio you probably want to run your tests from the Test Explorer in the IDE. This works fine for all the listed frameworks as you can see.

![Visual Studio Test Explorer](/img/posts/2018/test-explorer.jpg)

If the command line is more your thing, `dotnet test` works just fine. This example is a bit of a mess as it is running all the test libraries.

![Visual Studio Test Explorer](/img/posts/2018/console-tests.jpg)

#### Filtering

Sometimes it is useful to filter to run only certain tests. **XUnit** and `dotnet test` support this. With the following example you can filter down to just tests marked with this category using `--filter`.

```fsharp
[<Trait("Category", "Smoke")>]
[<Fact>]
let ``get list of numbers`` () =
    let expected = [|1;2;3|]
    let actual = Data.list |> Seq.take 3 |> Seq.toArray
    Assert.Equal<IEnumerable<int>>(expected, actual)
```

> `dotnet test --filter Category=Smoke`

Filtering with **Expecto** is a bit different. Remember how we assign a list of tests to a value? We could for example run our data tests like so by running them from a command line program.

```fsharp
// Program.fs

module Program = 
    
    open Expecto
    open TestFrameworks

    let [<EntryPoint>] main args = 
        runTestsWithArgs defaultConfig args ExpectoTests.datatests
```

And since this is just a normal console application, you can make it as simple or complex as needed. Now testing becomes a `dotnet watch run`.

## Conclusion

So that is our review of a few of the testing libraries available in the F# ecosystem. This is by no means comprehensive in terms of all libraries nor a deep dive into any of these. I do hope that if you have not used some of these, you did glimpse what some of them might offer.

## Resources

1. [xunit](https://xunit.github.io/)
1. [FsUnit](http://fsprojects.github.io/FsUnit/)
1. [Unquote](https://github.com/SwensenSoftware/unquote)
1. [Expecto](https://github.com/haf/expecto)
1. [Computation Expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions)
1. [Quoted expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/code-quotations)
1. [Filtering tests](https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests#xunit)