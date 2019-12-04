---
layout: post
title: "Reliability with Intents"
subtitle: "Telling the world about state changes"
description: "Looking at problems and solution with sending messages before and after changing internal state"
permalink: reliability-with-intents
author: "Devon Burriss"
category: Software Development
tags: [F#,Clean Code,Architecture,Messaging]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/path-bg.jpg"
social-img: "img/posts/2019/target-500.jpg"
published: true
---
If you are using any kind of messaging architecture to notify outside of your system of internal changes you may have noticed a reliability problem. Unless you are using distributed transactions to ensure atomic operations you may have noticed an ordering problem in updating state and notifying the rest of the world. In this post, I will look at this problem and a possible solution.
<!--more-->

> This post is part of [#FsAdvent 2019](https://sergeytihon.com/2019/11/05/f-advent-calendar-in-english-2019/). PS. THIS IS NOT PRODUCTION WORTHY CODE! FOR DEMO PURPOSES ONLY!

## The atomic problem

Oft times when doing an operation in an application, I see a call to put some kind of message on a queue (or topic) to notify other systems that this event occurred.

```fsharp
// save to database
// it could then fail
// put on queue
person
|> Data.createPerson dbConnection None
|> tap (fun _ -> failwith "Failed before sending message") // <-- simulate application crash
|> Result.bind (Message.personCreated queue)
```

What happens though if the application crashes right after saving some changes to the database? Your application has changed state but has not, and will not, notify the rest of the world about that change. What if other business processes rely on this?

![persist state then send](/img/posts/2019/intents-1.png)

> If you are thinking that the chances of this happening are vanishingly small, let me float this idea. A 99.99% uptime still means almost an hour of downtime a year. On a high load system in the cloud (chaos monkey as a service), systems can disappear more often than you think.

I have seen businesses be unaware of this communication loss for months, where the result is customer service calls routed to teams dependent on that message. The problem here is both assume every message is sent, never considering the loss. Only once these numbers were monitored to the problem become apparent.

So back to the problem. Of course, reversing the order does not help.

![send then persist state](/img/posts/2019/intents-2.png)

Now you are notifying the world about a change that in effect never happened.

## What is your intention?

I will mention a few more sophisticated variations in the conclusion but the solution is fairly simple. Separate the intention of sending the notification from the actual sending. 

F# discriminated unions give a nice way to define our intention, as it is a state machine.

```fsharp
// domain type
type Person = {
 id:string
 name:string
 email:string
}

// Here the type of case could be the entity, command, or the message to be sent. 
// Whatever makes the most sense.
type IntentOfPersonCreated = 
| Pending of Person
| Complete of Person
```

We can then save the intention to send the message in a transaction with the state change that is prompting the notification.

```fsharp
// save to database with intent
// intent puts on queue
use transaction = dbConnection.BeginTransaction()
let txn = Some transaction

person 
|> Data.createPerson dbConnection txn
|> Result.map (fun p -> Data.createPersonIntent dbConnection txn (Pending p))

transaction.Commit()
```

Finally, you need to process all persisted intents.

```fsharp
let handleIntent connection queue (id,intent) =
 // handle each state of the intent
 match intent with
 | Pending person -> 
 Message.personCreated queue person |> ignore
 Data.markCreatePersonIntentDone connection id (Complete person) |> ignore
 printfn "%A intent sent" person
 | Complete _ -> failwith "These should not be queried"
 

let processIntents (dbConnection:DbConnection) queue =
 let intentsR = Data.getCreatePersonIntents dbConnection
 match intentsR with
 | Error ex -> raise ex
 | Ok intents -> intents |> Seq.iter (handleIntent dbConnection queue)
```

Now as long as you have a process that is regularly running through and processing the **intents**, you can guarantee that as soon as all infrastructure is healthy, all notifications will be sent at least once.

![transactional persistence of state and intention](/img/posts/2019/intents-3.png)

## Implementation ideas

All the DEMO code is [available on my GitHub](https://github.com/dburriss/intent-blog) but I wanted to talk about a few implementation details and what you may want to do differently.

This is the table I am storing the **intents** in. I am using `iscomplete` to filter out the **intents** I do no longer need to process. `intenttype` allows me to use this table for multiple **intents** and treat each differently. `intent` is a JSON string of the serialized **intent**.

```sql
CREATE TABLE IF NOT EXISTS intents (
 id INTEGER PRIMARY KEY AUTOINCREMENT,
 iscomplete INTEGER NOT NULL DEFAULT 0,
 intenttype TEXT NOT NULL,
 intent BLOB NOT NULL
);
```

For production, you will likely want to add some indexes. Another thought I had was a partition key that could be used to process the intents from multiple consumers. This way you could scale out consumers even if the order was important for related **intents**.

You can check out the usage of this on the [GitHub](https://github.com/dburriss/intent-blog) repository, specifically `Data.fs` but the following code should give a sufficient peek under the hood to get you going.

```fsharp
let createIntent (connection:#DbConnection) (transaction:#DbTransaction option) (intent:string) (type':string)=
 let data = [("@intent",box intent);("@intenttype",box type')] |> dict |> fun d -> DynamicParameters(d)
 let sql = "INSERT INTO intents (intent,intenttype) VALUES (@intent,@intenttype);"
 execute connection sql data transaction

let createPersonIntent (connection:#DbConnection) (transaction:#DbTransaction option) (intent:IntentOfPersonCreated) =
 let intent' = intent |> JsonConvert.SerializeObject
 createIntent connection transaction intent' "create-person"
```

## Conclusion

Of course, increasing the reliability of your system comes at the cost of a bit of added complexity, as well as a speed penalty for the outgoing notifications. I will say that on top of the reliability increase, you also get a fairly good audit log without having moved to Event Sourcing (no I am not saying auditing alone is a good reason to do ES).

Another useful design choice that is related here is collecting events as your code executes. If you are using a functional style of programming, always returning events is the way to go. If you are using a more imperative style using classic DDD techniques, an aggregate root is a good place to accumulate these events. Erik Heemskerk has a great post [highlighting how a team we worked in together used this](https://www.erikheemskerk.nl/ddd-persistence-recorded-event-driven-persistence/).

I did want to acknowledge that the processing of the intents does have some challenges that I have not covered in this post. You want to try to avoid having multiple workers pulling the same kind of **intents** or the number of duplicate messages will explode. You should always cater for duplicate messages as EXACTLY ONCE message delivery is a pipe dream. Having a single instance processing means it can easily go down, so monitoring and restarts are important for the health of your system. A product like [Hangfire](https://www.hangfire.io/) may be useful here, or scheduled serverless functions. Your mileage may vary.

Finally, I did want to also point out a [great talk of Erik's](https://www.youtube.com/watch?v=FkDZw9HmwQY&list=FLtCKfk3-Xz9K1kCkvT_v6aQ) where he talks about turning this around so consumers came to get the events from you. If you want to send out notifications you can write the consumer of your event feed that notifies... or just tell people to come and fetch and be done with all this headache.

## Credits

- Photo by Jens Lelie on [Unsplash](https://unsplash.com/photos/u0vgcIOQG08)