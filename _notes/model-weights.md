---
title: Model Weights
published: true
keywords: [ai, llm, model, parameters]
topics: [ai-agentic-systems]
status: draft
---

Model weights, also known as Model Parameters, should not be confused with [[Sampling Parameters|Sampling/Inference Parameters]]. 

Model Weights are matrices of float arrays that represent the strength of connections in the [[LLM]]'s [[Neural Network]]. Practically what that means is that they are an encoding of the patterns found in the training data.

These values are set during training of the model, and then tweaked in a process called [[Fine tuning]]. But once this training is done, the model parameters are set and cannot change. This is important because it means a [[LLM|Model]] cannot learn once training is done. 

## Resources
- https://artificialintelligenceschool.com/understanding-weights-in-large-language-models/
