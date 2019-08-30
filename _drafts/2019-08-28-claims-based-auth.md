---
layout: post
title: "Claims based authorization in ASP.NET Core - PART 1"
subtitle: "A type driven approach to resource claims using actions and resources and policies"
description: "An example of using strong typing, attributes, and ASP.NET Core policies to do resource based authorization"
permalink: asp-net-core-policy-auth-1
author: "Devon Burriss"
category: Software Development
tags: [ASP.NET Core,Auth0,Authentication,Authorization,C#,Security]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/explore-590.jpg"
published: false
---
At work this month I got to spend a day doing a quick proof of concept of locking down an API by action on a resource. Although I have done this a few times before, this was the 1st time I really got to dive into the new [ASP.NET Core policies](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-2.2). ASP.NET Core policies are a new addition from previous ASP.NET versions and add a real flexibility that was missing from authorization before this. 
<!--more-->

## Defining terms

- Permission = the right to perform a specific operation on a resource.
- Claim = a specific claim a user makes about their identity or permissions in the system. This could be name, email address, role, or a permission to do something against a resource.
- Permission claim = a permission (represented as `operation:resource` claim)
- Role claim = a role a user has used for information or display purposes with the form. Used for grouping permissions together for easier management.

## Goals

Having done this before I had a few goals I wanted to achieve.

1. Avoid `string` based configuration as much as possible
1. Avoid configuring permissions in multiple places

## Getting started

So I