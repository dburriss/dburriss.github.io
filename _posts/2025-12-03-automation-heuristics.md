---
layout: post
title: "Automation Heuristics"
subtitle: "Some rules of thumb for automation"
permalink: automation-heuristics
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/posts/2018/bridge-cables-500.jpg"
published: true
topics: []
keywords: [DevOps, Automation]
---

---
While writing up some automation work with GitHub and Copilot (post coming soon), I had thoughts that didn't fit that post. I think them worth jotting down, so here's an experiment in a micro-post format.  
Let's see how this goes since my posts tend to always be longer than I intended.
<!--more-->

## Automate the boring things

If something is boring, chances are most humans are not going to enjoy doing it. This is a prime candidate for automation because if it is boring:

- It is probably repetitive
- It is likely predictable
- It does not bring joy to do (toil)
- Chances are it brings very little business value

You should be asking yourself how you can automate this.

This brings me to my next point. "Artificial Intelligence".

## A new capability: Natural language

I really dislike our use of the word "Artificial Intelligence" to describe the latest breed of LLMs. The marketing teams have done their jobs well though, so here we are.

These token generators have given us new capabilities that can drastically lower the cost of automating certain kinds of work. This new capability is **working in natural language**. Parsing language to instructions a machine can execute. Changing text based on natural language instructions. Things that were once difficult and expensive are now easy and cheap. **This expands the possibilities of things that we should be looking at automating**.

## The Golden Path

I have often run into platform teams mandating other teams use their tool because it is "the standard". There are often very good reasons for why teams SHOULD be using a standard way. When providing any kind of standard, there is an opportunity.

**Make the RIGHT way to do something, the EASIEST way to do the thing**.

People seldom want to do a bad job but they often have competing concerns. If you add too much friction to the right way of doing something, you are going to have a hard time convincing people. Conversely, if you provide something that makes their life easier while standardising, adoption becomes painless.

This brings me to my next point...

## Platform teams are leveraged

Platform teams operating in organisations with many development teams, are leveraged. And the more teams that use the platform, the higher the leverage. What do I mean by this?

Platforms act as a multipliers for productivity. Gains from good abstractions and workflows enabled by a platform team increase productivity across multiple teams. Trust goes up, adoption goes up, and productivity flywheels.

Unfortunately, the opposite is true too. If a platform team offers unintuitive abstractions and tooling, flaky workflows, and foot-guns galore; the impact is felt across the organisation. Worse, the platform generates an endless stream of support requests, ad-hoc fixes, firefighting, and general toil. This has a further knock-on effect to the capacity and quality of future work, feeding the flywheel of toil and tanking the potential of the whole engineering organisation.

**Platforms are a double-edged sword** that can cut both ways. The payout is big if done well, the cost is high if done poorly.

Enough bad analogies. Moving on...

## Build pipelines

A couple years ago I posted about a simple [functional programming architecture](/fp-architecture) that I use regardless of whether it is a CLI or a web API.

Reminding teams that are in the business of building automation to build pipelines might seem redundant, but hear me out. A pipeline is a series of steps. Too often I see pipelines that are:

1. Take the input.
2. Do the thing.

And don't get me wrong, sometimes doing the most straightforward thing is the best place to start to validate an idea. The heart of architecture, in my opinion, is optionality. It is making design choices that make it easy (or at least possible), to enable capabilities the business needs in the future. A future we cannot see.

So consider this small change:

1. Take the input.
2. Determine the actions.
3. Do the actions.

This simple change, turning a program into a pipeline, gives us optionality in the future. That input may produce only 1 action now but we have a design that can be easily extended. For sure, [YAGNI](/yagnyagni) and KISS are good principles to keep in mind but it depends what you are building. A quick one off automation? Keep it simple. A Platform that is the basis for the business to build on for years to come? Maybe put some thought into how to evolve requirements over time.

Which brings me to my final and related point...

## Composition over ... everything

I could probably write a whole series about composition but this post is already failing as a micro-post so I want to call out one controversial design choice unlocked by mastering good composition in your design.

Designing the perfect API or abstraction is difficult and in a complex environment it is unlikely you will succeed in catering for every teams needs. My controversial advice? Share your internal implementation details.

Ok, that was ragebait but I do mean it. Consider the situation where you have this neat abstraction where a team just needs to define a target workload, some environment setting, and an artifact to deploy. All the internal details are hidden away from the client teams. Now imagine some team comes in with some weird requirements because the business has pivoted and they need to get something highly experimental out ASAP.  
What happens in these cases? One of 2 things. The platform team needs to drop what they are doing and pivot to unblock this high priority project. They hack something into their beautiful abstraction, breaking the encapsulation, but unblocking the team. Or if teams are really really siloed or bureaucratic, the blocked team is left to hack together their own dodgy solution.

Now consider my alternative. Your beautiful abstraction is built from composable building blocks that represent atomic operations for working with workloads, environments, and artifacts. These building blocks are treated as public APIs and so have detailed documentation. The team with the custom request can now unblock themselves instead of scuttling the platform teams roadmap. The platform team can learn from what the other team built, thoughtfully integrating it using exactly the same building blocks.

**Exposing some internal building blocks has major upsides if done intentionally**. This allows you to focus on the 80% without screwing over the 20%.

## Conclusion

So in conclusion, much like an LLM I am incapable of not being verbose. Top of mind for me of late is that we are rushing into a time of accelerated pace. The use of generative and agentic tools unlocks new opportunities but also amplifies risks. Good practices and fundamentals are becoming more important than ever. We can both generate value faster, or drive our maintenance costs into orbit faster than it takes NPM to get hacked.


