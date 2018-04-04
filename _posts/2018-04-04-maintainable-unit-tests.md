---
layout: post
title: "3 tips for more maintainable unit tests"
subtitle: "Avoid having to fix hundreds of tests whenever you make a code change"
description: "Lists some bad unit testing practices and some suggestions on how to make unit tests maintainable by being resilient to change."
permalink: maintainable-unit-tests
author: "Devon Burriss"
category: Programming
tags: [Clean Code, TDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/vents-bg.jpg"
social-img: "img/posts/2018/bridge-cables-500.jpg"
published: true
---
Although having a good collection of unit tests makes you feel safe and free to refactor, a bad collection of tests can make you scared to refactor. How so? A single change to application code can cause a cascade of failing tests. Here are some tips for avoiding (or fighting back) from that situation.
<!--more-->

## Tip 1: Test behavior not structure



- test behavior not structure
- use in-memory dependencies
- build up test tooling
