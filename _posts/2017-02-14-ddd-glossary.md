---
layout: post
title: "Domain-Driven Design Glossary"
subtitle: "Summary of the main terms in Domain-Driven Design"
author: "Devon Burriss"
comments: true
permalink: ddd-glossary
excerpt_separator: <!--more-->
header-img: "img/backgrounds/hole-bg.jpg"
published: true
topics: [software-design]
keywords: [Software Development, DDD, Modeling]
---

---
<div class="row">
  <div class="col-xs-6 col-md-3">
    <a href="https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215/ref=as_sl_pc_tf_mfw?&linkCode=wey&tag=wwwnervstucoz-20" class="thumbnail">
      <img src="/img/posts/2017/blue-book.jpg"/>
    </a>
  </div>
DDD cannot be summarized in a few paragraphs. In fact it would take a few books to cover it thoroughly. 
Even then like anything worthwhile it requires much practice and many mistakes to start to become proficient at it.
This is how it is with most skills that add a lot of value.

A good start would be reading Eric Evans' [Domain-Driven Design: Tackling Complexity in the Heart of Software](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215/ref=as_sl_pc_tf_mfw?&linkCode=wey&tag=wwwnervstucoz-20).

It is worthwhile being familiar with some of the common terms thrown around in DDD.
</div>

<!--more-->

## What is DDD not?

DDD is not:  

