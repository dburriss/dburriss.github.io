---
layout: post
title: "The Page Module Model with F# and Canopy"
subtitle: "Its like testing with the Page Object Model but way way cooler"
description: "Using F#, Canopy (Selenium) and the Page Object(Module) Model to test UIs in a way that makes tests readable and easy to maintain"
permalink: page-module-model
author: "Devon Burriss"
category: Software Development
tags: [Canopy,Functional,F#,Testing,BDD]
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/hole-bg.jpg"
social-img: "img/posts/2018/frame-500.jpg"
published: true
---
In the past I have done some UI testing with Selenium. I quickly adopted the Page Object Model (POM) for this kind of testing to ease readability, maintenance, and re-use across tests. Recently I needed to look into doing some UI testing and I decided to use [Canopy](https://lefthandedgoat.github.io/canopy/) to abstract away working with Selenium. Although Canopy has some great helpers around Selenium I still found myself wanting to abstract away elements on each page and the pages themselves. Enter the Page Module Model (PMM)...
<!--more-->
So full disclosure... I doubt PMM is a thing. I didn't even try search for it until writing the previous sentence. It isn't. Yet... It is similar to the POM except using `module`s because I am using F#.

## What is the page object model?

The POM is simple. We encapsulate interactions with pages and elements on the site with objects. Here is an example of an [old POM framework I wrote years ago](https://github.com/dburriss/UiMatic).

```csharp
[Url(key: "home")]
public class GoogleHomePage : Page
{
    [Selector(name: "q")]
    public IInput SearchBox { get; set; }

    public GoogleHomePage(IDriver driver) : base(driver)
    {}
}
```

We can then use this class to instantiate an object that we interact with instead of interacting with Selenium directly.

```csharp
[Theory]
[InlineData(TestTarget.Chrome)]
public void Title_OnGoogleHomePageUsingConfig_IsGoogle(TestTarget target)
{
    using (IDriver driver = GetDriver(target, config))
    {
        //create page model for test
        var homePage = Page.Create<GoogleHomePage>(driver);
        //tell browser to navigate to it
        homePage.Go<GoogleHomePage>();
        //fill a value into the text box
        homePage.SearchBox.Value = "TEST";
        //an example of interacting with the config if needed. This gets expected title from config. 
        var expectedTitle = config.GetPageSetting("home").Title;
        //check the titles match
        Assert.Equal(expectedTitle, homePage.Title);
    }
}
```

If you have ever written tests against Selenium directly I am sure you can agree that is cleaner.

## Writing tests in F# and Canopy with Page Module Model

> You can find the [source code for this example on Github](https://github.com/dburriss/PageModuleModelExample)

So what would the Page Object Model look like with static functions on a `module`? Pretty cool actually...

```fsharp
"No laptops are free" &&& fun _ ->
    HomePage.searchFor "Laptops"
    let results = SearchResultsPage.results()
    test <@ results |> List.forall (fun x -> x.Price > 0m) @>
```

We can keep the tests really concise and describe what we want to happen. Here we search for "Laptops", get the search results, and then check that the price is not 0 on any items. We will dive a little deeper into how this is done in the next section.

This style also allows us to easily define simple smoke tests to run before getting into the the more functional tests. A smoke test is an easy quick test of something basic. The idea being that *"Where there is smoke there is fire"*, so if a smoke test fails, it is not worth proceeding with the more feature rich tests.

```fsharp
context "Smoke tests"
skipAllTestsOnFailure <- true
"home page loads" &&& fun _ -> displayed HomePage.homePageBanner
"search box available" &&& fun _ -> displayed Header.searchBox
"cart is available" &&& fun _ -> displayed Header.basketButton
```

We use the `skipAllTestsOnFailure <- true` to make sure we skip any other tests if any smoke tests fails.

## The building blocks for composition

I usually build a page that I need and then start extracting the reusable functions out into modules from there. Most sites will have some kind of header/navigation. Here is what I needed in a header for the tests I wrote for this post.

```fsharp
module Header =
    //selectors
    let searchBox = "#search_query"
    let basketButton = "a[href=\"/winkelmandje\"]"

    //actions
    let searchFor term =
        searchBox << term
        press enter
```

Here we define some selectors and a simple function that allows us to use the search functionality.

If possible to modify the HTML I recommend putting `data-test-xyz` style attributes on your elements to allow you to easily query elements. Unfortunately I did not have the luxury to do so even if the front-end developers would let a back-end developer like me near it. Probably wise :)  

Let's look at something a bit more complex. The following module represents search results on a page.

```fsharp
module SearchResults =
    open OpenQA.Selenium

    type SearchResultElement = {
        ProductId:string
        El:IWebElement
        Name:string
        Price:decimal
        IsAvailable:bool
    }

    let private toPrice (s:string) = s.Split(",").[0] |> decimal
    let private getOrderButton itemEl = itemEl |> elementWithin @".product__order-button"
    let private isOrderButton (orderBtnEl:IWebElement) =
        orderBtnEl
        |> getAttrValue "class"
        |> fun s -> s.Split(" ")
        |> Array.contains @"action--order"

    let items () =
        let rowEls = element (sData "component" "products")
                    |> elementsWithin ".card"
        let getId itemEl = itemEl |> elementWithin "a" |> getDataAttrValue "productid"
        let getTitle itemEl = itemEl |> elementWithin "a" |> getAttrValue "title"
        let getPrice itemEl = itemEl |> elementWithin @".product__sales-price" |> read |> toPrice

        rowEls
        |> List.map (fun itemEl ->
                                {
                                    ProductId = itemEl |> getId
                                    El = itemEl
                                    Name = itemEl |> getTitle
                                    Price = itemEl |> getPrice
                                    IsAvailable = itemEl |> getOrderButton |> isOrderButton
                                })
```

This is a bit complex because of the poor selector options available to me in the HTML but still not too bad. I want to draw attention to the `SearchResultElement` record. I parse the HTML to a record rather than constantly interacting with `IWebElement`. You saw this in the test for a 0 price where I was able to easily check the `Price` field.

Note: I make use of some helpers here like `getDataAttrValue` that are in the `Selectors.fs module` which you can checkout in the source if you like.

## The Page Module

With these building blocks the actual page `module` can end up being quite simple.

```fsharp
module SearchResultsPage =
    open Elements
    open canopy.classic

    let uri = "https://www.coolblue.nl/zoeken" //should use settings or relative urls
    let verifyOn() = on uri
    let searchFor term = Header.searchFor term
    let results() = SearchResults.items()
```

With the page we can now group functionality on a module that makes semantic sense and compose our functions from the building blocks we have already defined.

## Summary

![UI testing with Canopy](/img/posts/2018/ui-testing.jpg)

In this post we saw how the Page Object Model can be modelled in a more functional way, using building blocks to construct pages. We also saw how we can transform interesting elements of the page into records that give us type safety and intellisense.

Lastly, we saw how concise the combination of F# and Canopy can make our UI tests.

## Credits

Social image by [Reinhart Julian](https://unsplash.com/@reinhartjulian)