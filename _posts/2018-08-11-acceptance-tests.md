---
layout: post
title: "Writing readable Acceptance tests"
subtitle: "A story about getting Acceptance tests just right"
description: "Tips and examples on improving the readability of your acceptance/behavior tests"
permalink: acceptance-tests
author: "Devon Burriss"
category: Programming
tags: [Testing,ATDD,BDD,TDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/cottage-bg.jpg"
social-img: "img/posts/2018/forest-girl-500.jpg"
published: true
---

Acceptance tests can be a great way of making sure you are building the right thing. When used in in a way that uses natural language it also serves as a collaboration tool with stakeholders to define what should be built before it is built. This can save a great deal of development time in making sure you don't build the wrong thing and also has the added benefit of growing a developers domain knowledge as he or she collaborates with a stakeholder in fleshing out and verifying the acceptance tests. Recently we invested a fair amount of time in a team here at work iterating on the style of the acceptance tests. We figured if the goal is to allow developers and stakeholders to collaborate, then making sure the tests makes sense to both parties is important. In this post I will share some of the experiences I have gained over the years, more specifically showing how we applied this to improving our acceptance tests in my current domain. As always though this was a collaborative effort within the team.

## A brief introduction

> You probably want to skip to the next section if you already have experience with the Gherkin language.

Acceptance or behavior tests come in many different forms but probably the most common is those described in the [Gherkin](https://docs.cucumber.io/gherkin/reference/) language which is a domain specific language for writing easily readable specifications that can be executed. The most common keywords used are:

> `Feature`: provide a high-level description of a software feature, and to group related scenarios  
> `Scenario`: a concrete example that illustrates a business rule. Consists of one or more steps (Given, When, Then, Examples)  
> `Given`: describe the initial state of a system  
> `When`: describe events or actions that occur in or against the system  
> `Then`: describe the expected outcome of the `When` actions against the system

The Gherkin language has many different runners such as [Cucumber](https://docs.cucumber.io/), [Specflow](https://specflow.org/), and [Behat](http://behat.org/) for whatever your programming language of choice is. Using Gherkin is not the only way of writing behavior oriented tests. Many developers just use standard testing frameworks or more low level ones oriented toward behavior testing. Personally I think if you are committed to working on the tests collaboratively with stakeholders it is difficult to overestimate the benefits of a format that is readable to non-developers.

### Acceptance tests vs Behavior Driven Development (BDD)

Although this is not the focus of this post I did want to mention the difference here in my mind. BDD is the practice of defining specification of how a system should behave and automating the execution of those specifications. Defining the specification of what needs to be built requires deliberate discovery of requirements, which requires collaboration between stakeholders and developers. By discovering the unknowns upfront development is more productive, with less surprises and rework throughout the development life-cycle.

Acceptance tests can be an integral artifact from the process of BDD. In my mind Acceptance tests are simply the tests that answer these simple questions: "What must the feature do?", "Is it done?", and "Can I deploy it?". In a perfect world with perfect confidence in your acceptance tests, they are the gate for continuous delivery of features. Once they are passing the feature is in production.

## Lost in the woods

Once you sit down to write an Acceptance test you start to realise there are many ways you can write them. What classifies as a feature? What level of abstraction do I write against? How specific do I make my scenarios? Black-box tests or not?

I will attempt to answer these quickly before showing you the evolution of our acceptance tests, although I suspect some of my answers will fall short considering how different teams' stories can be.

*What classifies as a feature?* This is a single piece of functionality that can be shipped independently from others. This is often difficult to determine because sometimes just because a feature is independently shippable doesn't always make sense for it to be shipped. In the examples to follow we experienced this because although different *types of Purchase Agreements* have different behavior and can be independently shipped, until we covered a certain subset of all types it didn't make sense for us to release. A helpful question here might be *Could X be broken while Y is still considered correct?* Then it quite possibly could be a feature.

*What level of abstraction do I write against?* In a way, this one is easy. The very highest. The one the business operates and talks at. [Hopefully your code is written at this level of abstraction at the entry point as well](http://devonburriss.me/managing-code-complexity/). Your acceptance tests should not be mentioning things in your code or implementation details that are not going to make sense to business stakeholders. The easiest way to check this is to ask a business stakeholder to read your test. Or better yet, co-write them.

*How specific do I make my scenarios?* My advice here would be to make them pretty damn specific. What you are aiming for is an example that has the makings of a real life scenario that a stakeholder would be tackling. You are looking for a couple scenarios that collectively catch most permutations in the system. I don't think it is necessary to capture EVERY permutation through your scenarios. Other lower-cost forms of testing can catch these if necessary. `Examples` can also go a long way in covering permutations if you feel you need them and in a way that doesn't get too verbose. Use these judiciously though. If the test is no longer going to make sense to a stakeholder, prefer a lower cost test like a unit test to check permutations.

*Black-box tests or not?* I use the term **Black-box** to describe a test that doesn't know anything about the internals of your code. A black-box acceptance test would exercise the code through a UI, REST API, or command line and then observe the results in a database, message queue, logs, or console output. This has some pros and cons. Firstly you are really exercising your system like any other client would so you can have a lot of confidence that the system is working as a whole. The downside is that measuring the effects can be quite challenging and the tests can often take quite long to run, as well as be complex to setup. Whether you want to do this depends on the cost to benefit ratio. In the past where the core of a system was to orchestrate between many systems, I thought it important to verify that these interactions happened correctly. In that case a black-box test makes sense. For the examples I am going to show later in this post the major complexity was in the numerical calculations of the value of the agreement. Here we chose to execute against elements in the code without a running application because what we cared about was documenting and verifying the workings of these calculations. The most value was in being able to write and execute these in a shorter feedback loop. It did mean we missed some complexity related to persisting the stream of calculations and these needed to be covered by other tests.

## Waxing lyrical like Goldilocks

As mentioned in the introduction we really wanted to make sure that our acceptance tests were understandable by stakeholders and developers alike. We also really wanted these acceptance tests to serve as documentation in the future for how these calculations worked as we discovered in requirements gathering that this knowledge didn't reside in any one person's head.

So scenarios needed to be descriptive enough to really demonstrate how a calculation is done without each scenario being too dense with information. As it turned out this took some refinement.

As a quick introduction to the domain. In the contract management team we handle agreements with suppliers for an e-commerce company. Based on purchases or sales we might get money off the price of certain products purchased for stock, or sold on the website. 
If an agreement is a fixed amount per product sold with a factor of 10, then selling 3 is worth 3 units x 10 EUR = 30 EUR. 
Simple right?

### Too simple

The first iteration was optimized for ease of duplication for the developer. A lot of the details of the agreement are hidden. What I particularly dislike about this style is how hard it is to pick out the details that matter. There is some magic around it being `agreement1` possibly? See what you think of the first iteration...

```Gherkin
Feature: FixedAmountAgreement

Scenario: Purchase agreement limited to 2 product limitations is finalized (factor is 10, agreement runs for 5 days -> 2 euros per day -> 1 euro per target)
    Given Purchase agreement with id agreement1, starting yesterday and ending 3 days in the future, of type fixed amount, with status approved, with factor 10, and limitations
    | Type    | Name                     | Id   |
    | Product | Samsung Galaxy S8 Zwart  | P1   |
    | Product | Samsung Galaxy S8 Zilver | P2   |
    When the allocation process runs for the Purchase agreement
    Then the total allocated value for each day per product is 1
```

Is it easy for you to reason about what that scenario is? It is more about what data is used than what the actual scenario is.

### Too complex

Another trap that is easy to fall into is trying to test too much in a single scenario. This is similar to doing TDD with data driven tests ie. `[Theory]` with `[InlineData]` when using xUnit in .NET. Here we really loose any meaning in the scenario.

```Gherkin
Feature: SellInAgreement

Scenario Outline: Purchase agreement limited to 2 product limitations is finalized
    Given Purchase agreement with id agreement1, starting yesterday and ending tomorrow, of type <Type>, with status <Status>, with factor 2, and limitations
    | Type    | Name                     | Id   |
    | Product | Samsung Galaxy S8 Zwart  | P1   |
    | Product | Samsung Galaxy S8 Zilver | P2   |
    Given a purchase delivery verified yesterday with products
    | PurchaseDeliveryLineId | ProductId    | Quantity | Price |
    | PD1                    | P1           | 15       | 300   |
    | PD2                    | P2           | 10       | 280   |
    When the allocation process runs for the Purchase agreement
    Then the total allocated value on delivery line 1 is <DeliveryLine1Value>
    And the total allocated value on delivery line 2 is <DeliveryLine2Value>

Examples:
| Status                  | Type                               | DeliveryLine1Value | DeliveryLine2Value |
| approved                | percentage of purchased amount     | 90                 | 56                 |
| invoiced                | percentage of purchased amount     | 90                 | 56                 |
| waiting for credit note | percentage of purchased amount     | 90                 | 56                 |
| pending invoice         | percentage of purchased amount     | 90                 | 56                 |
| pending approval        | percentage of purchased amount     | 0                  | 0                  |
| rejected                | percentage of purchased amount     | 0                  | 0                  |
| deleted                 | percentage of purchased amount     | 0                  | 0                  |
| approved                | fixed amount per product purchased | 30                 | 20                 |
| invoiced                | fixed amount per product purchased | 30                 | 20                 |
| waiting for credit note | fixed amount per product purchased | 30                 | 20                 |
| pending invoice         | fixed amount per product purchased | 30                 | 20                 |
| pending approval        | fixed amount per product purchased | 0                  | 0                  |
| rejected                | fixed amount per product purchased | 0                  | 0                  |
| deleted                 | fixed amount per product purchased | 0                  | 0                  |
```

This one gives me little information on a scenario because it is really many scenarios. This is great for test coverage with a single test. It fails to document the behavior of the system in a way that makes it easy to reason about the characteristics of the system.

### Just right

The problem with both structures so far is they do not represent how a user of the system would reason about calculating the value of the agreement. Let's step through it and then try write a test with that mental model.

A user will have an agreement that they want to calculate. At any given time that agreement will apply to some deliveries on products defined in the agreement. When something happens to an agreement it will effect the calculation in a specific way. For example, if the start date of an agreement moves so the agreement runs for longer, then it is likely that more deliveries will fall within the running period of that agreement.

Ok so with this mental model of how a user would approach calculating the value of an agreement, can we write a test that mimics that...

```Gherkin
Feature: Fixed Amount Sell-in Purchase Agreement

Background:
    Given a fixed amount sell-in Purchase agreement
    | Name      | Value                             |
    | Starting  | 2017-01-05                        |
    | Ending    | 2017-02-25                        |
    | Type      | FixedAmountPerProductPurchased    |
    | Status    | Approved                          |
    | Factor    | 10                                |
    | Product   | P1                                |
    | Product   | P2                                |
    | Product   | P3                                |

 Scenario: Agreement start date is moved backwards so more purchase delivery lines are allocated against
    Given the following purchase delivery lines exist
    | Purchase Delivery Line Id | Product   | Quantity | Price | Verification date |
    | PD1                         | P1      | 3        | 100   | 01-01-2017        |
    | PD2                         | P2      | 6        | 110   | 05-01-2017        |
    | PD3                         | P3      | 10       | 210   | 05-01-2017        |
    And existing allocations for the agreement
    | Purchase Delivery Line Id | Product   | AllocatedValue |
    | PD2                         | P2      | 60             |
    | PD3                         | P3      | 100            |
    When the Purchase agreement start date changes to 2017-01-01
    And allocations are calculated for the Purchase agreement
    Then the following purchase delivery lines are allocated against
    | Purchase Delivery Line Id | Product   | AllocatedValue |
    | PD1                       | P1        | 30             |
    And the total allocated value for the Purchase agreement is 190
```

So that *agreement has some terms that effect the value* of it. These terms *mostly wont change across scenarios*. If they do *we want to highlight only the changes*. In the setup then we want to *show only what matters for the scenario*. We also want to *highlight behavior* and the *end result*.

#### Breakdown of the recipe

So we use `Background` to define the status quo across scenarios. It doesn't mean some of these values won't change but we only mention what does. This background can then be held constant across multiple scenarios. This allows us to still be explicit about the status of the agreement without needing to be verbose in EVERY scenario about it. It allows the reader to reuse the information across scenarios. It also means we only need to mention CHANGES.

Our `Scenario` can now be quite explicit about what will change. This allows us to document behavior way more explicitly than the previous tests while still having explicit information available to the reader if needed in the `Background`.

The `Given` steps allow us to define setup that is relevant to each `Scenario` only.

`When` steps will now typically define the actions that make a `Scenario` unique. This could of course be in the `Given` setup or a combination of both but typically it is the `When` that makes the scenario interesting.

Finally the `Then` steps allow us to define what happened in the system and what the final result is.

Do you see the focus on the actual scenario here? Did this convey more of what the business actually considers? I think so.

## Summary

So our first takeaway was that Acceptance tests and BDD in particular are a means of driving and documenting the expected behavior of the system while engaging with stakeholders. 

Then in writing behavior tests we want to focus on capturing scenarios that are meaningful to stakeholders and accurately capture the mental model they have of the system. By structuring the tests in such a way we not only make it easier for our stakeholders to understand but we also make it much more likely that we grow our understanding of the system. Any technique that allows developers to gain insight into the users perspective is worth more than just test coverage. Software development at its core is about learning a problem space. Writing code is the easy part.

I hope you found this useful. If you have any thoughts on Acceptance testing, BDD, and/or writing good tests, I would love to hear from you in the comments below.

## Credits

1. Background image by [Peter Kleinau](https://unsplash.com/@nepumuk)
2. Social image by [Hanna Postova](https://unsplash.com/@annapostovaya)