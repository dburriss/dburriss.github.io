---
layout: post
title: "Testing the Untestable"
subtitle: "Testing after the fact testing hurts"
author: "Devon Burriss"
category: Software Development
tags: [Testing,Programming, Unit Testing , TDD]
comments: true
permalink: testing-the-untestable
excerpt_separator: <!--more-->
published: true
---

If you have ever tried written unit tests for existing code you know it can be quite challenging. Not only is finding what to test difficult, the code usually just wont be testable. If it is code that you have written and you are at liberty to make some sweeping changes, then you can refactor toward testability. If not I still go through a technique at the end of this article for providing testable classes.

![bridge cables](/img/posts/2015/bridge-cables-resize.jpg)

<!--more-->
Let's first try refactor toward testablility.
Our checklist is as follows:

* Create integration tests
* Apply [Single Responsibility Principle](http://devonburriss.me/single-respon/) (SRP)
* Apply [Role Interfaces](http://martinfowler.com/bliki/RoleInterface.html)
* Apply Inversion of Control
* Last stand - [Extract and override](http://amzn.to/1EN0Ymg)

> NOTE: In the rest of this article I talk about abstractions and usually use interface as an example. A base class is often just as valid as an interface (unless it has multiple roles since the languages I use only allow one inheritence parent). The NB part is that the rest of your application is coded against the abstraction and knows nothing about the implementation class.

## Safety net
Your first step should be to create some high level integration tests. This will at least give you some indication that you have broken something when you do.

## What is in a name?
A good measure of whether a class adheres to the SRP principle is the name. If the name doesn't exactly describe what it does, or if it contains words like 'manager', 'provider', 'logic', 'handler', it probably does more than one thing. A name should tell you exactly what a class does, and a class can only have one name...
See the SRP link for an example of splitting a class into it's various responsibilities.

## Role abstraction
A good practice that can be used in conjuction with SRP is adding role interfaces to a class. Hopefully you can refactor to these roles until a class only contains the members in the abstraction but they are a start. Don't be afraid of having classes with a minimal amount of properties and/or methods on it. It means it has a very well defined role.
Even if you do not break a class into multiple classes immediately, if you can refer to them by the role interface you will have far fewer breaks in your code later when you do split it.
### Example
{% highlight csharp %}
public class CustomerManager
{
	public IEnumerable<Customer> GetAll()
	{
		...
	}

	public string GetOrderEmailTemplate()
	{
		...
	}

	public void SendEmail(string template, Customer customer, Order order)
	{
		...
	}
}
{% endhighlight %}

Depending on what you prefer you could split this into 2 or 3 interfaces. Definitely a store for retrieving customers and one for email. Better yet would be a 3rd for testability so you can seperate out retrieving email from sending it.

{% highlight csharp %}
public interface CustomerRepository
{
	IEnumerable<Customer> GetAll();
}

public interface EmailStore
{
	string GetOrderEmailTemplate();
}

public interface EmailService
{
	void SendEmail(string template, Customer customer, Order order);
}
{% endhighlight %}
> NOTE: This is just an example. I would usually try refactor this so that sending the email is completely unaware of the domain model.

Really if you have managed to refactor this far you need just split the classes by abstraction and apply Dependency Injection to invert the dependencies and by then you likely have some easily testable classes.

## Untestable I tell you!

Ok so you have looked at the above but to no avail. You have some dependencies in your class that cannot be injected. A very common reason for this is your class has a dependency on a static class that just cannot be refactored right now to an instance. Another reason is that you just cannot make changes to the public API of the class you are testing. 
> WARNING: Think long and hard before using static classes. The ease of use  they offer upfront comes at the dear dear price of testability.

So the trick to testing a class that seems untestable is [Extract and Override](http://amzn.to/1EN0Ymg). The technique is as follows for the untestable Monster class:
1. Create a class **TestableMonster** that inherits from **Monster**.
2. Now move the class within **Monster** into protected virtual methods.
3. Now you can override any par of **Monster** you need to to test it.
4. In your unit test you will test against **TestableMonster** but you will call the base class for the bits you want to test on it and provide faked procedures for the parts you need to test **Moster** in isolation.

Ok so we have gone over the technique in theory, lets take a look at an example.

### Example

Here is the untestable Monster class.
{% highlight csharp %}
public class Monster
{
	public void ScareAllTheChildren()
	{
		var now = DateTime.UtcNow;
		IEnumerable<Child> children= DataRepository.GetAllChildrenFrom(now);

		foreach (var child in children)
		{
			ScareService.Scare(child);
		}
	}
}
{% endhighlight %}
Although the actual example code is unlikely, the structure is tragically common. In less than 10 lines of code we have 3 static references. We will come back to the testable class, lets start extracting out the parts that make this class hard to test.
{% highlight csharp %}
public class Monster
{
	public void ScareAllTheChildren()
	{
		DateTime now = GetCurrentUtcDateTime();
		IEnumerable<Child> children = GetChildrenWithBedtimeAfter(now);

		foreach (var child in children)
		{
			ScareChild(child);
		}
	}

	protected virtual void ScareChild(Child child)
	{
		ScareService.Scare(child);
	}

	protected virtual IEnumerable<Child> GetChildrenWithBedtimeAfter(DateTime now)
	{
		return DataRepository.GetAllChildrenFrom(now);
	}

	protected virtual DateTime GetCurrentUtcDateTime()
	{
		return DateTime.UtcNow;
	}
}
{% endhighlight %}

As you can see we have made no changes to the external API of the class. The internal changes were done by wrapping the statics in a method call. Not too much there that is likely to break our production code.
So how would we use this?
{% highlight csharp %}
public class TestableMonster : Monster
{
	public DateTime TestDateTime { get; set; }
    public List<Child> ScaredChildren  { get; set; }
    
	protected override DateTime GetCurrentUtcDateTime()
	{
		return TestDateTime;
	}
    
    protected override void ScareChild(Child child)
	{
		ScaredChildren.Add(child);
		base.ScareChild(child);
	}
}
{% endhighlight %}
The above example just shows a way to have a date that is settable in your test. You could of course override the other method to return a known list of children.
The following test is more an integration test than a unit test, as the data is not faked (unless you sent back a fake db from the method) but it demonstrates the usage.
{% highlight csharp %}
[TestMethod]
public void Scare_With2OutOf3ChildrenAsleep_ScareCalledOn2Children()
{
	//Arrange
	var db = InitializeNewDatabase();
	db.Children.Add(new Child { Name = "Sam", LastWentToSleep = DateTime.Parse("2014-01-31 20:00") });
	db.Children.Add(new Child { Name = "Sam", LastWentToSleep = DateTime.Parse("2014-01-31 20:30") });
	db.Children.Add(new Child { Name = "Sam", LastWentToSleep = DateTime.Parse("2014-01-31 21:30") });
    
	var sut = new TestableMonster();
	sut.TestDateTime = DateTime.Parse("2014-01-31 20:45");
    
	//Act
	sut.ScareAllTheChildren();

	//Assert
	Assert.AreEqual(2, sut.ScaredChildren.Count);
}
{% endhighlight %}

## Summary
So we went through some steps you could take to make your classes more testable. If you find you are testing a lot of static code you might want to look at the paid for version of [JustMock](http://www.telerik.com/products/mocking.aspx) or [TypeMock](http://typemock.com/) which are the only to frameworks I know of that allow mocking of statics.

> NOTE: A quick note on DateTime. It is a very sneaky static that leaks into code often. Try make it team policy to not use DateTime and instead use something like this suggested by [Ayenda Rahien](http://ayende.com/blog/3408/dealing-with-time-in-tests)

Happy testing!

