---
layout: post
title: "Categories of tests"
subtitle: "Unit, Integration, UI, Acceptance, etc. Are these useful categorizations?"
description: ""
permalink: categories-of-tests
author: "Devon Burriss"
category: Software Development
tags: [Testing,ATDD,BDD,TDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2019/target-500.jpg"
published: true
---
There are many types of tests, and when the categorization depends on the properties of said tests, questions get asked such as: 
- "Does the test exercise a single method?"
- "Am I mocking all dependencies?"
- "Do they cross a structural/process boundary?"
- "Do I interact with external services/databases?"

Sometimes for good reason we developers can get really hung up on naming. Another thing we can get hung up on is the structural "how" of things. I believe this has resulted in tests being broken up by the structural parts, resulting in many types of often overlapping test categorization. I will make the case that we have possibly fixated on this "how" a little too much and propose a broader classification based on the [questions we are wanting our tests to answer](https://devonburriss.me/anatomy-of-automated-testing/).
<!--more-->
A project I am currently on has got me thinking about how we test. One of the very smart people we have on the team, Hongli, [already wrote about it](https://www.joyfulbikeshedding.com/blog/2020-02-07-a-better-way-to-reason-about-software-testing-terms.html). Although I agreed with most of the contents of that post, categorizing by size has a few short-comings in my opinion.  
Firstly, size is something that is relative. The time something takes to run could depend a lot on language, hardware, System-under-test, etc.
Secondly, it is overloading a term people may already be using to describe a problem with a test that may relate to the complexity of the domain. For example, a big complex domain may just require a lot of test setup & assertion. This may also be referred to as BIG.
Finally, although the goal is to look at behavior, size is a structural measure. I have written about this before but will reiterate them in the next section as they are central to my thoughts on why our current categorization is often off.  
Reading Hongli's article crystallized my thoughts on this issue tremendously, highlighting the importance of surrounding yourself with good people who are committed to well-reasoned arguments and the sharing of insights.

## Structural categorization

I have written previously about how [coupling to structure](https://devonburriss.me/maintainable-unit-tests/) can cause tests to be less maintainable. I propose that Unit, UI, Component, and even Broad-stack are emphasizing structure in their name, which is something we want to avoid in naming (yes UNIT is open to interpretation). This language is important because [language shapes how we think](https://www.ted.com/talks/lera_boroditsky_how_language_shapes_the_way_we_think).

**Why shouldn't your tests be structural?**

> Tests should allow you to change your implementation with confidence. Tying your tests to the implementation structure means your tests need to change when your implementation changes. This is the opposite of what you want.

Of course, many developers know we should decouple our tests from structure. What I am trying to spark is a different way of thinking and talking about tests that de-emphasizes structure. Do you accept that language & naming matter? Do you accept that you want to be able to change your implementation while keeping your tests unchanged and passing?

I have seen too many codebases that are difficult to change because of tests that know about the deep inner-workings of the code they are testing. This is not to say that detailed tests have no use but rather that it should be clear that that is what they are and that they can be deleted if and when they stop being useful. I will expand on this later in this post.

## Group by the types of questions your tests answer

Evolving from my [thoughts on this back in 2018](https://devonburriss.me/anatomy-of-automated-testing/), I propose that at a high level the types of tests we write are trying to do one of the following:

1. Verify the **correctness** of a feature we are developing
1. Confirm the **communication** contracts with other systems
1. Explore and get fast feedback on **construction of code** we write to achieve the previous 2 points

Alternative:  

1. *Acceptance* of a feature
1. *Building* implementation code
1. *Communicating* with external systems

Let's go through these 3 groupings and see what they mean and the types of tests you might write in each.

### Correctness | Acceptance

This category of tests focuses on the correctness of the feature for the purpose of release readiness. They answer questions that your **stakeholders** will care deeply about:

- Is a feature implemented in the system?
- Is a feature behaving as expected?
- Is it safe to release?

Your acceptance tests should give you an emphatic answer to these questions. This category makes no demands on whether you [test against code](https://devonburriss.me/acceptance-tests/) or [block-box boundary tests](https://blog.picnic.nl/staying-ahead-of-the-competition-with-executable-specifications-1c7cb31acbb1). It makes no demand on using regular code or a specialized DSL like [Gherkin](https://specflow.org/bdd/gherkin/).  
I do believe the tests in this category should have the following properties:

- AVOID deep knowledge of internals
- FOCUS on behavior
- FOCUS on domain language

### Construction | Building

In this category we have the more detailed implementation tests. They allow us to experiment with our implementation API, get rapid feedback on said implementation, and give a sense of progress. They give feedback to **developers** like:

- Is this API intuitive to work with?
- Does this library behave as I suspect?
- Does this method behave as intended in a specific situation?

These tests are part of a developers process of building high quality software. They exist because you built your software right. They are not there to tell you whether you built the right thing. They are for catching bugs while you are coding not specifically for catching regressions. They COULD catch a regression but I propose that this is an indication that other types of tests discussed in this article are missing a scenario. We want to be in the situation where we can delete any number of our tests used as scaffolding while building the code and still have a high level of confidence in our test suite.

- DO create scaffolding tests while building up your implementations
- FOCUS on just enough scaffolding to help you move forward
- FOCUS on just enough quality for tests to be quick and easy to create
- AVOID being dependent or attached to these tests
- DO delete these tests if implementation changes

### Communication | Communicating

The last category is about communication across processes. These tests are checking that all **collaborators** (producers and consumers) are communicating with a shared language. Additionally, you should check that collaborators are indeed where they are expected to be. What questions are these tests expected to answer?

- Is the producer using the schema the consumer is expecting?
- Is my environment configured with the correct connection information for endpoints?
- Does a the UI of a form send the expected data structure my backend is expecting?
- What happens when collaborators are unreliable?
- What happens when too many producers shout at once?

Consider a call on an unstable line with someone from a foreign country. "Hello. Is this Mikhail?". "Can you hear me?". "Are you comfortable doing this call in English?". "I have bad reception. I will call you back in 5 minutes."
Consumer-Driven Contract tests are the first tests I lump in this category. They can be immensely useful and cheaply written if you keep it simple. As implied by the name, they are testing the contract. From a communication point of view, you are ensuring that you are talking the same language with a shared understanding of what words mean.  
My next test may be a little more controversial to some. Health checks of dependencies. The idea here is that you expose a health check on an application that reports the health of its dependencies from the point of view of that application. This has 2 benefits. On deployment you can pick up whether communication lines are open. It gives information on why a service may be misbehaving. Some may disagree with this being a test in the correct sense of the word. I prefer to focus on whether something is useful? Absolutes are the domain of religion. I would ask whether testing configuration data in any setting other than the actual deployed environment has much value.
Other tests that fall here are contract testing in general, integration tests, and UI component tests that focus on collected data and not the backend service logic. On the more advanced side, there are 2 more aspects that can fall under communication. Resilience of communication lines can be tested with chaos monkey testing. Bandwidth (don't take this in too technical a sense) can be tested with load testing.

- DO focus on communication between collaborators
- AVOID testing application logic
- FOCUS on schemas in an isolated manner
- FOCUS on point-to-point in the environment the communication occurs in

### Summary

TODO: Table with category and types of tests, questions, focus

## Conclusion

> If you were expecting to see something like property testing here, I view that more as a technique that could be used across multiple of the categories and test types, depending on where you are wanting to exercise boundaries of your input.

In this article I did not present any new kind of tests. What I am trying to highlight again is that we should be thinking about the kinds of questions our tests should be answering. Furthermore, I propose a categorization with a simple acronym of **ABC** to reason through the focus these tests should have.

Although I don't think it is important to prescribe any specific way for organizing these tests, as that often comes down to types of tests and the test runners, I do think it is important to make it clear in a codebase which tests are in the BUILDING category. This way any developer on the team can be be confident to delete those tests when they start to be more of a hinderance than a boon.

Lastly, I wanted to call out that security tests are not included in any examples. I did not want to muddy the water as these are often a bit separate from pure application testing. Personally, I think these fall under Correctness if you raise security as a requirement, as well as the responsibility of every developer to safe-guard.

## Notes (to delete)

Questions  

- ABC acronym or CCC?
- Should this be broken into smaller posts? ie. go deeper on the types of tests and how?
- What is unexpected here and needs more explanation within the post?
- What, question, details, focus enough for each section? Too much?

- size can depend on complexity and nature of the problem
- why -> what -> how
- fixated on structure rather than why

Code / Construction - scaffolding always gets removed (NJ)
Communication / Boundary
Correctness

