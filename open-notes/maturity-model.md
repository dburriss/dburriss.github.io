---
id: '202107270936'
tags: []
related: []
from:
---

# Maturity Model

This is something that each org or team could come up by themselves depending on the their objectives. My feeling is that in an organisation having these standard across teams of the same type ie. dev, infra, etc. could be beneficial for standardizing in a way that still fosters autonomy.

This goes toward something I believe quite strongly. Standardizing when you have immature standards is a good way to get stuck in mediocrity, at best. Unfortunately, you cannot know what you do not know, so my hope is that publishing and crowd-sourcing these maturity models across organisations will lead to better outcomes.

A maturity model is made up of levels. Each of these levels have various skills in them. I find skill a good word for these as usually anything that brings benefit to a team requires practice. So the skill is not just about doing something, but practicing it until you are skillful at it.
Take unit testing as an example. You can have some unit tests in your project and say you have that Skill. Having unit tests in your project is different from having maintainable unit tests that test useful aspects of your codebase. If they are written mostly after the fact and submitted as completely different PRs, they are not really helping to catch bugs. This is a sign of chasing a test coverage metric rather than using unit testing as a skill to deliver higher quality software.

And here in lies the danger in having a checklist like this maturity model. It can be viewed as a todo list to check-off, maybe even a source of pride over other teams. This is not the intent! It is meant as a guide to becoming a more skillful team at delivering quality software that is a joy to work with and adds value to customers.


## Tags

I am proposing some tags that can be applied to each Skill. This allows us to chose a mix of Skills that stretch the team in different ways at each level.

Examples:  

- code
- knowledge
- process
- observability
- maintenance
- security
- communication

## Example

### General development team

### Level 1

