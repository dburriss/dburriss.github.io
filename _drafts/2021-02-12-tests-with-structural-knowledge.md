---
layout: post
title: "Tests with structural knowledge"
subtitle: "When tests inhibit change instead of enabling it"
description: ""
permalink: tests-with-structural-knowledge
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2019/target-500.jpg"
published: true
topics: [engineering-practices]
keywords: [Software Development, Testing, Contract Testing]
---

---
In this post I wanted to focus on the idea of tests knowing about the structure of your code. Tests know about the structure of your code means that the tests need to change if that structure changes. This is a bad place to be in since your tests are what you rely on to catch issues when you make changes to your code.
<!--more-->

- each tests knows about creation of instances
- Exercising the system depends on lots of internals
- Law of Demeter for asserts

