---
layout: post
title: "You have to teach"
subtitle: "LLMs cannot learn, so you have to teach"
permalink: you-have-to-teach
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
published: false
topics: [ai-agentic-systems]
keywords: [Tools, AI, Agentic, Context Engineering]
---

---
I often hear people compare LLM tools to junior developers. This comparison is not only incorrect, it is holding you back. If you have an incorrect mental model of the tool, you will constantly be surprised by the results. In this post I'll share 3 techniques that can drastically improve the quality of your agentic helper.

<!--more-->

The problem with analogues like "these tools are like a junior developer", is that they are only correct sometimes. Are you talking about the skill level? The reasoning level? The autonomy? Learning rate?

So I will try to give an analogue or paint a picture for different scenarios to try give you a feel for how to think about each situation.

## Technique 1: Extract Knowledge

**Problem**: The models, regardless of available context length, experience [context rot](https://research.trychroma.com/context-rot) way before they reach their limit. Even models with a 1M token context length will drop off in performance around 10k tokens, and fall off an accuracy cliff around 100k tokens. How big of a deal this is depends on the task.


My dad has a notebook. He uses this notebook all the time. For everything. The problem is that there is not really an organisation structure to it. So as the book fills up, it becomes more and more difficult to find the piece of information you are looking for. Every page is a scrawl of different pieces of data claiming territory. Finding the needed information becomes hit-or-miss way before the book fills up.  
I use Obsidian as my digital notebook. I have daily notes with a scratchpad section. Whenever I have something interesting in the scratchpad, I have the option to extract it up into it's own note on that topic.

4 different posts?

- Extract knowledge
- error -> go fix NO What should you know not to make that mistake?
- Proactive retrieval and context priming
- Keep it small


