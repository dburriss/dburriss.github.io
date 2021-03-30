---
layout: post
title: "Easy contract tests"
subtitle: "An easy 80/20 way of doing contract tests"
description: ""
permalink: easy-contract-tests
author: "Devon Burriss"
category: Software Development
tags: [Testing,Contract Testing]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2019/target-500.jpg"
published: true
---
Contract testing can get quite involved and there is plenty to get into on the topic. In this post I want to offer a really simple type of test that can often get you [80% of the benefits with 20% of the work](https://en.wikipedia.org/wiki/Pareto_principle). I will demonstrate writing a very simple test and describe the method you can use in your organization to apply it.
<!--more-->
If you would like a deep-dive into contract testing I recommend [this post on MartinFowler.com](https://www.martinfowler.com/articles/consumerDrivenContracts.html).

## The Problem

For me contract testing is about communication. More specifically, it is about "What do you mean when you tell me something?". With any communication there are at least 2 **collaborators**. Consider an HTTP request from client to server.