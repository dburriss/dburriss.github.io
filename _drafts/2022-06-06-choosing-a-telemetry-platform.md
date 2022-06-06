---
layout: post
title: "Choosing a telemetry platform"
subtitle: "A look at some of the non-obvious points to consider"
description: "When looking at a telemetry platform we will often look at support for a cloud provider, specific technologies supported, or maybe price. Something often not considered is usability."
permalink: choosing-a-telemetry-platform
author: "Devon Burriss"
category: Agile
tags: [Datadog,Observability]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2019/target-500.jpg"
published: false
---
Recently we decided to make the switch from using Azure Application Insights as our primary telemetry monitoring tool, to [Datadog](https://docs.datadoghq.com/). I wanted to drop a few thoughts on why this was a good choice so anyone else looking to make this decision could take a few more aspects into consideration.
<!--more-->

When choosing a new tool, it is easy to get get caught up in the technical requirements. Don't get me wrong, these are VERY important and it is important to do your homework. There is nothing worse that choosing a tool and then only noticing afterward it had a huge gap in functionality.

For us some of the broad technical requirements were:

- [x] Good integration with Azure
- [x] Web App Service monitoring
- [x] MS SQL monitoring
- [x] Support for OpenTelemetry tracing
- [x] Integrated logging and custom metrics
- [x] Security alerting
- [x] Alerting and integrations ie. Slack, PagerDuty, etc.

The things is, we were using Application Insights, and it does tick all of these boxes. So why change?

I can summarize it with this statement.

> A tool is useless if no-one wants to use it!

Let me phrase it another way. To get adoption of any tool or practice (monitoring is both), the best thing you can do is make it easy and immediately beneficial.

I have been at at least 3 companies where Datadog has been introduced. What I have noticed is that before it's introduction team's do not often build dashboards, and so team's do not use dashboards. Why? Well in most tools, dashboards are complicated to make, don't look great, and often don't have all the needed data. To be fair, not all of this is because it cannot be done. It doesn't help if the tool always feels like it is getting in the way.

## A challenge

I decided to create 2 similar dashboards in both Datadog and Application Insights. The idea was to create a basic application dashboard that gave some high level overview and then a dive into some health metrics.

### Datadog

The following dashboard took me 10 minutes to design and build with no prior knowledge of what I wanted on the board, or what metrics I would use.

![Datadog dashboard](../img/posts/2022/2022-06-06-13-21-12.png)

### Application Insights

The Application Insights dashboard took me just over 15 minutes to create, with idea being of following the same or similar design use on the Datadog one.

![Application Insights dashboard](../img/posts/2022/2022-06-06-13-24-31.png)

### Critique of the experience

Datadog allows you to explore the metrics data and resulting graph in real time. This makes for an interactive experience that can allows you to learn what is possible while experimenting with different types of visualization. It also allows for the quick creation of graphs and easy tweaking of data for better insights,  or fixes of mistakes.

To contrast with this, Application Insights really makes you decide on your design upfront. A change usually means deleting a graph and choosing a new tile type. Editing the graph requires change into different modes and saving in a way that is really unintuitive. Lastly, and the most costly, is the fact that instead of just searching for metrics, you have to choose upfront whether you want metrics from a service, database, AI instance, etc. before being able to explore what metrics are available. Each time you need to pick a resource you need to drill down from the subscription/resource group/ resource layer in a tedious and time consuming way.

## Conclusion

I am in no way affiliated with Datadog. I am clearly a fan of the product though. I have seen it succeed many times to up the level of monitoring in a company and I think this is in no small part due to it's excellent UI and the way it let's a user explore what is possible. One great thing not mentioned is that the types of products available in the Datadog suite is ever growing and the integration that they have with each other is great. This does also bring up one point to keep in mind. With Datadog becoming a 1 stop shop for metrics, APM, logging, security, etc. it can be overwhelming. I suggest starting with one or two and expanding slowly. So do you find yourself in the position where you wish you had a better handle on not only the errors in your system but also what normal behaviour looks like? Maybe you need to ask whether your tooling is holding you back...