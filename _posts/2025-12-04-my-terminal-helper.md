---
layout: post
title: "My Terminal Helper"
subtitle: "Asking for help without leaving the terminal"
permalink: my-terminal-helper
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/bulb-bg.jpg"
social-img: "img/posts/2018/bulb-500.jpg"
published: true
topics: [ai-agentic-systems]
keywords: [Tools, Automation, Agents, OpenCode]
---

---
As a long time user of [Warp Terminal](https://www.warp.dev/), I enjoyed the convenience of being able to do `> #How do I create a new worktree branch again?` and get the command straight in my terminal. A potential issue for you may be that Warp does not allow you to choose your model provider. So I created a little function to fill the gap.

<!--more-->

[Previously](/automation-heuristics), I tried my hand at a micro-post. I failed to keep it very micro. Let's see if I can do better today.

For this setup, I am using the awesome [OpenCode](https://opencode.ai/) but you can use whatever Agent CLI tool you prefer.

## Step 1: Create your terminal agent (Optional)

This step is optional as you could also just inline the prompt and the model selection into the `opencode` command below.

For reference, I will show how I have mine setup.

I have a custom [OpenCode agent](https://opencode.ai/docs/agents/) setup. It has instructions on the type of questions as well as output format. This markdown file is placed in `~./config/opencode/agent/terminal.md`.

```md


description: Answer questions for the terminal
mode: subagent
model: opencode/grok-code
tools:
  write: false
  edit: false
  bash: false


<role>
  You are an experienced software developer and terminal user.
</role>

<objective>
  Your objective is to answer questions and explain in 1 or 2 lines about what is asked.
  If the topic is about how to accomplish a task on the command line, answer with a short explanation and the command to run to a accomplish the task.
  Only respond, DO NOT run any tools/commands.
</objective>

<output_format>
  <one-line description of the command>
  CMD: <command>
  <one-line explanation of each argument>
</output_format>

<example>
  Prompt:
  How do I add a git worktree called feature-x
  Response:
  Create a branch with git worktrees named feature-x in folder ../feature-x, outside the bare repository.
  CMD: git worktree add ../feature-x
  folder - folder for the worktree and the branch name if no explicit name supplied
</example>
```

I have linked to the docs, so if you want to know more about OpenCode agents, the docs are pretty decent.

What you need to know is that because the file is in the `agent/` folder, with a filename of `terminal.md`, we have an agent called `terminal` available to us.

The other section to note in the agent is the `output_format` section, as we will make use of that return format in the script below.

## Step 2: Create your question function

In `~/.bashrc` or wherever you want to put your custom bash functions, copy in the following function.
```bash

q() {
    local prompt="$*"
    local out

    # Ask the agent
    out=$(opencode run --agent terminal "$prompt")
    printf "%s\n" "$out"

    # Extract command starting with "CMD:"
    local cmd
    cmd=$(printf "%s" "$out" | grep "^CMD:" | sed 's/^CMD:[[:space:]]*//')

    [ -z "$cmd" ] && return 0

    printf "Action? [r = run, c = copy, n = nothing] "
    read -r ans

    case "$ans" in
        r|R)
            eval "$cmd"
            ;;
        c|C)
            if command -v pbcopy >/dev/null 2>&1; then
                printf "%s" "$cmd" | pbcopy
                printf "Copied to clipboard (pbcopy)\n"
            elif command -v xclip >/dev/null 2>&1; then
                printf "%s" "$cmd" | xclip -selection clipboard
                printf "Copied to clipboard (xclip)\n"
            elif command -v wl-copy >/dev/null 2>&1; then
                printf "%s" "$cmd" | wl-copy
                printf "Copied to clipboard (wl-copy)\n"
            else
                printf "No clipboard tool found\n"
            fi
            ;;
        *)
            # n / empty / anything else → do nothing
            ;;
    esac
}
```

Walking through the main parts of the script:

1. `out=$(opencode run --agent terminal "$prompt")`: Asks your question to the "terminal" agent described above. It captures the response in the `out` variable.
2. `cmd=$(printf "%s" "$out" | grep "^CMD:" | sed 's/^CMD:[[:space:]]*//')`: If the response contains the text `CMD:`, it will extract the command to the `cmd` variable.
3. If `cmd` is not empty it will ask you if you want to run, copy, or do nothing with the command.

## Step 3: Test it out

Now you can use this to get those commands you don't use often and sometimes forget.

```bash
q How do I pretty print the last 5 commits in git?
Display the last 5 commits in a compact, one-line format showing abbreviated hash and commit message.

CMD: git log --oneline -5

--oneline - Format each commit on a single line with abbreviated hash and subject.

-5 - Limit the output to the most recent 5 commits.
Action? [r = run, c = copy, n = nothing]
```

## Closing thoughts

Unfortunately, this is quite a bit slower than the responses from Warp, so manage expectation.

A word of warning. You are executing commands being returned over the wire from a non-deterministic LLM. Use whatever caution you think that warrants. At the very least, check the command before you execute it.

Enjoy.


