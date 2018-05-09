---
layout: post
title: "Mobbing a story"
subtitle: "Leasons learned from a teams first few mob programming sessions"
description: "Mob programming is a great agile way to increase learning in a team. Here are 6 lessons learned during a mob programming session."
permalink: first-mob-programming
author: "Devon Burriss"
category: Programming
tags: [Agile,General,Productivity]
comments: true
excerpt_separator: <!--more-->
header-img: "/img/backgrounds/silhouette-bg.jpg"
social-img: "/img/posts/2018/hands-500.jpg"
published: true
---
Mob programming can be a great way of sharing knowledge, building ownership, as well as a way of getting a story done with everyone checking it. Although this can be slower because of everyone having an opinion, I do strongly believe that it results in a higher quality implementation with a greater chance of being functionally correct and bug free. I thought it would be helpful to share our learnings while completing a fairly complex story using mob programming.
<!--more-->

## Mob programming TL;DR

So a short TL;DR of mob programming if you don't know what mob programming is. Basically it is pair programming on steroids. Multiple developers work on a single problem using a single machine. This works well if there is a large screen or projector. All developers can contribute ideas and concerns while one person drives at the keyboard.
One common concern is over the efficiency of having a whole team working on a single problem. If it is a difficult problem, throwing more brain-power at it is a good idea. It also increases understanding and ownership of the code, which increases productivity of a team. Lastly, it is an opportunity for team members to learn from each other which again increases productivity over the long run.

## Learnings

We would regularly stop and review how things had gone and what might work better. This is important to build into all team based activities. Doing the wrong thing as a single developer is one thing, doing it with more people just multiplies the inefficiency. This bring me to the first learning...

### Time-box the drive time

Set a timer for 25 minutes (or whatever time you think works). Once a timer runs out use the moment to review what has been done in the time. Ask questions like "Are we happy with the current direction?" and "Do we want to continue on this path?". This breaks you from the flow of developing and engages all those brains involved to evaluate early and often. It also provides a good moment to swap drivers so someone else gets a chance at the keyboard. The previous driver then gets a chance to contribute without multitasking.

Another thing to check every few sessions is energy levels. If people run out of steam, engagement will drop and the benefits of mob programming dwindle.

## Park when needed

One thing we noticed very early on was that we would often go off on tangents that had very little to do with the story we were implementing. As an example we touched on the [style chosen to write the unit tests](/maintainable-unit-tests). This is a worthwhile discussion to have and it is important that the whole team understands and is on-board. On the other hand if we engaged on every topic, we would never complete the story. We decided that if any topic that was not directly related to the story could not be resolved in a few sentences, it should be parked. We wrote down the topic on a sticky note to discuss later and moved on.

## Have a roadmap

This was a fairly complicated problem in an existing codebase that not everybody was familiar with. At times we would lose track of what the current task was. On reflecting we decided it was useful to have a clear goal of what we were currently trying to achieve. We did this by drawing out the tasks that needed doing, their dependencies, and ticking off what had been done. The blue magnet is the task currently being worked on.

![mob todo list](/img/posts/2018/mob-todo.jpg)

## Avoid backseat driving

We found it nonconstructive to have everyone shouting instructions at the driver. Instead we would discuss a problem and decide on a direction. The driver would then implement what was decided on with the team helping out as necessary.

## Be courteous to other drivers

Criticizing the developer driving does not lead to a constructive environment to mob program in. Remember at some stage you should drive too.

## Pit-stop early and often

Be sure to commit early and often. Whenever a test passes, a new direction is chosen, a refactor is done. Commit it. We learned the hard way what happens if you do a refactoring and then want to back out of it.

## Conclusion

The team did comment on certain parts of the activity being more engaging than others. Some activities like creating types with lots of properties can be quite tedious to watch. Some learnings did come out of this, like what parts of the codebase can be repetitive which might be a code smell of over engineering.

Mob programming is a great activity for working more as a team and for those who have not pair-programmed before, participating without driving might make them more open to pair programming. It is also awesome for sharing knowledge throughout the team. The benefits will pay for the momentary drop in productivity due to parallelizing work. If you approach it in an agile way with continual feedback you can find ways to make it work for you. Just be sure to be accepting toward one another. Have you tried mob programming? If not, give it a go in your team. You do it regularly?
Please share your experiences!