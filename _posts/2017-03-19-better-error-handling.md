---
layout: post
title: "Better error handling"
subtitle: "Part 3 of Designing clear method signatures"
author: "Devon Burriss"
category: Software Development
tags: [Clean Code, OOP, Functional]
comments: true
permalink: better-error-handling
excerpt_separator: <!--more-->
published: true
series: "Honest Types"
---

In my [previous post](/honest-return-types) I discussed handling `null` and `Exception` in the return type. In this post I will discuss returning logic errors.

# Handling errors

There are times when valid errors can occur but are not exceptional. Validation is a common example of this and where a validation result is often the go to type. Wouldn't it be nice if we could apply the same pattern as with exceptions?

## Either: Errors or no errors

Functional languages define a type with the following form: `Either<Left, Right>`. `Left` and `Right` can be anything but in the case of error handling `Left` is the unhappy path and `Right` is the happy path. Let's assume we have an `Error` type for representing errors that occurred, then using `Either` to represent error handling could look something like this: `Either<IEnumerable<Error>, T>`. `Error` has an implicit conversion to `string` so let's work with `string` for demonstration purposes below.

```csharp
Func<int, int, Either<IEnumerable<string>, int>> divide =
    (i, d) =>
    {
        if (d == 0)
            return List("Cannot divided by zero.");

        return (i / d);
    };

Either<IEnumerable<string>, int> divideByZeroResult = divide(1, 0);
divideByZeroResult.Match(
    Left: errors => errors.ToList().ForEach(x => Console.WriteLine(x)),
    Right: i => Console.WriteLine($"Answer is {i}")
);
//Cannot divide by zero.

Either<IEnumerable<string>, int> twoResult = divide(4, 2);
twoResult.Match(
    Left: errors => errors.ToList().ForEach(x => Console.WriteLine(x)),
    Right: i => Console.WriteLine($"Answer is {i}")
);
//Answer is 2
```

This works great but `Either<IEnumerable<string>, int>` is quite a verbose return type definition. If we know we are always going to use `IEnumerable<string>` as `Left` why not specify that in the type? Before we do that, we are going to take a quick dive into some functional programming ideas.

## Functional side-bar

Lets go through a couple concepts that will come up. Hopefully you read the previous post that introduced *Elevated types*. Here I will quickly go through working with elevated types.

### Return: To the world of elevated types

*Return* is raising to the world of elevated types. You have already seen examples of return already in this post. `Some` and `None` for `Option<T>` and `Left` and `Right` for `Either<L, R>` are just some *return* operations. 

```csharp
//return - elevate an int to Option<int>
Option<int> optInt = Option<int>.Some(1);
// Some(1)
```

### Apply - just this part

*Apply* unpacks a function and applies the first argument then returns an elevated function representing the result.

```csharp
//apply
Func<int, int, int> add = (a, b) => a + b;//function 
Option<int> addOpt = Some(add);//elevate function

var increment = addOpt.Apply(1) ;//apply: b => 1 + b
increment.Apply(5);
// Some(6)
```

### Map: ol' switch-a-roo

*Map* applies the function to the value contained in the elevated value and returns the elevated result. In C# terms *Map* like LINQ's `Select`.

```csharp
Func<int, string> intToString = (i) => i.ToString();
Option<int> optInt = Option<int>.Some(1);
//map - apply function to inner value
Option<string> optString = optInt.Map(intToString);
// Some("1")
```

### Bind: functions in the darkness

> "... and in the darkness bind them"

Sorry that was a Lord of the Rings reference. My 2nd name is legally Aragorn (from birth), I didn't stand a chance...  
*Bind* allows you to compose (bind) functions in an elevated world. It is analogous to `SelectMany` from LINQ fame.

```csharp
Func<string, Option<int>> ifEvenInt = (s) =>
{
    if (int.TryParse(s, out int i))
    {
        return (i % 2 == 0) ? Some(i) : None;
    }
    else
    {
        return None;
    }
};

Func<int, Option<int>> doubleIt = (i) => Some(i * 2);
Func<int, Option<int>> exp = (i) => Some(i * i);

Option<string> optString = optInt.Map("2");

//bind - passes inner value to a function that returns an elevated result
Option<int> eventResult = optString.Bind(ifEvenInt);
// used to combine elevated functions
var worked = eventResult
    .Bind(doubleIt)
    .Bind(exp);
// Some(16)
```

