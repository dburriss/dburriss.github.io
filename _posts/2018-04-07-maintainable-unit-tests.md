---
layout: post
title: "3 tips for more maintainable unit tests"
subtitle: "Avoid having to fix dozens of tests every time you make a significant code change"
description: "Highlights some tips for making test more resilient by focusing on behavior rather than structure. By testing from the outside we are free to change implementation details."
permalink: maintainable-unit-tests
author: "Devon Burriss"
category: Programming
tags: [Clean Code, TDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/vents-bg.jpg"
social-img: "img/posts/2018/bridge-cables-500.jpg"
published: false
---
Although having a good collection of unit tests makes you feel safe and free to refactor, a bad collection of tests can make you scared to refactor. How so? A single change to application code can cause a cascade of failing tests. Here are some tips for avoiding (or fighting back) from that situation.
<!--more-->

> Important! This post contains example code. Don't copy/paste into production code.

## Tip 1: Test behavior not structure

The behavior of the system is what the business cares about and it is what you should care about as well from a verification point of view. If requirements change drastically then changes to the system are expected, including the tests. The promise of good unit test coverage is that you can refactor with confidence that your tests will catch any regressions in behavior. However if you are testing the structure of your application rather than the behavior, refactoring will be difficult since you want to change the structure of your code but your tests are asserting that structure! Worse, your test suite might not even test the behavior but you have confidence in them because of the sheer volume.

If you test the behavior of the system from the outside you are free to change implementation and your tests remain valid. I am not necessarily talking about integration style tests but actual unit tests whose entry point is a natural boundary. At work we have use-case classes that form this natural entry-point into any functionality.

So let's look at an example of structural testing, and see the what happens when we try make a change to the implementation details.

```csharp
// tests
// test for invalid name omitted...

[Fact]
public void CreatingPerson_WithValidPerson_CallsIsValid()
{
    var name = "Bob";
    var people = Substitute.For<IPersonRepository>();
    var validator = Substitute.For<IPersonValidator>();
    var createPerson = new CreatePerson(people, validator);

    createPerson.With(name);

    validator.ReceivedWithAnyArgs(1).IsValid(Arg.Any<Person>());
}

// anemic domain entity
public class Person
{
    public Person(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
}

// use-case
public class CreatePerson
{
    private readonly IPersonRepository personRepository;
    private readonly IPersonValidator personValidator;

    public CreatePerson(IPersonRepository personRepository, IPersonValidator personValidator)
    {
        this.personRepository = personRepository;
        this.personValidator = personValidator;
    }

    public void With(string name)
    {
        var person = new Person(Guid.NewGuid(), name);
        if (personValidator.IsValid(person))
        {
            personRepository.Create(person);
        }
        else
        {
            throw new ArgumentException(nameof(name));
        }
    }
}
```

Notice how we are asserting against a dependency (`IValidator`) of the use-case (`CreatePerson`). Our test has structural knowledge of how `CreatePerson` is implemented. Let's see what happens when we want to refactor this code...

Your team has been trying to bring in some new practices like Domain-Driven Design. The team discussed it and the `Person` class represents an easy start to get used to the idea. You have been tasked with pulling behavior into the the `Person` entity and make it less anemic.

As a first try you move the validation logic into the `Person` class.

```csharp
public class Person
{
    public Person(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public bool IsValid()
    {
        if (Id == Guid.Empty) return false;
        if (string.IsNullOrEmpty(Name)) return false;
        return true;
    }

    public Guid Id { get; }
    public string Name { get; }
}
```

Looking at the use-case, we no longer need to inject `IValidator`. Not only is what we test going to have to change, we are going to have to change the test completely because we no longer have a validator to inject as a mock. We have seen the first signs of our tests being fragile.

Let's try make our test focus on the behavior we expect instead of relying on the structure of our code.

```csharp
// test for invalid name omitted...
[Fact]
public void CreatePerson_WithValidName_PersistsPerson()
{
    var name = "Bob";
    InMemoryPersonRepository people = Given.People;
    var createPerson = new CreatePerson(people);

    createPerson.With(name);

    Assert.Equal(name, people.All().First().Name);
}
```

Don't worry too much about `InMemoryPersonRepository people = Given.People;` for now, we will come back to it. All you need to know is that `InMemoryPersonRepository` implements `IPersonRepository`.

Since we no longer need `IValidator` and it's implementation, we delete those. We also get to delete the test `CreatingPerson_WithValidPerson_CallsIsValid` as we have a better test now `CreatePerson_WithValidName_PersistsPerson` that asserts the behavior we care about, the use-case creating and persisting a new person. Yay, less test code, better coverage!

At this point you might be saying "Wait! Unit tests are supposed to test one method, on one class". No! A unit is whatever you need it to be. I am by no means saying write no tests for your small implementation details, just make sure you are comfortable deleting them if things change. With our focus on behavior tests we can delete those detailed tests freely and still be covered. In-fact, I often just delete the tests after I am done developing the component as I just used TDD for fast feedback loop on the design and implementation. Remember that test code is still code that needs maintenance so the more coverage for less the better.

So back to the code. What does our use-case look like now?

```csharp
public class CreatePerson
{
    private readonly IPersonRepository personRepository;
    public CreatePerson(IPersonRepository personRepository)
    {
        this.personRepository = personRepository;
    }

    public void With(string name)
    {
        var person = new Person(Guid.NewGuid(), name);
        if (person.IsValid())
        {
            personRepository.Create(person);
        }
        else
        {
            throw new ArgumentException(nameof(name));
        }
    }
}
```

Thats ok. We got rid of a dependency and moved some logic to our `Person` entity but we can do better. On reviewing your pull request someone in the team pointed out something important. You should be aiming to make unrepresentable states unrepresentable. The business doesn't allow saving a person without a name so let's make it so that we can't create an invalid `Person`.

```csharp
// person entity
public class Person
{
    public Person(Guid id, string name)
    {
        if (id == Guid.Empty) throw new ArgumentException(nameof(id));
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));

        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
// use-case
public class CreatePerson
{
    private readonly IPersonRepository personRepository;
    public CreatePerson(IPersonRepository personRepository)
    {
        this.personRepository = personRepository;
    }

    public void With(string name)
    {
        var person = new Person(Guid.NewGuid(), name);
        personRepository.Create(person);
    }
}
```

Look at that! We refactored the implementation without having to update our test. It still passes without any changes.

This was a contrived example to illustrate the point but I hope this tip helps you write more maintainable tests.

## Tips 2: Use in-memory dependencies

You have already seen `InMemoryPersonRepository` so this tip should be less verbose to explain. The claim is simply that the maintainability of your tests can be increased by using in-memory versions of your dependencies a little more and using mocking frameworks a little less.

I find in-memory versions of something like a repository that speaks to a database preferable to mocking frameworks for a few reasons:

1. They tend to be easier to update than a mocking framework, especially if creation of the mocks is done in every test or fixture
1. Coupled with some tooling (see next tip) they lead to far easier setup and readability
1. They are simple to understand
1. Great debugging tool

On the down side, they do take a little time to create.

Let's take a quick look at what the one looks like for our code so far:

```csharp
public class InMemoryPersonRepository : IPersonRepository
{
    private IDictionary<Guid, Person> data;

    public InMemoryPersonRepository(IDictionary<Guid, Person> data)
    {
        this.data = data;
    }

    public IReadOnlyCollection<Person> All()
    {
        return new List<Person>(data.Values);
    }

    public void Create(Person person)
    {
        data.Add(person.Id, person);
    }
}
```

Super simple! Put in the work and give it a try, it may not be as sexy as a mocking framework but it really will help make your test suite more manageable.

## Tip 3: Build up test tooling

Test tooling in this context means utility classes to make readability and maintainability of the tests easier. A big part of this is about making your tests clear about the setup while still keeping it concise.

Let's discuss a few helpers you should have in any project...

### In-memory dependencies

This was already discussed above. I can't stress enough how much this improves maintenance and simplifies reasoning about tests.

### Builders

Builders can be used as an easy way to setup test data. They are a great way of simultaneously avoiding dozens of different setup methods for your tests and a way to make it clear what the actual setup of your test is without diving into some setup method that looks like all the other setup methods.

```csharp
public class InMemoryPersonRepositoryBuilder
{
    IDictionary<Guid, Person> data = new Dictionary<Guid, Person>();
    
    public InMemoryPersonRepositoryBuilder With(params PersonBuilder[] people)
    {
        foreach (Person p in people)
        {
            data.Add(p.Id, p);
        }
        return this;
    }

    public InMemoryPersonRepository Build()
    {
        return new InMemoryPersonRepository(data);
    }

    public static implicit operator InMemoryPersonRepository(InMemoryPersonRepositoryBuilder builder)
        => builder.Build();
}
```

A little trick is to put an `implicit` conversion to the class you are building up. Also take a look at [Fluency](https://github.com/nrjohnstone/Fluency) for helping with the creation of builders.

A final note on this point. Just because I use builders a lot does not mean I completely throw mocking frameworks out the window. I just tend to use them for things I really don't care about and really aren't likely to change. I also tend to use them within other builders rather than directly in tests.

### Accessors

Not sure what else to call these but it is useful to have a static class that makes access to builders and other types you would use in setup simple. Typically I have `Given` and `A`.

```csharp
/// <summary>
/// Handles creation of instances useful to testing like entites, value objects, settings, etc.
/// </summary>
public static class A
{
    public static PersonBuilder Person => new PersonBuilder();
}

/// <summary>
/// Handles the creation of builders that build external services for testing
/// </summary>
public static class Given
{
    public static InMemoryPersonRepositoryBuilder People => new InMemoryPersonRepositoryBuilder();
}
```

This allows me to write some very concise setup code. For example if I needed to populate my person repository with 3 random people I could do so like this:

```csharp
InMemoryPersonRepository people = Given.People.With(A.Person, A.Person, A.Person);
// if i wanted another with a specific name
people.Create(A.Person.With(name: "Bob"));
```

For completeness the `PersonBuilder` implementation:

```csharp
public class PersonBuilder
{
    private Guid id;
    private string name;

    public PersonBuilder()
    {
        id = Guid.NewGuid();
        name = $"name {Guid.NewGuid()}";
    }

    public PersonBuilder With(Guid id)
    {
        this.id = id;
        return this;
    }

    public PersonBuilder With(string name)
    {
        this.name = name;
        return this;
    }

    public Person Build()
    {
        return new Person(id, name);
    }

    public static implicit operator Person(PersonBuilder builder) => builder.Build();
}
```

## Wrapping up

So those are my 3 tips for making your tests more maintainable. I encourage you to give them a try. Without investing in the maintainability of your tests they can quickly become a burden rather than a boon. I have seen the practices above improve things not only in my teams but other colleagues have converged on similar learnings with the same positive results. Let me know if you find this helpful, or even if there are any points you strongly disagree with. I would love to discuss in the comments. Happy coding!