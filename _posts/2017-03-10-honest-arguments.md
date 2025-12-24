---
layout: post
title: "Honest Arguments"
subtitle: "Part 1 of Designing clear method signatures"
author: "Devon Burriss"
category: Software Development
tags: [Clean Code, OOP]
comments: true
permalink: honest-arguments
excerpt_separator: <!--more-->
published: true
series: "Honest Types"
topics: [engineering-practices, platforms-runtime]
keywords: [Software Development, Clean Code, OOP]
---
One of the benefits of statically typed languages is that we can rely on more than the method and parameter names for information on what is expected and what is returned. A well designed method should be about more than naming. Too often we give up on this type safety and expressiveness for the ease of instantiating primitives and `string`.

<!--more-->

# Expressively typed parameters

Consider the following 2 tips for message choice. To be fair I chose less than expressive names to demonstrate that even if a developer doesn't pick the best names (which they should of course try to do and should be fixed), the types of the argument provide all the intent needed. The parameter names could be 'l', 'f', and 'e' and a developer could still infer the usage from the types.

![primitive parameters](/img/posts/2017/primitive-typed-method.jpg)
*Figure 1: Using simple type parameters*

![expressive parameters](/img/posts/2017/expressively-typed-method.jpg)
*Figure 2: Using expressive type parameters*

So how would we represent something like a name as a type instead of a `string` but still have it play nice with the capture in a client or storage of an instance in a database?
The trick is with the `implicit` or `explicit` keywords.

## Lose the primitives (but play nice)

For types that are always a direct conversion with no chance of failing, use the `implicit` keyword.

```csharp
public class FirstNames
{
    string Value { get; }
    public FirstNames(string value) { Value = value; }

    public static implicit operator string(FirstNames c)
        => c.Value;
    public static implicit operator FirstNames(string s)
        => new FirstNames(s);

    public override string ToString() => Value;
}

//usage
FirstNames name = "Devon Aragorn";
string nameAsString = name;
```

On the other hand when you start adding a bit of behaviour into your class, there is a chance that the conversion can fail. Take for instance an `Email` type that has some validation of the email address.

```csharp
public class Email
{
    private const string regexPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    private string Value { get; }

    public Email(string value)
    {
        if(!Regex.IsMatch(value, regexPattern, RegexOptions.IgnoreCase))
        {
            throw new ArgumentException($"{value} is not a valid email address.", nameof(value));
        }
        Value = value;
    }

    public static explicit operator string(Email c)
        => c.Value;
    public static explicit operator Email(string s)
        => new Email(s);

    public override string ToString() => Value;
}

//usage
Email email = (Email)"test@test.com";
string emailAsString = (string)email;
```

Here we are using the `explicit` keyword because the constructor can throw an exception if the string is not a valid email address.

### Pros

Let's list some reasons why you would want to do this with simpler types.

* Using **expressive types reveal intent** to consumers (other developers and future you)
* **Finding usage** of particular concepts can be done by type rather than searching text
* If doing domain modelling you can now **group behavior and data** to have a descriptive model
* Once assigned to an expressive type they **provide type safety**
* Creation of more **targeted extension methods**

### Cons

As with most things in programming, #ItDepends. There are some down sides to using types this way...

* **More code** to write and maintain 
* **Serialization** requires a bit more work to do 
* **ORM mapping** could be more complicated
* Implicit conversion means you lose some type safety

Let me quickly discuss a few of these cons and how they can be mitigated.

#### More Code

Not much to do about the maintainability part. I will say that these are relatively simple and are unlikely to change or have far reaching effects due to dependencies. To address the effort of actually creating these see [Visual Studio Implicit Snippet](/visual-studio-implicit-snippet).

#### Serialization

For some help easily serializing these types check out the [Honest Types repository](https://github.com/dburriss/HonestTypes). That package provides a Json.NET Converter like `new SimpleJsonConverter<LastName, string>()` that can be supplied to the settings when serializing and deserializing.

#### ORM Mapping

If you are modelling your domain (like with DDD) which is likely the case if you are using types this way, then you shouldn't be using your domain models for persistence. This tends to tie your domain models to the underlying data model and you will find the schema requirements will start leaking into your domain model. So create models for your data layer and map from them to your domain models in the repository.

## Recommended Reading

[Functional C#: Primitive obsession](http://enterprisecraftsmanship.com/2015/03/07/functional-c-primitive-obsession/)
