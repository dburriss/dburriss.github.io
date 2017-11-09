---
layout: post
title: "Estimation"
subtitle: "Tackling the uncertainty of software estimation."
author: "Devon Burriss"
category: Project Management
tags: [Agile]
comments: true
permalink: estimation
excerpt_separator: <!--more-->
social-img: "img/posts/2014/old-clock-800.jpg"
published: true
---

> Tackling the uncertainty of software estimation.

Most developers are horrible at estimation. Period. There are numerous reasons for this. Some of the responsibility falls outside of a developers control but there are still steps that a developer is obligated to take. 
<!--more-->

## Under-estimating the complexity

Without actually writing the code a developer can never know every nuance of the problem and possible corresponding solutions. Not to mention the problems spawned from the chosen solutions. This gets better with experience but is not an exact science. Even with UML diagrams and use-cases, the devil is in the details. The best course of action for a developer here is to break the the problem down into such small subtasks that the possible problems start to expose themselves but even this is not a guarantee. Not to mention the time that this actually takes. It falls to management to ensure that developers have the time they need to make these estimates, as well as all the information to do so. It falls to the developers to insist on both of these. Even so. These are only estimates and should be seen as such and not taken by any stake-holders as commitments, unless the developer has committed to these times under no duress. 

### Solution: Break down tasks 

As mentioned. Breaking down the tasks into easier to estimate chunks will go a long way in refining the schedule, as well as revealing hidden complexity. 

## Over-estimating ability

Often a problem seems simple and as a developer you would like to think you could implement a solution in minimal time. This often happens when problems emerge similar to ones we have solved before. Resist the urge to commit. Find out all the information. Break it down. Plan. Estimate. Do not let your ego get you into a position where you are sacrificing your health, family, and friends for a deadline you cannot realistically meet. And DO NOT sacrifice quality. There are no true shortcuts. What you gain in the short term you will lose over the length of the project with interest. 

### Solution: Planning Poker 

Planning Poker (http://en.wikipedia.org/wiki/Planning_poker) is an estimation technique. The basics are such: 
Get some developers into a room. 
Discuss a task that needs implementation. 
All developers write down an estimate or hold up fingers at the same time with their estimate. 
If there are huge discrepancies the task is discussed more. Discussions and estimations are repeated until all developer estimations are similar. 

See: [http://www.mountaingoatsoftware.com/agile/planning-poker](http://www.mountaingoatsoftware.com/agile/planning-poker)

## Handed down deadlines 

Sometimes deadlines are given to you from above. As an employee you will feel pressured to accept these deadlines. It is your choice whether you accept them. In The Clean Coder, “Uncle Bob” talks about the responsibilities of developers and managers. CEOs are trying to strategically grow a business, marketing is trying to win customers, project managers are trying to meet deadlines, and as a developer you are tasked with developing a quality product for the customer. By agreeing to unrealistic deadlines, you endanger the project. The earlier problems are identified, the more chance that catastrophe can be avoided. 

### Solution: Team discussion of workable solution 

If a deadline is immovable, the team (including the customer) need to work together toward a realistic goal. Features can be cut, overtime can be worked (within reason), and additional resources can be allocated (to a point) but the end result should always be a quality solution. Cutting corners just slows down development in the long run. A project becomes a mess. Productivity grinds to a halt. It is a chore to work on and eventually developers leave the company rather than work on the project.

## PERT

[Pert](http://en.wikipedia.org/wiki/Program_evaluation_and_review_technique_(PERT)) is an estimation technique developed by the U.S Navy for estimating projects. Combining it with planning poker should give a reasonable idea of when you can expect a task to be done. It works as follows. 
A developer will give 3 estimates for a work item (use with Planning Poker). 

**O:** Optimistic estimate – this is the time to complete a task if the stars align and unicorns come down and help complete the code. In other words, the best case scenario.

**P:** Pessimistic estimate – this is the time to complete a task when you have invoked the wrath of the programming gods. So. The worst case. 

**M:** Most likely estimate – this is the time that a developer usually gives. 

Plugging these values in we can get the time estimate for a task. 

**T = (O + 4M + P) ÷ 6**

Banking on this value would be dangerous though. Some buffer time is usually added to estimates. Rather than just thumb-sucking a buffer time, lets calculate the variance and add that to the estimate. 

**V = (P – O) ÷ 6** 

**Estimate = T + V** 

### Example 

Ok. So lets say that your team is asked to add a Quick Contact widget to an existing website. You get 3 developers in the room and ask for times. 

You get the following answers. 1, 3, and 4. In days. 

The 1 came from the developer who is going to be doing the work. 3 from the developer who did most of the existing widgets. 4 from the team lead. Due to the large discrepancies, discussions ensue. It turns out the widget creation process is non trivial but some functionality is inherited from existing widgets. So another round of planning poker gives the following values 3, 3 , and 4. You decide to go with 3. 
This was for the most likely time. For the best case you get 1 day and worst case is 7 days. 
**T** = *(O + 4M + P) ÷ 6* = *(1 + 12 + 7) ÷ 6 = 3.3 V* = *(P – O) ÷ 6* = *(7 – 1) ÷ 6* = **1** 

**Estimate = T + V = 4.3** 

So let's schedule this for a 4.5 days. 

### Conclusion 

So knowing our failings, and bearing in mind the goals of management, we can mitigate potential disaster by using the techniques outlined here. Estimation is never going to be an exact science but we can go a long way in making our estimates more accurate. Hope this helps. Good luck with your next project.

