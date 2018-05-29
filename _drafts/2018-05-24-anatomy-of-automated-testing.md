---
layout: post
title: ""
subtitle: ""
description: ""
permalink: anatomy-of-automated-testing.md
author: "Devon Burriss"
category: Programming
tags: [Testing,ATDD,BDD,TDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/explore-590.jpg"
published: false
---
Recently in an open session on BDD I noticed quite some disagreement on terms used around talking about testing,  when different styles of testing should be used, and who the target audience is. These are not necessarily terms on authority but rather what I have found to be useful after a couple of years of trying to reason about these.
<!--more-->
NOTES

- building it right vs building the right thing
- Types: Unit, Integration, Acceptance
- Process: TDD, BDD
- Acceptance: For communication and understanding
- Integration: Prove contracts and catch regressions
- Unit: For developers to get fast feedback

Unit, Integration, Acceptance. The naming is not about what they test but rather what they are used for.

Specifications identify what a system should do, not how.

================

Software testing is a vast subject, riddled with contradicting opinions. So what do we need to clear this up? ONE MORE OPINION ;)

When talking about different types of tests the test pyramid is often used. The pyramid itself doesn't tell you what