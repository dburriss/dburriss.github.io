---
layout: post
title: "Anatomy of an automated test suite"
subtitle: "Looking at the parts and functions of various automated test types"
description: "Comparing structure and function of various test types to tease out what we really care to verify"
permalink: anatomy-of-automated-testing
author: "Devon Burriss"
category: Programming
tags: [Testing,ATDD,BDD,TDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/pyramids-bg.jpg"
social-img: "img/posts/2018/pyramid-500.jpg"
published: true
---
Unit, integration, end-to-end, acceptance, UI tests, and more. With so many types of automated tests is it no wonder that we so often disagree on whether something is an acceptance test or an integration test? Or maybe an end-to-end test? What if instead of thinking about the structure of the test, what it tested, we instead considered the question that the test is answering...
<!--more-->

## A quick note on the test pyramid

For some reason the test pyramid comes up when talking about what tests to write. The test pyramid gives us an indication of the relative return on investment of writing certain types of tests. The cost of writing and maintaining UI tests is usually quite high, thus diminishing their value as a verification of correctness. Unit tests on the other hand should be pretty quick to write, easy to maintain, and so give more value over UI tests. Therefore we should have a relatively large number of unit tests compared to UI tests.

In this image I use *Service* to encompass integration, end-to-end, acceptance tests, etc.

![Test Pyramid](/img/posts/2018/test-pyramid.jpg)

If we had a way that made [UI tests easy to write and maintain](/page-module-model/), they would switch places in the test pyramid. They might then provide more value relative to the cost of creation and maintenance.

## Practices around testing

Let us take a look at some of the testing practices around and what they focus on. This will give us a good indication of what questions we can ask of our tests.

First up we have **Test-Driven Development**. A lot can be written about TDD and half of it would be disagreed with by half its practitioners half of the time. I will try stay away from questions of what to mock and the granularity of the tests. I have [written about my thoughts on maintainable unit tests](/maintainable-unit-tests/) already though. The practice of writing tests first, then making them pass, and then refactoring; gives fast and incremental feedback on both progress and the design of your code. While making functional progress a test suite is being built up that proves that what you have implemented is working as expected by you as the developer.

**Behavior-Driven Design** builds on top of the idea of TDD but with a focus on capturing requirements in an automated way that fosters domain understanding and collaboration with stakeholders. 

It really isn't clear to me that the 2 need to be separate practices. BDD is just TDD practiced by developers with a [focus on domain knowledge and stakeholder collaboration](/acceptance-tests/). From the other side, TDD has become what developers do when they are not focusing on stakeholder collaboration. This was not its original intent.

Honestly though I do find the distinction a little bit useful in thinking about the kind of tests I am writing just because it allows me to as questions about the quality of what was built and the functional correctness.

![Test Quadrant](/img/posts/2018/test-quadrant.jpg)

So for the sake of comparison we will make the distinction that unit tests are an artifact of TDD and acceptance tests are an artifact of BDD. Don't get too attached to this idea, it is just useful for the upcoming discussion.

## Asking the right questions

I promised some questions to be asked to give a different perspective on the types of tests. What if instead of thinking about tests in terms of how they were written (xUnit and C# vs Gherkin) we thought about them in terms of questions directed at the test?

*Do I understand the problem?  
Is my feature ready to ship?  
Does it behave as expected?*  
Check the **Acceptance tests**. It passed? Ship the feature.

*Am I confident I built it well?  
Does my code handle exceptions correctly?  
Is my codes API intuitive to use?*  
Check your **Unit tests**. Passes. I can am confident in the code. I can refactor with confidence.

*Does my data access work against a real database?  
Do my API calls work as expected?  
Are my message queues configured correctly?*  
Check the **Integration tests**. Passes. I am confident that I won't have surprises when the system runs. I will find integration problems quickly.

There are other types of tests like **Consumer-driven contracts** and **UI tests** that might be useful to you and I am sure you can come up with the questions if they matter to you. The point is that dividing your tests based on how they are implemented is less useful than distinguishing what answers each group of tests is good at giving.

## Summary

In this post I suggested that instead of looking at tests based on what they test or how they are implemented, it is more useful to ask what questions they can answer. For example:  
**Acceptance tests** answer *Did I build the right thing?* and *Can I ship it?*.  
**Unit tests** give me confidence on *Did I build it right?*.  
**Integration tests** tell me *Can these components communicate?* I especially like checking across process boundaries here.

Hopefully by this point I have convinced you to think about your tests in terms of the questions they answer and the actions you will take from those questions.

One last thing. Much of the gain in TDD is that unit tests gives you rapid feedback. As long as you have good trustworthy acceptance tests, deleting unit tests if they are causing any issues should be completely acceptable. They already gave a large amount of their benefit in the design and verification process.

I hope you found this useful. If so I would love to hear your thoughts on the different types of testing.

## Credits

- Background photo by [Stijn te Strake](- https://unsplash.com/@stijntestrake)
- Social photo by [Jeremy Bishop](https://unsplash.com/@tentides)