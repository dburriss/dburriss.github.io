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
published: true
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
        `dotnet new xunit -lang F#`
      </p>
    </div>
    <div class="col-md-5">
      <p> 
        You can install the Expecto template<br/>
        `dotnet new -i Expecto.Template::*`<br/>
        `dotnet new expecto -n PROJECT_NAME -o FOLDER_NAME`
      </p>
    </div>
  </div>

  <div class="row">
    <div class="col-md-2"><b>Nuget</b></div>
    <div class="col-md-5">
      <ul> 
        <li>[xunit]()</li>
      </p>
    </div>
    <div class="col-md-5">
      <ul> 
        <li>[Expecto]()</li>
      </ul>
    </div>
  </div>
  
  <div class="row">
    <div class="col-md-2"><b>VS Adapter</b></div>
    <div class="col-md-5">
      <ul> 
        <li>[Microsoft.NET.Test.Sdk]()</li>
        <li>[xunit.runner.visualstudio]()</li>
      </ul>
    </div>
    <div class="col-md-5">
      <ul> 
        <li>[YoloDev.Expecto.TestSdk]()</li>
      </ul>
    </div>
  </div>

</div>

The only issue I had was finding out to use *YoloDev.Expecto.TestSdk* to get Visual Studio integration working instead of *Expecto.VisualStudio.TestAdapter*.

### Style

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
