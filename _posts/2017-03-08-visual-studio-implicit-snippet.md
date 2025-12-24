---
layout: post
title: "Visual Studio Implicit Snippet"
subtitle: "Quickly create implicit class for a simple type"
author: "Devon Burriss"
category: Tools
tags: [Visual Studio]
comments: true
permalink: visual-studio-implicit-snippet
excerpt_separator: <!--more-->
published: true
topics: [tooling-automation]
keywords: [Tools, Visual Studio]
---
Sometimes you want to create a [descriptive type](/honest-arguments) to better represent a concept such as an email (rather than a `string`) but what stops you is the effort in creating this type. Here is a quick snippet to allow you to quickly generate these types reliably.

<!--more-->

# What will we be generating?

What we are trying to generate is a class that ends up looking something like this.

```csharp
public class LastName
{
    string Value { get; }
    public LastName(string value) { Value = value; }

    public static implicit operator string(LastName c)
        => c.Value;
    public static implicit operator LastName(string s)
        => new LastName(s);

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object obj)
    {
        if (Value == null || obj == null)
            return false;

        if (obj.GetType() == typeof(string))
        {
            var otherString = obj as string;
            return string.Equals(Value, otherString, StringComparison.Ordinal);
        }

        if (obj.GetType() == this.GetType())
        {
            string otherString = string.Format("{0}", obj);
            return string.Equals(Value, otherString, StringComparison.Ordinal);
        }

        return false;
    }
}
```

This class will implicitly convert between `LastName` and `string` and compares like a value type. So two different instances of the same last name will be equivalent.

## Visual Studio Snippet

If you are using [Resharper](https://www.jetbrains.com/resharper/features/code_templates.html) or another development productivity extension, creating snippets is fairly easy. In Visual Studio without a productivity extension it takes a little more effort but not much.  

First you will need to create the snippet. Open up your favourite editor ([I use Visual Studio Code](https://code.visualstudio.com/)) and create a file called *impl.snippet* and save it somewhere. You will be importing it into Visual Studio later so remember where you put it. Also be aware that it will actually be copied to *C:\Users\{user}\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets* when you import it and the one you saved is not the one that Visual Studio uses. So if make changes to the original you will need to re-import it and if you edit the imported one it seems VS needs a restart.

```xml
<?xml version="1.0" encoding="utf-8"?>  
<CodeSnippets  
    xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">  
    <CodeSnippet Format="1.0.0">  
        <Header>  
            <Title>Class with implicit string operator</Title>  
            <Author>Devon Burriss</Author>  
            <Description>Creates a class that can implicitly convert to and from string.</Description>
            <Shortcut>impl</Shortcut>
        </Header>
        <Imports>  
            <Import>  
                <Namespace>System</Namespace>  
            </Import>  
        </Imports>  
        <Snippet>
            <Declarations>  
                <Literal>  
                    <ID>name</ID>   
                    <ToolTip>Name of the class.</ToolTip>   
                    <Default>MyImplicitType</Default>   
                </Literal>
            </Declarations>
            <Code Language="csharp">  
                <![CDATA[
                    public class $name$
                    {
                        string Value { get; }
                        public $name$(string value) { Value = value; }

                        public static implicit operator string($name$ c)
                            => c.Value;
                        public static implicit operator $name$(string s)
                            => new $name$(s);

                        public override string ToString() => Value;
                        public override int GetHashCode() => Value.GetHashCode();
                        public override bool Equals(object obj)
                        {
                            if (Value == null || obj == null)
                                return false;

                            if (obj.GetType() == typeof(string))
                            {
                                var otherString = obj as string;
                                return string.Equals(Value, otherString, StringComparison.Ordinal);
                            }

                            if (obj.GetType() == this.GetType())
                            {
                                string otherString = string.Format("{0}", obj);
                                return string.Equals(Value, otherString, StringComparison.Ordinal);
                            }

                            return false;
                        }
                    }
                ]]>  
            </Code>  
        </Snippet>  
    </CodeSnippet>  
</CodeSnippets>  
```
Xml file: *impl.snippet*

The `<Header>` element defines some generic information about the snippet. It is all self explanatory. I do want to just point out the `<Shortcut>` element. This is what you will edit if you want anything other than typing **impl** and then hit the **Tab** button to activate the snippet.  

The interesting bit is the `<Literal>` element. It has an `<ID>` element which is used in the snippet template to be the replacement variable. So when you hit **Tab** you can type a name for the class and it will be inserted into all the relevant places.

## Import into Visual Studio

Once you have created your snippet and saved it somewhere, go to Visual Studio (if that isn't what you used to create the snippet). 

- Navigate to *Tool > Code Snippets Manager...* (or press Ctrl+K, Ctrl+B).
- Click *Import...* (you can choose C# language to be safe but it seems to pick it up from the snippet)
- Browse to the *impl.snippet* file you created earlier an click *Open*
- Make sure **My Coded Snippets** is selected and click *Finish*

And you are done. Now to create the class you can type `impl` in any .cs file and hit **Tab** and it will generate the class

## Conclusion

If you find yourself creating repetitive classes, or avoiding to create classes because they are repetitive. Consider automating it to a degree by using a snippet.

## Further reading

- [How to](https://msdn.microsoft.com/en-us/library/ms165396.aspx)
- [Snippet functions](https://msdn.microsoft.com/en-us/library/ms242312.aspx)
- [Schema Reference](https://msdn.microsoft.com/en-us/library/ms171418.aspx)
