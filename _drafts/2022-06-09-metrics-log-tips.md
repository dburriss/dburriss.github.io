
Requirements

- structured logging
- OpenTelemetry tracing (or something similar)

Examples will be in C# but I will be providing generic tips.

Tips:

- come up with metric and tagging conventions early
- build tooling in your code to try help with the standardisation
- enable metric and log correlation
- do not litter your application code with telemetry language
- log at the boundaries were actions happen

## Define metric and tagging conventions early

Anyone who has used a telemetry tool like Datadog in an organisation where the conventions are not clear will recognise the problem this is trying to solve. As teams start sending data, searching for anything becomes difficult as you sort through metrics ranging from `AcmeCoolServiceStartedUp` to `record-updated`. This can quickly get out of hand and can make searching for metrics quite frustrating. Since we want to make working with telemetry as [easy as possible], I suggest you tackle this as soon as possible.

- Use lowercase `kebab-case` for metrics and tags. Reasons: Language agnostic choice. Most readable. Ubiquitous.
- Consider namespacing if you foresee multiple complex domains with lots of unique metrics eg. `some-domain.some-app-some-metric`
- Make use of key-value pairs in tags eg. `env:prod`
- Be clear on reserved tags eg. `env:prod` and `service:app-name`
- Use `result:success` and `result:fail` tags on the same metric rather than 2 separate metrics

## Build up tooling to help with standards

Tooling for helping developers fall into the pit of success with the configuration and tagging of metrics can go a long way.

One such example is a thin wrapper around application setup that enforces the setup and sending of a service name, environment, etc. Often these things can be handled by the host environment but if not it is worth the small effort.

Types or constants that represent common tags can be an invaluable tool for having standardized set of tags that are discoverable through a developers IDE. Remember that tags should always be of a fixed set, and so capturing them in code should be possible. If you are using an almost unconstrained range like a database identifier as a tag, expect a large bill from your telemetry provider for excessive indexes.

You may want to consider a metric naming check that would take a namespace and a metric name. This could normalize the formatting before sending to ensure standard metric naming. This is probably the least beneficial of the suggestions.

## Enable metric and log correlation

The true power of modern telemetry solutions is in the correlation of metrics, traces, and logs through identifiers that allow correlation between parent and child processes, even across network boundaries. Hopefully most of you would have seen this in action already but if not, it unlocks another level of observability in your applications.

## Don't mix telemetry code and application code

## Instrument where the action happens