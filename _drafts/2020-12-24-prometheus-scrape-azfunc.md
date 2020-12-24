---
layout: post
title: "Capturing custom business metrics in Azure Functions"
subtitle: "A Proof-of-concept for scraping Prometheus metrics from Azure Functions"
description: "A simple example showing how to expose a Prometheus endpoint from Azure Functions using the Fennel library"
permalink: azfunc-prometheus-endpoint
author: "Devon Burriss"
category: Software Development
tags: [F#,Monitoring,Prometheus,FsAdvent]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/dashboard-bg.jpg"
social-img: "img/posts/2018/flame-500.jpg"
published: true
---
For years now I have noticed a blind-spot when using serverless functions and observability platforms like Datadog. Custom metrics. Observability tools are constantly improving their integrations with cloud providers but are still not on par with having access to the OS like with VMs or containers. In this post I explore a little proof-of-concept I did to get custom metrics out of Azure Functions.
<!--more-->

> This post is part of [FsAdvent 2020](https://sergeytihon.com/2020/10/22/f-advent-calendar-in-english-2020/).

## How it started

A couple years back I explored solving this with a [custom binding](https://github.com/dburriss/DatadogAzureFunctions) to Datadog but it was a naive implementation that just called Datadog's HTTP API. About a year ago I had the idea of scraping these metrics using Prometheus but at the time I couldn't find a library that easily allowed me to "speak Promethean". The .NET libraries I found didn't seem to allow you to create or parse Prometheus logs, instead handling things from end-to-end. Usually as middleware.

## Clearing the path

So about 7 months back I created a small library called [Fennel](https://github.com/dburriss/fennel) which has a very simple purpose. Parse Prometheus text to objects, and turn these metric objects into valid Prometheus text. This gave me the building block I needed to easily try my experiment.

You can find my [post on Fennel here](/prometheus-parser-fennel).

## Taking the steps

![Design for scraping metrics from Azure Functions](/img/posts/2020/azfunc_prom_setup.jpg)

So my idea is fairly simple. In any function that needs to emit metrics, use a Azure Function binding to write them to some store. I chose an Azure Storage Queue for simplicity but I need to post a disclaimer at this point:

> This is demo code hacked together in an evening and does not consider the following very important production quality points:
> 1. Longer persistence of the metrics
> 1. Multiple consumers of the metrics
> 1. Enforcing ordering if more than 1 function instance is running
> 1. Resilience and sending custom metrics only if state has changed
> 1. This ignores a lot of the more complex things Prometheus exporters do
> 
> The code will be available on my [GitHub](https://github.com/dburriss). 

As a reminder, the Prometheus format is a text based format.

```text
# This is a comment but the following 2 have meaning
# HELP http_requests_total The total number of HTTP requests.
# TYPE http_requests_total counter
http_requests_total{method="post",code="200"} 1027 1395066363000
http_requests_total{method="post",code="400"}    3 1395066363000
```

```fsharp
// The builder ensures that a metric has HELP and TYPE information when written to a string
let metricsBuilder = PrometheusLogBuilder()
                        .Define("sale_count", MetricType.Counter, "Number of sales that have occurred.")

[<FunctionName("MetricsGenerator")>]
let metricsGenerator(
                    [<TimerTrigger("*/6 * * * * *")>]myTimer: TimerInfo,
                    [<Queue("logs")>] queue : ICollector<string>) =
    // random number for some sales coming in
    let sales = Random().Next(0, 50) |> float
    // use the builder to return a string of the array of metrics using Fennel under the hood
    let promLogs = metricsBuilder.Stringify [|
        Line.metric (MetricName "sale_count") (MetricValue.FloatValue sales) [] None
    |]
    queue.Add(promLogs)
```

Next create a HTTP Azure Function to serve as the `/metrics` endpoint to be scraped by Prometheus.

```fsharp
[<FunctionName("metrics")>]
let metrics ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)>]_: HttpRequest) =
    async {
        // get queue client
        let queueName = "logs"
        let connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process)
        let queueClient = QueueClient(connectionString, queueName)
        if queueClient.Exists().Value then
            // pull from queue and build up a string to return
            let messages = queueClient.ReceiveMessages(Nullable<int>(32), Nullable<TimeSpan>(TimeSpan.FromSeconds(20.))).Value
            let sb = StringBuilder()
            let processMessage (msg : QueueMessage) =
                let txt = Encoding.UTF8.GetString(Convert.FromBase64String(msg.MessageText))
                sb.Append(sprintf "%s\n" txt) |> ignore
                queueClient.DeleteMessage(msg.MessageId, msg.PopReceipt) |> ignore
            messages |> Array.iter processMessage
            let responseTxt = sb.ToString().TrimEnd()
            // return as content matching what Prometheus expects
            let response = ContentResult()
            response.Content <- responseTxt
            response.ContentType <- "text/plain; version=0.0.4"
            response.StatusCode <- Nullable<int>(200)
            return response :> IActionResult
        else return NoContentResult() :> IActionResult
        
    } |> Async.StartAsTask
```

Nothing too interesting here other than the `ContentType` being "text/plain; version=0.0.4" as per Prometheus specification.

## How it's going

Having the metrics endpoint up, all that is left is to [setup a local Prometheus instance](/setup-local-prometheus) to call our Azure Function.

Looking at Prometheus' UI at `http://localhost:9090/graph` we can query for `sale_count` and we can see the metrics are coming in:

![Prometheus graph](/img/posts/2020/prometheus_sale_count.png)

At work we use Datadog and it turns out the [Datadog agent has support for scraping a Prometheus endpoint](https://www.datadoghq.com/blog/monitor-prometheus-metrics/). Once we have the [Datadog agent setup](/datadog-prometheus-scraping) we can see the metrics flowing into Datadog.

![Datadog metric from Prometheus](/img/posts/2020/datadog_sales.png)

## Conclusion

This was a quick proof-of-concept of whether this approach was worth pursuing. I intend to take it further by running this in Azure and have a container with an agent reach out for metrics. It is unfortunate that the workarounds described here are necessary at this point but if we want a view on business metrics, we need to get creative. What I do like about this approach though is that it leverages bindings as well as Prometheus' scraping model, so not much had to be re-invented here. I am sure in the future we will see better baked in solutions for this but for now we work with what we have.

<!-- 
https://prometheus.io/docs/prometheus/latest/configuration/configuration/#scrape_config

docker run --rm -it -p 9090:9090 -v /Users/dburriss@xebia.com/GitHub/Fennel.MetricsDemo/prometheus.yml:/etc/prometheus/prometheus.yml prom/prometheus

check its up: http://localhost:9090/graph

https://www.datadoghq.com/blog/monitor-prometheus-metrics/

https://medium.com/@jeffhollan/ordered-queue-processing-in-azure-functions-with-sessions-c42ee21e689d

https://michaelscodingspot.com/performance-counters/ -->

<span>Photo by <a href="https://unsplash.com/@_ggleee?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText">Gleb Lukomets</a> on <a href="https://unsplash.com/s/photos/flame?utm_source=unsplash&amp;utm_medium=referral&amp;utm_content=creditCopyText">Unsplash</a></span>