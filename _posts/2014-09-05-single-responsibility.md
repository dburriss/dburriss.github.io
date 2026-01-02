---
layout: post
title: "Single Responsibility Principle"
subtitle: "The S in SOLID."
author: "Devon Burriss"
comments: true
permalink: single-respon
excerpt_separator: <!--more-->
published: true
topics: [engineering-practices, platforms-runtime]
keywords: [Software Development, Programming, SOLID, OOP]
---

---
> The **S** in **SOLID**.

If I had to pick one principle that had to be enforced strongly on a code base, this would be it. Most techniques for writing elegant code fall by the wayside if this principle is not followed. 

**Layering your application.** Good luck! 

**Inversion of Control.** Constructor injection overload!

**Polymorphism.** I am a concrete implementation of what exactly? 

**Don’t repeat yourself.** Well this does something slightly different… 

It has been a long time but I do remember a time when I was averse to lots of files in a development project. When I had god classes that contained demi-god functions. I am not sure if it is related but it may have been a side effect of programming in a dynamic language but to blame it on a language would be naïve. Besides, I learned the basics of programming in C++ and Java. I also remember a time when every little change I made in my projects broke a chain of other parts, some expected, and way too many completely unexpected. And it was exactly those circumstances that made me question how I was doing things. Enter SRP.
<!--more-->

## Definition

