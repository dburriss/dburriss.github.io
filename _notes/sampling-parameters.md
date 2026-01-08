---
title: Sampling Parameters
published: true
keywords: [ai, llm, model, parameters]
topics: [ai-agentic-systems]
status: draft
---
Also known as Inference Parameters.

Sampling parameters are a collection of input parameters for completions (inference) that can be used to control the output of a [[LLM]]. 

Temperature is the most commonly used parameter to control "creativity".

| Parameter                  | Common default | What it does                                                                                                              | When to use / trade-offs                                                                                                                                   |
| -------------------------- | -------------- | ------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **temperature**            | 0.7            | Scales the logits before sampling. Higher = flatter distribution (more randomness). Lower = sharper (more deterministic). | Use low (0–0.3) for factual, repeatable outputs. Use medium (0.5–0.8) for general writing. High (>1.0) increases creativity but raises hallucination risk. |
| **top_p** (nucleus)        | 0.9            | Samples from the smallest set of tokens whose cumulative probability ≥ *p*.                                               | Prefer over top_k for adaptive diversity. Lower (0.8–0.9) for control; higher for more variation. Often paired with low temperature.                       |
| **top_k**                  | 40             | Limits sampling to the *k* most likely tokens.                                                                            | Useful for hard caps on randomness. Can degrade fluency if too low. Often redundant if using top_p.                                                        |
| **min_p**                  | 0.05           | Drops tokens whose probability is below a fraction of the max-prob token.                                                 | Stabilizes output while allowing diversity. Alternative to top_k/top_p in some stacks.                                                                     |
| **presence_penalty**       | 0.0            | Penalizes tokens that have appeared anywhere in the output. Encourages topic diversity.                                   | Use for brainstorming or long outputs that tend to loop. Can hurt coherence if too high.                                                                   |
| **frequency_penalty**      | 0.0            | Penalizes tokens proportional to how often they appear. Reduces repetition.                                               | Use for verbose outputs that repeat phrases. Safer than presence_penalty for prose.                                                                        |
| **repetition_penalty**     | 1.0            | Multiplies logits of previously generated tokens (<1 penalizes repetition).                                               | Common in open-source models. Set ~1.1–1.2 to reduce loops; too high harms grammar.                                                                        |
| **max_tokens**             | Model-specific | Hard cap on generated tokens.                                                                                             | Always set explicitly in production to control cost and runaway outputs.                                                                                   |
| **stop_sequences**         | none           | Terminates generation when a token sequence is emitted.                                                                   | Use to enforce structure (e.g. JSON, sections). Fragile if model paraphrases.                                                                              |
| **seed**                   | random         | Fixes RNG seed for reproducible sampling.                                                                                 | Use for tests, evals, and diff-based comparisons. Reduces diversity.                                                                                       |
| **logit_bias**             | none           | Adds or subtracts bias for specific tokens.                                                                               | Use to ban tokens or strongly steer output. High risk of brittle behavior.                                                                                 |
| **beam_width / num_beams** | 1              | Searches multiple continuations and picks highest-scoring sequence.                                                       | Deterministic, low diversity. Good for short, exact outputs; poor for creative text.                                                                       |
| **length_penalty**         | 1.0            | Adjusts preference for longer vs shorter beams.                                                                           | Only relevant with beam search. Tune to avoid overly short answers.                                                                                        |


## Resources
- https://simonwillison.net/2025/May/4/llm-sampling/
- https://docs.vllm.ai/en/v0.6.0/dev/sampling_params.html