- [ ] [Unit testing](#unit-testing) [maintenance, code]
- [ ] [Centralized and prioritized stories](#centralized-and-prioritized-stories) [process, communication]
- [ ] [Regular team retrospective](#regular-team-retrospective) [process, knowledge]
- [ ] [Centralized Structured Logging](#centralized-structured-logging) [observability]
- [ ] [No secrets in source control](#no-secrets-in-source-control) [security]
- [ ] [Deploy monthly](#deploy-monthly) [process]
- [ ] [Database backups](#database-backups) [maintenance, security]

### Level 2

- [ ] [4 eyes principle](#4-eyes-principle) [process, knowledge]
- [ ] [Basic health endpoint](#basic-health-endpoint) [observability]
- [ ] [Refinement](#refinement) [process]
- [ ] [Infrastructure as Code](#infrastructure-as-code) [maintenance, process, code]
- [ ] [Validated configuration](#validated-configuration) [code]
- [ ] [Resources are tagged](#resources-are-tagged) [code, maintenance, observability]
- [ ] [Smoke tests](#smoke-tests) [maintenance]

### Level 3

- [ ] [Dashboards I](#dashboards-i) [observability]
- [ ] [Contract testing](#contract-testing) [maintenance, communication]
- [ ] [API client keys](#api-client-keys) [code, security]
- [ ] [Telemetry capture](#telemetry-capture) [observability]
- [ ] [Story readiness gate](#story-readiness-gate) [process]
- [ ] [Ubiquitous language](#ubiquitous-language) [process, code, knowledge]
- [ ] [Deploy weekly](#deploy-weekly) [process]

### Level 4

- [ ] [Dashboards II](#dashboards-ii) [observability]
- [ ] [Uptime alerts](#uptime-alerts) [observability]
- [ ] [Secrets in key vault](#secrets-in-key-vault) [security]
- [ ] [Custom metrics and KPIs](#custom-metrics-and-kpis) [observability]
- [ ] [Limited WIP](#limited-wip) [process]
- [ ] [Use-case entry points](#use-case-entry-points) [code]

### Level 5

- [ ] [Dashboards III](#dashboards-iii) [observability]
- [ ] [Acceptance tests](#acceptance-tests) [maintenance]
- [ ] [Design pitches](#design-pitches) [process, knowledge, communication]
- [ ] [Tech debt days](#tech-debt-days) [process]
- [ ] [Vulnerability scans](#vulnerability-scans) [security]
- [ ] [Idempotent APIs](idempotent-api) [code]
  
### Level 6

- [ ] [Keep current](#keep-current) [security, maintenance]
- [ ] [Command Query Separation](#command-query-separation) [code]
- [ ] [Dependency aware health checks](#dependency-aware-health-checks) [observability]
- [ ] [Retro all the things](#retro-all-the-things) [process]
- [ ] [Deploy daily](#deploy-daily) [process]

### Level 7

- [ ] [Honest types](#honest-types) [code, knowledge]
- [ ] [Pair programming](#pair-programming) [process, knowledge]
- [ ] [Correctness alerts](#correctness-alerts) [observability]
- [ ] [APM tracing](#apm-tracing) [observability]
- [ ] [Resilient requests](#resilient-requests) [code]

### Level 8

- [ ] [Embrace immutable code](#embrace-immutable-code) [code]
- [ ] [Mob programming](#mob-programming) [process, knowledge, communication]
- [ ] [Learning days](#learning-days) [process, knowledge]
- [ ] [Synthetics](#sythetics) [observability]

### Level 9

- [ ] [Cyclomatic complexity ceiling](#cyclomatic-complexity-ceiling) [code, observability]
- [ ] [Quantitative team health](#quantitative-team-health) [observability, process]
- [ ] [Embrace FP principles](#embrace-fp-principles) [code]
- [ ] [PR to production](#pr-to-production) [process]

### Level 10

- [ ] [Dashboards IV](#dashboards-iv) [observability, process]
- [ ] [Load testing](#load-testing) [maintenance]
- [ ] [Immutable Architecture](#immutable-architecture) [code]

### Level 11

- [ ] [Domain Driven design](#domain-driven-design) [process, knowledge, code, communication]
- [ ] [Property based testing](#property-based-testing) [maintenance]
- [ ] [Chaos engineering](#chaos-engineering) [code, maintenance]


## Skills

### Unit testing

Unit testing are written for all new features. The unit tests are readable, and [easy to maintain](https://devonburriss.me/maintainable-unit-tests/) using an ever growing set of helpers designed specifically for that domain. Unit tests are submitted with the new code or changes they are covered, thus actually ensuring some quality before code is deployed to production.

### Centralized and prioritized stories

The team keeps a single shared list of work that is being worked on and the list is prioritized from highest priority to lowest... preferably based on value, not urgency.

### Regular team retrospective

The team has a regular moment, AT MINIMUM monthly, to reflect on how things have gone over the last while and suggest improvements, which they then implement. Hopefully one of the first things to come out is monthly is too long a time to reflect on possible improvements.

### Centralized Structured Logging

Simply logging is not enough. Logs should be collected in a central place where they can be easily searched. To facilitate the searching, logs should be structured, rather than free text logs.

### No secrets in source control

No secrets to any environments should be in source control. Not just the current HEAD but any previous secrets should be scrubbed from history with something like [BFG](https://rtyley.github.io/bfg-repo-cleaner/).

### Deploy monthly

The minimum is that each application is deployed at least once per month. This includes applications that have not changed in that month to ensure that pipelines are being maintained.

### Database backups

Databases have a backup schedule. Databases are not considered backed up unless the backups are tested regularly. There is a process documented for restore.

### 4 eyes principle

Pull Requests are only merged when at least one other person has reviewed and agreed that the PR is ok.

### Basic health endpoint

All services have a basic `health` endpoint returning back a `200OK` when the service is running.

### Refinement

It is important that stories are refined and that those at the top of the list are in a state where they can be picked up. This skill asserts that the team has a process and techniques that make sure that this is always the case. Typically, this is only achieved if stories are worked on collaboratively with people with different skill and knowledge sets.  

Some recommended techniques for knowledge mining:

- 3 Amigos: Divide and conquer so the team can refine stories in smaller groups. This is often more effective and so any final refinement is done with stories fleshed out from multiple points of view.
- Example mapping: A great way to flesh out stories and give clear examples. Great for identifying edge-cases and expected behaviour. Also easy to do collaboratively.
- EventStorming: A great technique for knowledge mining. It makes clear current process to all involved and makes it easy to talk about needed changes or dive into new behaviour.

### Infrastructure as Code

All infrastructure should be created via selected code provider such as Terraform.

### Validated configuration

Configuration should be validated, at latest, at application startup. If you use conventions, it is useful to place the environment in the names so they can be validated for the current environment. For this reason it is best to have them at the start or end of the name.
### Resources are tagged

Resources should be tagged with a convention followed for all resources of each type. These should be enforced by policy automation.

### Smoke tests

A section of tests should be marked as smoke tests that run against an environment. These tests represent such fundamental functionality of the system that if any of those tests fail, the deployed version is considered invalid and rolled back. One consideration is configuration issues, so automated checking as much configuration before deployment is valuable.

### Dashboards I

A central dashboard with all owned applications and the current status and uptime SLO of that application.

### Contract Testing

The contracts between services should be tested and checked for breaking changes. There are some advanced techniques for this but this adds complications that tend to move them downstream on the software delivery pipeline. A simple and early test is to serialize the expected response and add a unit test that checks for backward compatibility (no rename or removal of properties. No type changes of properties.). 

### API client keys

All API requests are required to have a client key that identifies the calling application. This should be matched against a known list and rejected if not a registered client.

### Telemetry capture

Basic telemetry is captured and available in a central place for query. This includes things like CPU usage, disk usage, memory usage, request response times, etc.

### Story readiness gate

Stories are not even considered ready for work if there is any unclarity and have not gone through rigorous refinement.

### Ubiquitous language

Business and development use the same language to talk about software processes. These discussions happen in terms of business language and we should try not to force the business to adopt technical terms. When there is divergence, we should try to agree on common terminology. Code should reflect the ubiquitous language at all times.

### Deploy weekly

All applications should be deployed at least once a week. Even applications that are not changing should be deployed to validate the deployment pipeline.

### Dashboards II

From Dashboard I link to a dedicated dashboard the core telemetry data for that application such as memory, CPU, request count, response time, error rate. This is Dashboard II per application. Depending on space and number of applications owned you may want to add a more detailed health indicator like error rate alongside uptime on Dashboard I.

### Uptime alerts

Alerts should be setup to notify a team on a given channel if their application is unresponsive for a time. The time is dependent on SLO, based on criticality of that system, which should be found in default tags for that resource.

### Secrets in key vault

All secrets should be stored in a key vault with access following least privilege.

### Custom metrics and KPIs

The system sends custom metrics representing key business processes being fulfilled by the system. It must be possible to track and visualize KPIs based on these metrics.

### Limited WIP

By focusing on completing stories rather than maximizing work in-progress, we can add value as quickly as possible by focusing on the most valuable story. We achieve this by limiting work in progress (WIP) so development capacity -1. This means at all times someone is free to test, pair-program, handle support questions, or follow up on open questions. By helping others finish, they increase the delivery of value as well as help with knowledge share and social interaction.

### Use-case entry points

It is often useful when dealing with functionality that mutates state to wrap it up in it's own dedicated class that is not shared. This is serving a similar function that something like an application service is often serving but is more isolated. It makes dependencies per feature explicit and allows for easier testing per feature as only the dependencies for that feature are needed. Mostly, it serves as an entry point where the high-level story of how that feature works can be told. See [this post](https://devonburriss.me/managing-code-complexity/) for more discussion.

### Dashboards III

Create key Detail dashboards for a domain. These may span applications. If a domain does not span applications this can be merged with Dashboard II telemetry view.

### Acceptance tests

Acceptance tests are focused on answering the question, is this feature ready to release. You should be aiming for confidence in deploying if these tests pass.

### Design pitches

Before implementing a complete story developers are encouraged to discuss how they will be implementing a given story before implementation is underway (or very far). This gives others a chance to weigh in on the implementation details and bubble up any hidden knowledge or pitfalls. I suggest a regular prompt for this, possibly straight after standup.

### Tech debt days

Despite our best efforts, codebases can start to get away from us. This can be because of pressure to deliver, changing business requirements, or simply not knowing better at the time of implementation. Regardless, if this goes on for too long, features take longer, bugs creep in, understanding takes more effort, and morale drops. For this reason when there is a decent amount of brownfield applications, is is often helpful to dedicate a regular day (I recommend every second week) completely to improvements. A [wall of debt](http://fabiopereira.me/blog/2009/09/01/technical-debt-retrospective/) is a useful and [experiment with methods](https://verraes.net/2020/01/wall-of-technical-debt/). 

### Vulnerability scans

Implement, review, and action vulnerability scans. Cloud providers often have some sort and 3rd party providers like [Snyk](https://snyk.io/) or even [SonarQube](https://www.sonarqube.org/features/security/owasp/) at a push. Each product has it's own focus but it is important to at least start the habit of scanning, and then taking the report seriously.

### Idempotent APIs

If you want to have resilient API clients, you need first have idempotent APIs. A simple way to do this is to provide a unique identifier for each request to the API. For internal APIs the simplest way of doing this is to use client generated IDs.

### Keep current

Not bleeding edge. We upgrade packages to the latest SemVer minor version as they are released. For frameworks like .NET we use the LTS version and trail by one minor version.

### Command Query Separation

https://www.martinfowler.com/bliki/CommandQuerySeparation.html

### Dependency aware health checks

Distinguish in your health checks between a service being healthy, its dependencies for partial availability, and completely unavailable. For example if an applications database is unavailable, it cannot do anything. If on the other hand a service where it gets fresh data from that it then caches is unavailable, it can still function although with stale data.

https://github.com/beamly/SE4  
https://hootsuite.github.io/health-checks-api/

### Retro all the things

Agile practices is about learning often and folding that learning into how you behave. Scrum is a starting point but is really just a minimum bar. By taking a moment to retro every standup, every meeting, every whiteboard session, and every pair-programming session, we set ourselves up to improve at an exponentially more rapid pace than when we restrict our reflections to once a sprint. Start with standup and retro it every day. See what happens.  
https://devonburriss.me/agile-is-a-characteristic/

### Deploy daily

To drive quality up by automating and and learning rapidly, we will deploy any application that changes every day. This is still assuming some sort of manual intervention of sign-off but all intermediate steps like deployment and testing are already automated to make this just a gate.

### Honest types

Known as Value types in DDD, these are a replacement of simple types like `string` and `int` with more meaningful and smarter types. See https://devonburriss.me/honest-return-types/

### Pair programming

A pair of developers work together on a single story. One is the "driver" and is the one controlling the keyboard. The other is the navigator and is thinking through and suggesting what to do.

https://www.davefarley.net/?p=267

### Correctness alerts

Using your business metrics, monitor what is normal and alert when that normal is not met. As an example if you are monitoring the conversion rate in a step and that drops, that can indicate a problem that is not necessarily throwing exceptions but is effecting customers.

### APM tracing

Instrument your code so it is possible to visualize and measure important calls within and between services.

This article mentions the main standards:  
https://www.datadoghq.com/blog/instrument-opencensus-opentracing-and-openmetrics/

### Resilient requests

Requests between services need to acknowledge that the network is not reliable. Requests should cater for this by supporting retries. If the dependency is down, a decision needs to be made about partial availability and continuation later or rollback of changes so far.

###  Embrace immutable code

By embracing immutable types we make our code more predictable and easier to reason about. We also support doing work in parallel.

### Mob programming

Along the lines of pair programming but now involving a whole team/group. Similar rules of having a "driver" and a "navigator" are useful. This is really useful for onboarding people onto a new codebase or tackling something really problematic as you have many heads trying to solve the problem together. Another helpful techniques to use for long programming sessions: https://devonburriss.me/first-mob-programming/

### Learning days

Once low hanging fruit has been tackled it is important for people to have more freedom to learn and experiment to get gains. Having a day dedicated (say every second week) to learning will foster innovation that can often fold back into the product. 

### Synthetics

By having controlled tests running that simulate users in production it is possible to know with greater certainty what the user is experiencing. It is important that these are measured from the outside but can be tied to telemetry.

### Cyclomatic complexity ceiling

A lot of code metrics and recommendations can be debated but cyclomatic complexity is a fairly straightforward measure of the complexity. Measuring this and putting a ceiling on what is acceptable before refactoring efforts need to be focused in that area.

### Quantitative team health

Find ways to measure how the team is doing and visualize these. Then identify when the trend is going poorly and take action to turn things around. The idea here is 

- Code/test metrics
- Aspect rating

### Embrace FP principles

Functions and immutable data. Distinguish between calculations (pure functions) and actions (impure functions). 
Keep the level of abstraction consistent at different layers (stratified design).

### PR to production

Each PR to the main branch ends up in production when merged with zero manual intervention (and within a few minutes of the merge). This implies:

- trust in tests
- impeccable monitoring and alerting
- fast recovery/rollback

### Dashboards IV

The final dashboard includes non-system elements like:

- team metrics
- code metrics

![Metrics framework](images/maturity-model-2021-07-28-19-40-55.png)

 Then bring the dashboards together across system business performance, infrastructure performance, delivery performance, and team health.

 ### Load testing

 Automated test runs can be scheduled to test how the systems perform under heavy load.

 ### Immutable Architecture

- Versioned API
- Idempotent API
- Non-destructive data mutations only
- Event feeds

### Domain Driven design

- Ubiquitous language is shared and found in the codebase (Value types as a starting point)
- Strong understanding from development of processes and workflows (use EventStorming)
- Shared model with business (aggregates enforce business invariance and repositories return valid aggregates only). Model business process over structure.
- Use cases as Application Service (preferred)

For the rest use patterns as needed but keep it simple.

### Property based testing

Gain for confidence by testing the full range of a domain. Discover the characteristics of your system.

### Chaos engineering

Build resilient systems by exercising the resilience often. Automate partial outages of systems to dependent systems are forced to evolve to be stronger.