* Calling your area of work a Domain
* Modelling the state of objects required into a bunch of [anemic models](http://www.martinfowler.com/bliki/AnemicDomainModel.html)
* Services containing logic that act on the anemic models
* A giant ball of interconnected objects where every class in your project has a reference somehow to every other

## What is DDD?

DDD is about modelling, and more. It encompasses common language, techniques, patterns, and architecture. It puts the focus on the business and modelling the problems you are solving. All the while giving developers techniques for minimizing the complexity and driving collaboration.
It is about taking requirements and really mapping the business processes to the model using the same language the business uses in your code.
It also gives us a common technical language to use for the different categories of classes we create while modelling our problem space.

## Glossary of terms

### Ubiquitous language

The term *Ubiquitous language* is thrown out occasionally in DDD discussions but ironically itself is often not discussed. It is also the part often left out from the development side which means the heart of DDD is not being followed and instead some of it's technical approaches used (often incorrectly).  
It is the practice of **using the terms used throughout the business within the codebase**, and working new terms from the modeling back into the business. Language often evolves and the codebase should evolve with the language. The essence really of DDD is that your code models the processes within the business and if you are not starting with the same language then how descriptive can it really be. If a product owner is looking at the application code he should recognise the classes, methods, and variables as models, workflows, and actions that actually occur.

It is not a one-way street however. Often the business has over-loaded terms, or a multiplicity of terms used for the same thing. Work with the them to define a glossary of terms that is used everywhere (ubiquitously). 

### Bounded context

The *Bounded context* is the context in which the *Ubiquitous language* and the corresponding models are valid. As developer it is a common trap to fall into to try reuse code and concepts across contexts. This is a recipe for disaster since the terms and verbs used to describe a model in one context will likely be similar but not the same. This results in blurring of the model to cater for both. This adds confusion as well as inviting changes with unintended consequences. This is especially true when a model is shared across more than one team (strongly concider whether it really is one context).

#### Example

Imagine a Product class in the Logistics domain. For tracking around the warehouse you need a barcode, for shipping you need the packaged dimensions and weight. Now think of a product for display on an e-commerce website. You need photos, description, and other specs like its actual dimensions unpacked.  
Why would a developer need all of this at one time? Why confuse matters? Why would the clients of the code like a scanner in the warehouse or a customer on the website need both? If all that is shared is maybe a name and a SKU, is the code sharing of 2 properties worth coupling different parts of the system? Different teams together?  
There are many reasons to want to keep these models seperated based on their context and few reasons to combine them. Yet it is a very common occurance in development. Why? Code re-use.  
Only re-use models if they are indeed the same model. 

### Entities

Entities are the classes that model the domain concepts and have identity. This usually means there is a unique primary key associated with the entity. Remember that modelling in DDD takes us back to the OOP we learned in the text books... behavior and data together. This is in antithesis to the usual [anemic models](https://martinfowler.com/bliki/AnemicDomainModel.html) found in most software.

### Value objects

Value objects are much like entities except they do not have identity. Money is the quintessential example of a model that shows intent, contains rules, but does not have identity. The important part here is using types to convey meaning as well as place logic along with the data in a very obvious way.

### Aggregate

An Aggregate is a hierarchy of objects (Entities and Value objects) that make up a consistency boundary.  
Why would we want to set a boundary rather than just reference any object needed?  

Minimising associations helps to prevent a reference web. This can be problematic when fetching and reconstituting a hierarchy of objects into memory. Lazy loading can quickly get out of hand, alternatively null references about and conntinually need to be checked.

Let us turn the question around. What if the relationships of our object model clearly showed us the effects of change? For example, the aggregate was the scope of the transaction...

#### Aggregate root

The Aggregate Root is an Entity that all other Entities and Value Objects in the hierarchy hang off. For example if you have an Order with Order Lines and a Supplier, the `OrderRepository` will return an Order with all `OrderLines` and `OrderSupplier` populated. If would not be possible to fetch an `OrderLine` separately, nor a `OrderSupplier`. If needed though you would provide methods on your `OrderRepository` to fetch an order by Order Line Id or by Supplier Reference for example.

#### Points to keep in mind

- Technical difficulties implementing an aggregate (like transaction issues persisting it) are usually indicative of a poorly chosen model. Put more effort refining the model rather than trying to fix a modelling problem with a technical implementation.
- Access to objects from outside the aggregate must occur through the Aggregate Root.
- Aggregates are always constructed in a consistent state.
- The logic is usually within the aggregate to disallow consistent state or at least check its consistency.
- It is better to encapsulate changes to state through method calls rather than directly mutating properties. This shows intent as well as adds an extra layer of indirection allowing implemntation changes without changing the API.

### Factories

Since an aggregate should always be in a consistent state it is important that they are constructed in a consistent state to the user. Factories provide a way to **ensure that new instances of an aggregate always start in a consistent state**.

### Repositories

Repositories protect us from taking a data-centric view of our code. They allow us to **persist and retrieve aggregates** without dealing directly with the underlying persistence. It is however important for developers to at least be aware of the underlying implementations so as not to abuse the repository from a performance or scoping way.

The abstraction of the repository is contained within the domain. This abstraction knows about the domain models within that context. More specifically it knows about the aggregate that it is returning. A repository returns an Entity (or collection of Entities) and the aggregate for wich that Entity is the Aggregate Root.

The implementation of the repository abstraction does not reside in the domain. It is a Infrastructural concern and can change. What is important though is that the repository handles mapping however the data is persisted into a fully hydrated and consistent aggregate.

The developer is free to add multiple query methods to the repository but the return results are always in terms of the Aggregate Root.

#### Points to keep in mind

- The repository abstraction is part of the domain
- The repository implementation is NOT part of the domain
- The repository exposes data in terms of that repository's Aggregate Root
- Query methods should use the domain language
- If complex queries look to encapsulate in query objects using the [Specification](https://www.martinfowler.com/apsupp/spec.pdf) pattern
- Transaction should be controlled by the client code

### Domain Service

> Sometimes, it just isn't a thing.

When modeling sometimes an operation or workflow doesn't fit into the current model. Usually this just means you are not accurately capturing the model you need to represent the business problem but every now and again it is valid to place this operation in a domain service. If placing a workflow comflates your model objects maybe a service is the way to go. Services are represented by verbs rather than nouns and speak to what the DO. An important distinction from model objects is that they are completely stateless. A service will take various other domain objects and execute some action, possibly returning some result.

#### Points to keep in mind

- Don't give up too quickly trying to fit an operation into the model (concider a new concept that encapsulates entities and values objects... maybe this is actual aggregate root?)
- The Service is named after an activity (verb not noun)
- Services are stateless
- Services still use the Ubiquitous Language

### Application Service

The application service is what presents an input for a use-case. It calls off to the domain for execution, calls any other services (like notifications) and returns. This could be something like a WebApi controller in .NET or you could choose to explicitly create an an application service. 

#### Points to keep in mind

- A thin layer that receives a request and passes it to the domain to processes
- Think use-case
- A good place to handle transactions
- Can call out to Infrastructure Services

### Infrastructure Service

This is a technical implementation for something that performs some task such as notifications (IM, email, etc.), put messages on a bus, or retrieve some data from another system.

### Anti-corruption layer (ACL)

An ACL is at the very least a thin translation layer between two bounded contexts. Even if both bounded contexts are well defined, and share similar models. The models in one context should not influence the models in another and without a layer in between to translate between the two corruption will creep in. If the external system a bounded context is talking to is a legacy system with a very poor model it is even more likely it will corrupt unless the ACL acts as a strong buffer.

### Modules

Modules are simply packages or assemblies. Whatever your technology's means is of bundling built code is.

### Shared Kernel

Sometimes a model needs to be shared across multiple Bounded Contexts. If so a Shared Kernel can be created but in a lot of cases the coupling created between the contexts and the teams is not worth it. 

### Clients

This is not really a term from the *Blue Book* (that I remember) but I find it useful when talking about DDD and Clean Architecture. Clients are the callers of the application layer. These could be another application automated service or an application been driven by a user. Regardless the clients execute the use-cases defined in the application layer.

### Further reading

1. [Strengthening your domain](https://lostechies.com/jimmybogard/2010/02/04/strengthening-your-domain-a-primer/)
2. [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
3. [Services in Domain-Driven Design](http://gorodinski.com/blog/2012/04/14/services-in-domain-driven-design-ddd/)
4. [Domain-Driven Design: Tackling Complexity in the Heart of Software](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215/ref=as_sl_pc_tf_mfw?&linkCode=wey&tag=wwwnervstucoz-20)
5. [Implementing Domain-Driven Design](https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577/ref=pd_bxgy_14_img_2?_encoding=UTF8&pd_rd_i=0321834577&pd_rd_r=P6PNCC27GC5B7Q513JJ4&pd_rd_w=6neVY&pd_rd_wg=Rn8gy&psc=1&refRID=P6PNCC27GC5B7Q513JJ4)  
6. [Applying Domain-Driven Design Patterns Examples](https://www.amazon.com/Applying-Domain-Driven-Design-Patterns-Examples/dp/0321268202/ref=as_sl_pc_tf_mfw?&linkCode=wey&tag=wwwnervstucoz-20) 

