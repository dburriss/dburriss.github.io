# Moving to topics

The goal is **one flat concept** that is human-readable, stable over time, and UI-friendly. Avoid language/tool churn driving taxonomy.

### First: critique of current taxonomy

You currently have:

* **Categories** = very broad, overlapping, and inconsistent in abstraction level.
* **Tags** = mixture of:

  * Languages (.NET, Java, F#)
  * Frameworks (ASP.NET, MVC)
  * Practices (TDD, BDD, DDD)
  * Tools (Datadog, Docker, VS Code)
  * Meta/life topics (Meditation, Morality)

This creates:

* Redundant classification (Agile vs XP vs TDD).
* Time decay (specific tools age badly).
* Poor browsing value (readers don’t click “Nuget” to explore ideas).

This strongly argues for consolidation.

---

### Principle for reduction

Group by **intent and mental model**, not technology.

A good topic should answer:

> “Why would I read multiple posts under this?”

Not:

> “What library/version is this about?”

---

### Proposed reduced topic set (8 total)

#### 1. **Software Design**

Covers:

* Architecture
* DDD, C4, modeling, diagrams
* Design patterns
* Temporal / functional modeling
* Distributed systems

Maps from:
Architecture, DDD, C4, Modeling, Design Patterns, Distributed Systems, Temporal Modeling, Functional Modeling

---

#### 2. **Engineering Practices**

How we build and validate software.

Covers:

* TDD, BDD, ATDD
* Clean Code, SOLID
* Testing, Unit Testing
* XP, Agile (practice-level, not org-level)

Maps from:
TDD, BDD, ATDD, Clean Code, SOLID, Testing, Unit Testing, Extreme Programming, Agile

---

#### 3. **Platforms & Runtime**

The execution environment, not the tools.

Covers:

* .NET, Java, F#, runtimes
* Messaging
* OS-level concerns when relevant

Maps from:
.NET, .NET Core, ASP.NET, Java, F#, Messaging, OOP, Functional

This intentionally avoids versioning and framework sprawl.

---

#### 4. **Tooling & Automation**

Things that support development, not concepts.

Covers:

* CI/CD
* Build tools
* Editors
* Automation scripts

Maps from:
Docker, Git, AppVeyor, Cake, Maven, Paket, Powershell, VS Code, Visual Studio, Github Pages

---

#### 5. **Reliability & Observability**

Production-facing concerns.

Covers:

* Monitoring
* Metrics
* Reliability
* Failure modes

Maps from:
Observability, Datadog, Prometheus, Reliability, Idempotent

---

#### 6. **AI & Agentic Systems**

Clearly distinct and future-facing.

Covers:

* AI, LLMs
* Agentic systems
* Prompting, workflows

Maps from:
AI, LLMs, Agents, Agentic, OpenCode

This deserves separation due to pace of change and reader interest.

---

#### 7. **Leadership & Teams**

Human systems around software.

Covers:

* Team lead
* Leadership
* Project management
* Org design

Maps from:
Leadership, Team Lead, Project Management, Product (where relevant)

---

#### 8. **Notes & Reflections**

Intentional “catch-all”, but framed.

Covers:

* Learning
* Career
* Ethics
* Life-adjacent thinking

Maps from:
Life, Learning C#, Meditation, Mindfulness, Morality, Science, Productivity

This prevents taxonomy pollution elsewhere.

---

### What I would explicitly drop

Do **not** promote these to first-class topics:

* Specific libraries (NuGet, Selenium)
* Versions (MVC 6, ASP.NET 5)
* Minor tools (Pretzel, Hyper-V)

They can exist as **inline tags or text**, not navigation.

---

### Sanity check

This set:

* Is stable over years.
* Works visually in a minimal UI.
* Keeps topics <10.
* Matches how experienced engineers think.


