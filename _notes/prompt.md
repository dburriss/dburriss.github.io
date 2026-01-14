---
title: Prompt
published: true
keywords: [ai, prompt, context]
topics: [ai-agentic-systems]
status: draft
---
A prompt is an instruction given to a [[LLM]] for completion/inference. It can be anything from a direct question to a complex instruction to complete a complicated task.

Since this prompt is added to the [[Context]] along with any system prompts and [[Retrieval]] text, the contents and even the order of the contents can influence results substantially.

Some best practices around [[Prompt Structure]] have evolved over the years, with a whole discipline called [[Prompt Engineering]].

When working in an [[Agent Harness]] like an IDE, TUI, or desktop application, it is common to wrap up commonly used prompts as [[Slash Commands]].
