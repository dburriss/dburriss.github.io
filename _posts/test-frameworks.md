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
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/explore-590.jpg"
published: false
---
In this post I go through a few of the available assertion libraries and 2 test runners. We will look at running options, assertion style, and the clarity of the error messages.
<!--more-->
## Introduction

Before we get into reviewing some different options, let me introduce the the libraries and frameworks up for review and the criteria I will be looking at. One criteria you may expect here is speed. I will make some small observations on this at the end but I didn't see enough difference that I think it should be a factor.

### Frameworks

We will be looking at 2 frameworks: XUnit and Expecto. Some may disagree with me labeling them as frameworks. That is fine but it is useful to distinguish that both have components that allow you to write tests and hand that over to .NET tooling or Visual Studio to then run those tests. This is in contrast to the assertion libraries that are focused on the actual assertion of the outcome of a test.

#### XUnit

XUnit is a popular unit testing tool in the .NET space. It will be the baseline for a lot of the comparisons and is also necessary for the assertion libraries, as they are not test runners. 

#### Expecto

Expecto is a F# testing framework that does a lot. It has an API for running tests, test adapters for runners, assertions, performance tests, and integration with FsCheck for property based testing. In this post we will only be looking at the basic features of setting up tests and the assertions.

### Criteria

When reviewing or comparing anything it is useful to have a concrete list of attributes that are compared.

- **Setup:** what options there are in terms of getting up and running
- **Style:** test setup style for the frameworks as well as the assertion style
- **Messages:** format of the error messages and comment on ease of parsing as well as the amount of detail in the message
- **Runners:** Running from Visual Studio and command line as well as filtering tests

## Review

So let's get into the comparison...

### Setup

The project used to test out the examples is [here on Github]().

<div class="container">
  <div class="row">
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
        <li><a href="">xunit</a></li>
      </p>
    </div>
    <div class="col-md-5">
      <ul> 
        <li><a href="">Expecto</a></li>
      </ul>
    </div>
  </div>
  
  <div class="row">
    <div class="col-md-2"><b>VS Adapter</b></div>
    <div class="col-md-5">
      <ul> 
        <li><a href="">Microsoft.NET.Test.Sdk</a></li>
        <li><a href="">xunit.runner.visualstudio</a></li>
      </ul>
    </div>
    <div class="col-md-5">
      <ul> 
        <li><a href="">YoloDev.Expecto.TestSdk</a></li>
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

<div class="container">
  <div class="row">
    <div class="col-md-3"><h4>XUnit</h4></div>
    <div class="col-md-3"><h4>FsCheck</h4></div>
    <div class="col-md-3"><h4>Unquote</h4></div>
    <div class="col-md-3"><h4>Expecto</h4></div>
  </div>

  <div class="row">
    <div class="col-md-3"><h4>XUnit</h4></div>
    <div class="col-md-3"><h4>FsCheck</h4></div>
    <div class="col-md-3"><h4>Unquote</h4></div>
    <div class="col-md-3"><h4>Expecto</h4></div>
  </div>
</div>


### Error message

### Runners

- pics of vs
- commands and pics of console
- commands for filtering

## Resources

1. [xunit](https://xunit.github.io/)
1. [FsUnit](http://fsprojects.github.io/FsUnit/)
1. [Unquote](https://github.com/SwensenSoftware/unquote)
1. [Expecto](https://github.com/haf/expecto)
1. [Computation Expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions)