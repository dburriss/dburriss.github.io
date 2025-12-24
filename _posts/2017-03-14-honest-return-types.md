---
layout: post
title: "Honest Return Types"
subtitle: "Part 2 of Designing clear method signatures"
author: "Devon Burriss"
category: Software Development
tags: [Clean Code, OOP, Functional]
comments: true
permalink: honest-return-types
excerpt_separator: <!--more-->
published: true
series: "Honest Types"
topics: [engineering-practices, platforms-runtime]
keywords: [Software Development, Clean Code, OOP, Functional]
---
In [Part 1](/honest-arguments) we looked at ways of making your code more descriptive by using custom types instead of simple types like `string`. In this article we will look at what your return type can tell you about a method.

> Updated: 19 March 2017
<!--more-->

# Honest Return Types

For most of this post let us build on the example of a `Person` repository. We are not going to dive into implementation but instead focus on the descriptiveness of the return type. Our starting point is this:

```csharp
public interface IQueryPerson
{
    Person Get(Email email);
}
```

The return type should be honest about what can happen when you call a method. Does this repository method return `null` if no record is found? Does it throw and exception? Does it return a [special case](https://martinfowler.com/eaaCatalog/specialCase.html) subtype? Wouldn't it be nice if your return type could tell you this instead of you having to dig into the implementation to find out.

My 2 criteria are:

1. A return type should be really descriptive of what the possible outcomes are
2. The interface for interacting with a type should make it difficult for developers to do the wrong thing


## Result: A first try

One solution is a `Result<T>` or some such flavour. It might look something like this:

```csharp
public class Result<T>
{
    public T Value { get; set; }
    public bool IsSuccess { get; set; }
    public IEnumerable<string> Errors { get; set; }
    public Result()
    {
        Errors = new List<string>();
    }

    public Result(T value)
    {
        if(value == null)
        {
            IsSuccess = false;
        }
        else
        {
            IsSuccess = true;
            Value = value;
        }
    }
}
```

This could be written in slightly different ways, with error codes instead of string for Errors, or even `Exception`. Let's discuss the pros and cons of this.

### Pros

* It does acknowledge that something could go wrong
* Can return some error and state information without throwing an exception (read unexplicit `goto` statement)

### Cons

* It is not descriptive about what represents a failure
* Value can be accessed without checking for success
* The type doesn't convey whether `null` could still be a valid value

So it is something but doesn't really fulfill either of my criteria very well. We are going to have to take a quick sidebar and talk about representing `null`. `Result<T>` doesn't tell us whether we should expect `T` to be `null` and whether that is valid.

## Functional side-bar

In functional terms an elevated type is like a wrapper. It is a higher level of abstraction that allows us to work with the type in a predictable way. `IEnummerable<T>`, `Option<T>`, `Exception<T>`, `Either<L. R>`, `Validation<T>` are all examples of elevated types.

## Option: `null` is None

"It depends" is something you hear a lot in development, and wouldn't it be great if a type conveyed this? `Option` or `Maybe` are types often found in more functional languages that highlight the fact that a value could not be present. It allows you to say that there is `Some` value, or the value is `None`. This is probably easier to demonstrate...

> I am using [LanguageExt](https://github.com/louthy/language-ext) to get some more functional types. This one is mature and fully featured but pick whatever works for you.

```csharp
public Option<Person> Get(Email email)
{
    Person person = QueryByEmail(email);//person could be null if no matching email found in the datasource
    return person;
}

//usage example
var person1 = personRepository.Get(email);

//print out last name if person was found otherwise print "Nobody"
person1.Match(
    Some: p => Console.WriteLine(p.LastName),
    None: () => Console.WriteLine("Nobody")
);

//return fullname or Nobody if no one was found
var person1Name = person1.Match(
    Some: p => $"{p.FirstNames} {p.LastName}",
    None: () => "Nobody"
);
```

The implementation uses `implicit` conversion to return `None` if the value is `null` otherwise the `Person` is elevated with Some.  
I explicitly elevate the result to demonstrate what is happening. Let's also add some error-handling as this will show a problem.

```csharp
using static LanguageExt.Prelude;
public Option<Person> Get(Email email)
{
    try
    {
        Person person = QueryByEmail(email);
        if(person == null)
            return None;
        return Some(person);
    }
    catch (Exception)
    {
        return None;
    }
}
```

So this is looking a little better.

### Pros

* Return type is explicit about possibility of no value being returned
* The API of the type encourages handling of branch between happy and unhappy path

### Cons

* We cannot differentiate between no value and an exception

## Exception: return don't throw

> The following `Exceptional<T>` and `Validation<T>` types are defined in [HonestTypes](https://github.com/dburriss/HonestTypes). Check the project page for installation instructions.

So our type needs to be a bit more explicit about what can happen. Let's introduce an `Exceptional<T>` type. 
This is similar to `Option<Person>` but instead of **Some** and **None** it has **Exception** and **Success**.  
For those of you familiar with functional programming it is basically `Either<Exception, T>` with left set to `Exception`.

```csharp
public Exceptional<Option<Person>> Get(Email email)
{
    try
    {
        Person person = QueryByEmail(email);
        Option<Person> result = person;
        return result;
    }
    catch (DbException ex)//only catch expected exceptions
    {
        return ex;
    }
}

//usage
var person1 = personRepository.Get(email);

person1.Match(
    Exception: ex => Console.WriteLine($"Exception: {ex.Message}"),
    Success: opt => opt.Match(
        None: () => Console.WriteLine("Person: Nobody"),
        Some: p => Console.WriteLine($"Person: {p.FirstNames} {p.LastName}")
    )
);
```

One important point in the repository implementation is you need to assign it to `Option<Person>` before returning it which implicitly converts to `Exceptional<Option<Person>>`. 
You can't go directly from `Person` to `Exceptional<Option<Person>>` unfortunately.  

The difference in this implementation is in the exception handling. See how we just return the exception? The exception has an implicit conversion to the elevated type of `Exceptional<T>`.

### Pros

* Return type is very explicit about both errors and no value
* API of return type encourages good handling of code paths

### Cons

* With the nested generics the type declaration is quite verbose

## Conclusion

So with a bit of borrowing from functional programming and some added verbosity to our method signature we managed to move from an admittedly simple signature to a slightly more verbose one that is brutally honest about the possible outcomes.

```csharp
Person Get(Email email);
Result<Person> Get(Email email);
Option<Person> Get(Email email);
Exceptional<Option<Person>> Get(Email email);
```

I hope you found something useful in this and if you did I cannot recommend enough the brilliant [Functional Programming in C#](https://www.manning.com/books/functional-programming-in-c-sharp) from Manning. I must warn that some of the chapters in this book are heavy going. Not because they are badly written but because as a C# and Java developer the concepts are so foreign that they take a while to sink in. Like most things worthwhile it takes effort and determination but you will be a better developer for it.

In my following post I will discuss [error handling](/better-error-handling) and how logic/validation errors can be represented as return types following the same criteria as in this post.

## Recommended Reading

1. [Elevated world](https://fsharpforfunandprofit.com/posts/elevated-world/)
2. [Railway oriented programming](https://fsharpforfunandprofit.com/rop/)
