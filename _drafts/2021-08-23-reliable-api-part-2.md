---
layout: post
title: "Reliable APIs - Part 2"
subtitle: "Exploring reties, retry implications, and the failure modes they are appropriate for"
description: "Takes a look at using retry policies to increase the reliability of calls to APIs as well as the endpoint internals. This post looks at how retry policies can go wrong and walks through analysing for proper use."
permalink: reliable-apis-part-1
author: "Devon Burriss"
category: Software Development
tags: [Distributed Systems,API Design,Reliability,Idempotent]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2019/target-500.jpg"
published: true
---

- increased traffic

## That idempotence thing

So you stopped using XML and SOAP and started sending JSON so you figured you had this REST stuff down. You do recall this idea of *idempotent* calls though and this seems like what you are looking for. Searching for solutions, the internet seems to be a dumpster fire of people arguing about whether POST should be idempotent or not. Reading the POST section of the [RFC](https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.3) you decide to on:

- Respond with `201 Created` if the resource does not exist
- Respond with `303 See Other` if the resource already exists

So apparently a POST can be idempotent. Regardless, it seems like a good idea.

- idemotency-key in a cache (users double click and 2nd request occurs before cache is updated)
- move to client-generated id in a DB + back-channel to a cache
- outbox

The more difficult question is, how to tell if a request is a duplicate? Apparently, the semantic way to handle this would be to use [Idempotency-Key](https://tools.ietf.org/id/draft-idempotency-header-01.html). 


next

- double click

## Summary

**Problem:** Duplicate calls

**Solutions:** idempotency via a response cache

**Consequence:** Duplicate calls because cache update is not atomic

## Resources

- https://stripe.com/blog/idempotency
- https://repl.ca/what-is-the-idempotency-key-header/
- https://www.techyourchance.com/client-generated-ids-vs-server-generated-ids/
- https://tech.trello.com/sync-two-id-problem/