---
layout: post
title: "Developer Quest II - Variables"
subtitle: "Trial by reference"
author: "Devon Burriss"
comments: true
permalink: developer-quest-variables
excerpt_separator: <!--more-->
published: true
topics: [notes-reflections]
keywords: [Software Development, Programming, Learning c#, Tutorial]
---

---
> Hold this for me.

## The story so far

Lets go over what we have so far from [Part 1](http://devonburriss.me/developer-quest-getting-started/) and touch on some terminology. We have a **namespace** called DeveloperQuest1. Namespaces are a way of grouping an application or parts of it. Specifically its used in the grouping of the Types that make up an application.
Then we have a **class** called **Program**. **class** is the keyword used to define a Reference Type in C#. We will explore it in more detail later in this tutorial. Then we have the first *member* of Program. *Main* is the **method** that is run when a console application starts. Methods are ways of grouping behaviour in a program that can be executed.

![hero enters town](/img/posts/2014/gfs_36744_2_2.jpg)

<!--more-->

## Variables

Writing things to the screen is great but to make programming useful we need to be able to take input from somewhere, store it, manipulate it and possible then show it or save it.
You can think of variables as the buckets that we store values in while we are using them in the program.
We get 2 main categories of variables. **Value Types** and **Reference Types**. So every variable has a unique **Type** that falls into one of these 2 categories but is always a **Type**.

### Value Types

Value types fall into 2 main sub-categories :

* struct
* enumeration

Structs in turn fall into further categories of

* Numeric
* boolean
* user-defined

I just mention this so you are aware of it when we go through examples. If it doesn't make much sense right now, don't worry about it.
So let's see an example of using a numeric value type

{% highlight csharp %}
int myNumber = 1;
{% endhighlight %}

This assigns the number *1* to the 'bucket' named *myNumber*. The default for an *int* is zero.
There are numerous types of numeric value types that vary in terms of the size of the number they can hold as well as the precision. 
Next are boolean values. The valid options here are either true or false. The default being *false*.

{% highlight csharp %}
bool isHero = true;
{% endhighlight %}

For the full list see here: http://msdn.microsoft.com/en-us/library/bfft1t3c.aspx

Finally a **struct**. Structs are complex values. These can be used to store groups of values together logically. You will see that these seem a lot like reference types but differ in how they are handled in the program.

* In the Solution Explorer **Right-click** on the C# Console Project DeveloperQuest1
* Expand **Add**
* Click **Class...**
* Name the class **Hero**
* Click **Ok**
![new class image](/images/posts/2014/code-change2.jpg)

This will create a new **class** (will discuss later). 

* Change the **class** keyword to a **struct** and add the folowing 2 *fields*.
* Save the changes

It should look like this now:
{% highlight csharp %}
public struct Hero
{
    public int Health;
    public string Name;
}
{% endhighlight %}

> **string** is used to store text. It is a reference type but is handled in a special way.

![structure of application](/img/posts/2014/so-far-1.jpg)

You will see shortly when we explore reference types how similar they look to a **struct**.
The key characteristic to understand about value types is that they always point to their own 'bucket'. 
This can be demonstrated with the following example.
Change your Main *method* to match the code below.
Notice the **using** statement at the top now. This is the *System* namespace and allows us to remove *System* from in front of **Console**. This is because **Console** is a **class** in the *System* namepspace. This makes your code simpler to work with.

{% highlight csharp %}
using System;
namespace DeveloperQuest1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("So you want to be a C# developer?");

            Hero hero1 = new Hero(){
                Health = 10,
                Name = "Bob"
            };

            Hero hero2 = hero1;
            hero2.Name = "Ted";
			Type heroType = hero1.GetType();

            Console.WriteLine("Hero 1 is " + hero1.Name);
            Console.WriteLine("Hero 2 is " + hero2.Name);
            Console.WriteLine("Type is " + heroType.Name);
            Console.WriteLine("Is value type: " + heroType.IsValueType);

            Console.ReadKey();
        }
    }
}
{% endhighlight %}

Run the application by hitting **F5**.

#### Output should be:
			Hero 1 is Bob
			Hero 2 is Ted
			Type is Hero
			Is value type: True
So *hero1* and *hero2* represent 2 unique values. Changing one does not effect the other.

### Reference Types

Reference types, as the name alludes to, can reference the same 'bucket'.
Rather than the *struct* keyword, a reference Type uses *class*. Usually you will create a **class** and the *members* of the **class** are comprised of value and reference types. **Members** can be *fields*, *properties*, or *methods* on a Type. *Name* and *Health* on **Hero** above are examples of *fields*.

Let's change the Hero Type from a *value* type to a *reference* type.

* Open the *Hero.cs* by double-clicking it in the Solution Explorer, or click on the tab if it is still open from when you created it.
* Change **struct** back to **class**
* Save
* Hit **F5** to run the application

#### Output should be:

			Hero 1 is Ted
			Hero 2 is Ted
			Type is Hero
			Is value type: False
So *hero1* and *hero2* both point to the same 'bucket' now. Changing one will change the other. Because *hero2* points at *hero1*, when we changed 2, 1 was also changed because they are the same thing actually. This is the essential difference between a reference type and a value type. Hopefully the names make sense now?

### Using our new found knowledge

We have a reference type that represents our hero. Let's add functionality to the program so we can give our hero a name.
Change the program to match the following.

{% highlight csharp %}
using System;
namespace DeveloperQuest1
{
    class Program
    {
        static void Main(string[] args)
        {
            Hero hero = new Hero();
            hero.Health = 10;
            Console.WriteLine("So you want to be a C# developer?");
            Console.WriteLine("What is your hero's name?");
            hero.Name = Console.ReadLine();

            Console.WriteLine("Your adventure begins " + hero.Name);
			//to pause program
            Console.ReadKey();
        }
    }
}
{% endhighlight %}

So on line 1 we have the *using* statement that imports the *System* namespace to we can use it throughout our code without explicitly referencing it all the time.
Our program is in the *DeveloperQuest1* namespace.
It contains a **Type** called **Program** (which uses the **class** keyword and is such a reference type).
It contains a *method* called **Main** which is run by default by a console application. We will explore the arguments passed in as **args** in a later tutorial.
The 1st statement in the Main method declares a new **Hero** using the **new** keyword. 
We then assign a value of 10 to the hero's **Health** *field*.
We then write to the Console asking for the hero's name and read it into the **Name** *field* on the hero. This is done using a *method* on **Console** called *ReadLine* which reads everything you type in until you hit *Enter*.
We then write out to the console the name we stored on the hero.
Lastly we still have the *ReadKey* call which pauses the application. Above it I show the use of comments. These are ignored by the program but can be used by you to leave instructional text. Use only when something is unclear.
Hit **F5** to run it.

## Summary

In this tutorial we explored the Type categories you get in C# and how to create and use them. In the following tutorial we will dive into *classes* and the various *members* you can have on them.

### Further Reading and References

* http://msdn.microsoft.com/en-us/library/s1ax56ch.aspx
* http://www.albahari.com/valuevsreftypes.aspx

