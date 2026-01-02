---
layout: post
title: "LLMs are not junior programmers"
subtitle: "Sometimes better, sometimes worse, always different"
permalink: llms-not-junior
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/posts/2018/team-500.jpg"
published: true
topics: [ai-agentic-systems]
keywords: [Tools, AI, Agentic, LLMs]
---

---
I often hear people compare LLM tools to junior developers. This comparison is not only incorrect, it is holding you back. If you have an incorrect mental model of the tool, you will constantly be surprised by the results. In this post I will explain how viewing the LLM as a junior is holding you back from better outcomes. 
<!--more-->

It should be no surprise that a large language model (LLM) is not like a human. The marketing hype has been pushing the Artificial Intelligence narrative for years now though. As social primates, language is integral for how we communicate with other humans, so it is only natural that we anthropomorphise these machines that are so adept at language.

It is important to know what you are dealing with. I summarise it as:

> The LLM is a stateless, stochastic function: `(parameters, prompt, runtime config) → output`  

So what does this mean?

- Stateless: It does not remember. The output is based on the input.
- Stochastic: The next generated token is based on a probability distribution of what has come before in the text. Effectively, it is non-deterministic.[^1]
- Function: It generates the next token based on what has come before. The output is the result of a sequence of next most likely tokens.

Hopefully, this is a useful grounding as we go through the precise ways that a coding assistant is not like a junior programmer.

## 1. A junior learns

A junior is responsible for learning from their experience. As they work and are mentored, they gain knowledge and skills that they bring into their next task. They improve.

The agentic tools like Claude, OpenCode, or Copilot do not learn. They are stateless. All context that will determine a high or low quality output needs to be supplied by you or your tooling.

Knowing this as the operator of the agentic coding tool, it is your responsibility to "teach" the agent as you go. The trick is that you need to teach it with every input. I have heard this phrased as "A highly skilled programmer with amnesia".

There are many techniques for giving these agents memory but the two easiest and most commonly used are:

- [AGENTS.md](https://agents.md/) - read by most agentic tools by default
- [Extract Knowledge](https://lexler.github.io/augmented-coding-patterns/patterns/extract-knowledge/) - as you converse with the agent you extract correct knowledge into documents on specific topics for later use

The important meta technique to takeaway here is that these are continuous exercises that you are doing. If you only correct the agent in your current session, the learning will be lost. You need to capture the learning somewhere if you want your agent to improve like a junior would.

## 2. Depth of knowledge

The next way that these models diverge from our typical junior programmer is in their depth of "knowledge". The `parameters` part of our stateless function can be thought of as a compression of all of the model's training data. These parameters are arrays of numbers that encode the syntax, semantics, knowledge, and biases of the training data and fine-tuning that went into the model's creation.

It is important to know that the knowledge is not stored "as-is" for recall. It is encoded in a "lossy" way in these parameters. These are used to generate responses based on that knowledge that was in the training data.

Functionally, the model has access to more knowledge than any human would be able to consume in a lifetime. This is very different from our junior fresh out of university, college, or a coding bootcamp.

This means we can bounce architecture ideas off our agent and then swap over to asking for tips on UX, then testing, then performance optimisations. When giving it tasks, it will do most far better than a junior would, given the correct guidance.

Remember though, that the "knowledge" captured in the parameters for a model is based on its training data. So if your task requires specialised knowledge, you are going to have to feed that knowledge to the model EVERY SINGLE TIME it is needed (see point 1).

## 3. Speed of execution

I am not going to spend much time on this. If you have used these tools, you know that they can complete a task faster (and usually cheaper unless something goes wrong) than any human can.

This has an important implication. If you are not learning how these tools work, and when they work well, you will be outperformed by those that do. In the future it will be expected that certain classes of tasks are not done by hand. Conversely, given sufficient proficiency, you should probably not give certain tasks to a coding agent. Learning which is the newest in a long line of things developers are expected to know.

## 4. World model

Returning to output generation. We already covered that the "knowledge" is encoded in a "lossy" way using the arrays of numbers in the parameters. These weighting numbers combined with the runtime configuration of things like *temperature* determine the next token generated until the full output is returned.

It is important to notice that this stochastic model contains nothing that represents a model of the real world, other than that the training data was created in the real world. The LLM has no way of testing its output with reality, because it *understands neither its output nor the real world*.

When there is a mismatch, this is referred to as hallucinating. *Hallucination is perception in the absence of stimulus*. These models have no stimulus. They have no model of the real world to test their output on. For example, it does not know that `if` is followed by a condition; just that something containing `==` or `>=` is statistically likely. 

> LLMs are hallucinating 100% of the time. They happen to match reality well enough, often enough, to be useful.

So our junior programmer is different to these models in a useful way. They can reality test and reason in a way that the coding assistant cannot.

## 5. Context switching

As a human developer, you sometimes need to complete little sub-tasks on your way to your main task. These tasks can add up as you [shave the yak](https://www.hanselman.com/blog/yak-shaving-defined-ill-get-that-done-as-soon-as-i-shave-this-yak). For us this can be natural as we follow the chain of steps and get them done, and move on. This is [terrible for coding assistants](https://lexler.github.io/augmented-coding-patterns/obstacles/limited-focus/) as their [context rots](https://lexler.github.io/augmented-coding-patterns/obstacles/context-rot/).

The implication is that, for best results, as you work you should constantly manage your context by capturing knowledge and then refreshing your context by starting a new session.

## 6. Task explosion

These coding assistants and our junior programmer have one annoying tendency in common. Given a task, they will happily charge forward and implement the given task in one massive 2k line pull request.

For best results, especially for more complex tasks, it is best to be explicit about breaking it down into smaller tasks. These agents will often break down larger tasks into smaller tasks but in my experience it is best to give them small focused tasks to begin with.

## Conclusion

Comparison with junior or even senior developers does more harm than good, as I see it. It distracts from how this tool works and holds us back from making the most of it. I suggest that everyone learn a little about how these tools work under the hood, to gain some mechanical sympathy. Then use them regularly to gain hands on experience.

So remember: They are token generators, not social primates.

I love you Skynet. Don't kill me.

## Footnotes

[^1]: Non-deterministic given the seed is uncontrolled.