Since it is a principle, rather than a rule; it doesn’t have one clear definition but as far as I can tell Robert C. Martin (http://www.objectmentor.com/omTeam/martin_r.html) coined the term and so his definition will be used:
>THERE SHOULD NEVER BE MORE THAN ONE REASON FOR A CLASS TO CHANGE.

This is a very simple statement but one that is quite hard to get right in practice. It takes discipline to think carefully about where each piece of code is placed to make sure it belongs there.

![trainline into the distance](/img/posts/2014/train-track-800-slim.jpg)

## Class Cohesion 

A discussion of SRP would not be complete without mention of cohesion (http://en.wikipedia.org/wiki/Cohesion_(computer_science)). Cohesion is the measure of how well the members of a class group together. An easy tell to look for when looking for classes with low cohesion is to look for fields that are used in separate functions. If you find a field that is used in some functions, and another field that is used in others, it is likely that you need 2 classes rather than 1 for the behaviour. We will see an example of this later. 

## Example 

Ok. Enough talk (or writing rather…). Lets look at an example of a class that does not follow SRP and refactor it towards one that does. 
The example I use is a service that processes a customer’s order. 

{% highlight csharp %}
public class OrderServiceBefore : IDisposable
{
    private const string connection = @"c:\Example.mdf";
    private readonly DataContext db;
    private SmtpClient emailClient;
    public OrderServiceBefore()
    {
        this.db = new DataContext(connection);
        this.emailClient = new SmtpClient();
    }

    public void Process(Order order)
    {

        //validate order            
        if (order == null)
            throw new ArgumentNullException("order");
        if (order.Customer == null)
            throw new ArgumentException("Customer cannot be null.");
        if (order.OrderLines.Count < 1)
            throw new InvalidOperationException("Cannot process an order with no lineitems.");

        //save order
        db.GetTable<Order>().Attach(order);
        db.SubmitChanges();

        //email order form
        var email = string.Format("New order {0} place on {1} by {2}.");
        foreach (var item in order.OrderLines)
        {
            email = email + "\n";
            email = email + item.Product + " : " + item.Quantity;
        }
        emailClient.Send(new MailMessage("me@me.com", "sales@company.com"));
    }

    public void Dispose()
    {
        if (this.db != null)
            this.db.Dispose();
        if (this.emailClient != null)
            this.emailClient.Dispose();
    }
}
{% endhighlight %}

Looking at the code you can see that the Process method does more than 1 thing. It checks the validity of the order, persists it to the database, and then emails sales with the order details. 
Lets start refactoring this toward a cleaner implementation…

{% highlight csharp %}
public class OrderServiceIntermediate : IDisposable
    {
        private const string connection = @"c:\Example.mdf";
        private readonly DataContext db;
        private SmtpClient emailClient;

        public OrderServiceIntermediate()
        {
            this.db = new DataContext(connection);
            this.emailClient = new SmtpClient();
        }

        public void Process(Order order)
        {
            OrderProcessGaurd(order);
            SaveOrder(order);
            EmailOrderToSales(order);
        }

        private void EmailOrderToSales(Order order)
        {
            var email = string.Format("New order {0} place on {1} by ");
            foreach (var item in order.OrderLines)
            {
                email = email + "\n";
                email = email + item.Product + " : " + item.Quantity;
            }

            emailClient.Send(new MailMessage("me@me.com", "sales@comp"));
        }

        private void SaveOrder(Order order)
        {
            db.GetTable<Order>().Attach(order); db.SubmitChanges();
        }
        private void OrderProcessGaurd(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            if (order.Customer == null)
                throw new ArgumentException("Customer cannot be null.");
            if (order.OrderLines.Count < 1)
                throw new InvalidOperationException("Cannot process an order with no lineitems.");
        }
        public void Dispose()
        {
            if (this.db != null)
                this.db.Dispose();

            if (this.emailClient != null)
                this.emailClient.Dispose();
        }

    }
{% endhighlight %}

Here all I did was extract the different activities being performed into methods. This does little else other than make the intent of the Process method clearer, which in turn highlights that this class contains implementation details outside of it’s responsibility. 
So lets extract these methods into classes that are responsible for the needed functionality. We will interface each of these so we can inject the abstraction in rather than the concrete implementation.

{% highlight csharp %}
public class OrderRepository : IOrderRepository
    {
        private const string connection = @"c:\Northwnd.mdf";
        private readonly DataContext db;
        public OrderRepository()
        {
            this.db = new DataContext(connection);
        }
        public void SaveOrder(Order order)
        {
            db.GetTable<Order>().Attach(order);
            db.SubmitChanges();
        }
        public void Dispose()
        {
            if (this.db != null)
                this.db.Dispose();

        }
    }
    public interface IEmailService : IDisposable 
    { 
        void SendOrderToSales(Order order);    
    }

    public class EmailService : IEmailService { 

        private SmtpClient emailClient; 

        public EmailService() { 
            this.emailClient = new SmtpClient(); 
        } 
        public void SendOrderToSales(Order order)        
        {            
            var email = BuildEmailContent(order);                         
            emailClient.Send(new MailMessage("me@me.com", "sales@company.com"));        
        }          
        
        private string BuildEmailContent(Order order)        
        {            
            var email = string.Format("New order {0} place on {1} by {2}." );           
            foreach (var item in order.OrderLines)            
            {                
                email = email + "\n";                
                email = email + item.Product + " : " + item.Quantity;            
            }            
            return email;        
        }          
        public void Dispose() 
        { 
            if (this.emailClient != null)                
                this.emailClient.Dispose(); 
        } 
    }
{% endhighlight %}

With these new classes extracted we can now make use of them in our OrderService class.

{% highlight csharp %}
public class OrderServiceAfter : IDisposable
    {
        private readonly IOrderRepository orderRepository;
        private readonly IEmailService emailService;
        public OrderServiceAfter(IOrderRepository orderRepository, IEmailService emailService)
        {
            this.orderRepository = orderRepository;
            this.emailService = emailService;
        }
        public void Process(Order order)
        {
            OrderProcessGaurd(order);
            orderRepository.SaveOrder(order);
            emailService.SendOrderToSales(order);
        }
        private void OrderProcessGaurd(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            if (order.Customer == null)
                throw new ArgumentException("Customer cannot be null.");
            if (order.OrderLines.Count < 1)
                throw new InvalidOperationException("Cannot process an order with no lineitems.");
        }
        public void Dispose()
        {
            if (orderRepository != null)
                orderRepository.Dispose();
            if (emailService != null)
                emailService.Dispose();
        }
    }
{% endhighlight %}

### Analysis

Lets take a quick look at what running code metrics on this in Visual Studio 2013 looks like (Analyze > Calculate Code Metrics for Selected Projects).
![](/img/posts/2014/Code-Metrics-SRP.png)

**Maintainability Index** – Here we see a nice gain just separating out into functions, with a 1 point drop when separating out into classes. I guess Microsoft see it as less maintainable with the logic in different classes. Marginally. The gains on the other criteria more than make up for the 1 point drop though. See: [http://blogs.msdn.com/b/zainnab/archive/2011/05/26/code-metrics-maintainability-index.aspx](http://blogs.msdn.com/b/zainnab/archive/2011/05/26/code-metrics-maintainability-index.aspx) 

[**Cyclomatic Complexity**](http://en.wikipedia.org/wiki/Cyclomatic_complexity) – This basically highlights the paths through the code. It is a good measure of how complex the code is. This dropped so marginally. Typically we can see much better gains here when applying SRP on more complex problems. See: [http://blogs.msdn.com/b/zainnab/archive/2011/05/17/code-metrics-cyclomatic-complexity.aspx](http://blogs.msdn.com/b/zainnab/archive/2011/05/17/code-metrics-cyclomatic-complexity.aspx) 

**Depth of Inheritance** – We are not using inheritance to solve this problem so not going to touch on this. See: [http://blogs.msdn.com/b/zainnab/archive/2011/05/19/code-metrics-depth-of-inheritance-dit.aspx](http://blogs.msdn.com/b/zainnab/archive/2011/05/19/code- metrics-depth-of-inheritance-dit.aspx) 

[**Class Coupling**](http://en.wikipedia.org/wiki/Coupling_(computer_programming)) – We dropped the coupling to other classes quite substantially. This is a very good thing. The less dependencies you class has, the less likely that it breaks due to a change elsewhere in the codebase. See: [http://blogs.msdn.com/b/zainnab/archive/2014/02/22/10168042.aspx](http://blogs.msdn.com/b/zainnab/archive/2014/02/22/10168042.aspx) 

### Resources
[http://www.objectmentor.com/resources/articles/srp.pdf](http://www.objectmentor.com/resources/articles/srp.pdf)