If we changed "2" to "1" the output would be `None` since `ifEvenInt` would return `None` which would short-circuit all the `Bind` calls.

## Match: what goes up must come down

*Match* is the yin to *Return*'s yang. Where *Return* operations elevate values to the elevated world, *Match* drops an elevated value back to the real world.

```csharp
//match
Option<int> optInt = Option<int>.Some(1);
optInt.Match(
    Some: x => Console.WriteLine(x),
    None: () => Console.WriteLine("Nothing")
);
// 1
```

Now that we can get to the elevated world, do what we need to do and then return back through the cupboard, let us get back to the business at hand. Validation!

## Validation: Your result (might have errors)

> You can find the `Validation` type in [HonestTypes.Returns](https://github.com/dburriss/HonestTypes#return-types) package

So let's define a type `Validation<T>` that is `Either<IEnumerable<Error>, T>`? That would remove some of the verbosity of the return type as well as give a clearer semantic to the type name.

```csharp
using static F;

public Validation<Person> Validate(Person person)
{
    if (person == null)
        return Error("Person is null");

    //short circuit on error
    return Valid(person)
        .Bind(ValidateFirstNames)
        .Bind(ValidateLastName)
        .Bind(ValidateEmail);
}

private Validation<Person> ValidateFirstNames(Person person)
{
    if (string.IsNullOrWhiteSpace(person.FirstNames))
        return Invalid(Error($"{nameof(person.FirstNames)} cannot be empty"));

    return person;
}

private Validation<Person> ValidateLastName(Person person)
{
    if (string.IsNullOrWhiteSpace(person.LastName))
        return Invalid(Error($"{nameof(person.LastName)} cannot be empty"));

    return person;
}

private Validation<Person> ValidateEmail(Person person)
{
    if (string.IsNullOrWhiteSpace((string)person.Email))
        return Invalid(Error($"{nameof(person.Email)} cannot be empty"));

    return person;
}

//usage
var validatedPerson = service.Validate(person);

validatedPerson.Match(
    Valid: p => Console.WriteLine($"{p.LastName}, {p.FirstNames} <{p.Email}>"),
    Invalid: err => err.ToList().ForEach(x => Console.WriteLine(x.Message))
);
```

The code above uses `Bind` and short-circuits on the first error. This might not be the desired behaviour. What if we want to check all validations? Here is a version that does that...

```csharp
public Validation<Person> Validate(Person person)
{
    if (person == null)
        return Error("Person is null");

    //collect all errors
    return Valid(Person.Create)
        .Apply(ValidateFirstNames(person.FirstNames))
        .Apply(ValidateLastName(person.LastName))
        .Apply(ValidateEmail(person.Email));
}

Func<FirstNames, Validation<FirstNames>> ValidateFirstNames => firstNames =>
{
    if (string.IsNullOrWhiteSpace(firstNames))
        return Invalid(Error($"{nameof(firstNames)} cannot be empty"));

    return firstNames;
};

Func<LastName, Validation<LastName>> ValidateLastName => lastName =>
{
    if (string.IsNullOrWhiteSpace(lastName))
        return Invalid(Error($"{nameof(lastName)} cannot be empty"));

    return lastName;
};

Func<Email, Validation<Email>> ValidateEmail => email =>
{
    if (string.IsNullOrWhiteSpace((string)email))
        return Invalid(Error($"{nameof(email)} cannot be empty"));

    return email;
};
```

The above code uses `Apply` and is applicative so all errors are returned. Notice how the return result is actually a `Func` that performs the validation.

if you don't like the `Func` style you can continue to use the `Bind` syntax but with the applicative nature using `Validation` types `Join` method...

```csharp
//collect all errors
return Valid(person)
    .Join(ValidateFirstNames(person))
    .Join(ValidateLastName(person))
    .Join(ValidateEmail(person));
```

## Conclusion

And there you have some neat validation logic. If you have any comments or suggestions please leave them below. If you found this useful, please share it with someone who you think might also find it useful.

## Recommended Reading

1. [Elevated world](https://fsharpforfunandprofit.com/posts/elevated-world/)
2. [Railway oriented programming](https://fsharpforfunandprofit.com/rop/)