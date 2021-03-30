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
1. Use terms `Role`, `Permission`, and `Claims` as defined above in the code 

## Getting started

I will start off from where the [Auth0 getting started tutorial](https://auth0.com/docs/quickstart/webapp/aspnet-core/01-login) ends off. I won't repeat what is already covered there so if you want to follow along you can clone/fork the [Auth0 ASP.NET Core GitHub repository](https://github.com/auth0-samples/auth0-aspnetcore-mvc-samples/tree/master/Quickstart/01-Login).

## Approach

So as not to use strings we will define `enum`s to represent our permission parts of `Operation` and `Permission`. Typically, these are represented as a string in the format `operation:resource`. We will also define a few helper members (both instance and `static`) to help work with some `string` to type conversion. 
Roles and Permissions will be defined in static classes rather than a database. I have often received resistance to this idea but consider this; you will always need to add code to check for a specific operation/resource combination, so why not have type safety and define them only once? 

