---
layout: post
title: "How to F# - Part 9"
subtitle: "Working with databases in F#"
description: "Explore connecting and working with a database in F# using Dapper and SQLite"
permalink: how-to-fsharp-pt-9
author: "Devon Burriss"
comments: true
excerpt_separator: <!--more-->
header-img: "img/backgrounds/silhouette-bg.jpg"
social-img: "img/posts/2018/package-500.jpg"
published: true
topics: [platforms-runtime]
keywords: [Software Development, Functional, F#, .NET]
---

---
In almost any software system we want to store data at some point. For decades the bread and butter of persisting data has been databases, and in this post we look at ways of working with a database in F#.
<!--more-->

## Introduction

SQL databases are very common and have been for decades. In this post we will look at how to interact with a [SQLite database](https://www.sqlite.org/about.html) using the [Dapper](https://github.com/StackExchange/Dapper) library. Lets briefly go through the technologies we will be touching today that are not F#. If you already have experience with relational databases and are just here for the F#, you probably want to skip this introduction section.

### Structured Query Language

Structured Query Language (SQL) is a domain specific language. What is a domain specific language? Well it is a language that is designed and used in a very specific domain. In this case, working with databases. I am not going to go into the mathematics as I did with functions because frankly the syntax is not nearly as similar, so it doesn't demonstrate much. Suffice to say it has its roots in relational algebra. It is a language that is easy to start using but hard to master.

Imagine we want a table of data like this:

<div class="table-responsive"><table class="table table-hover">
<thead> <tr> 
    <th>id</th> <th>name</th>  <th>email</th> </tr> </thead>
<tbody>
<tr> <td>1</td> <td>Sue</td>   <td>sue@acme.com</td>        </tr>
<tr> <td>2</td> <td>Bob</td>   <td>khan@acme.com</td>       </tr>
<tr> <td>3</td> <td>Neo</td>   <td>neo@metacortex.com</td>  </tr>
<tr> <td>4</td> <td>Fen</td>   <td>fen@acme.com</td>        </tr>
<tr> <td>5</td> <td>An Si</td> <td>we@acme.com</td>         </tr>
<tr> <td>6</td> <td>Jan</td>   <td>lee@acme.com</td>        </tr>
</tbody>
</table></div>

#### Creating a table

So how would we create the structure for the table above? SQL is one of the more descriptive languages, because it is so specific. This is a good thing.

```sql
CREATE TABLE people (
                     id INTEGER PRIMARY KEY,
                     name TEXT NOT NULL,
                     email TEXT NOT NULL UNIQUE
                    );
```

Firstly we specify the name of the table to create, *people*. Secondly, we specify the columns found in the table.  
We have an `id` that is an integer. We mark it as `PRIMARY KEY` to indicate that it is the primary way to uniquely identify our record. The database will automatically insert an incrementing identifier for each record we insert.  
Next we have `name` which is a `TEXT` field indicating we can store a `string` value. `NOT NULL` indicates we cannot leave this record out.
Finally, we have `email` which is similar to `name` except we have an extra constraint on it that it be `UNIQUE`. The database will enforce these constraints of `NOT NULL` and `UNIQUE`, giving us some measure of protection from bad data.

#### Inserting a record

So we have our table but how do we get data in the database? Unsurprisingly we use `INSERT`.

```sql
INSERT INTO people (name,email) VALUES ("Bob","bob@acme.com");
```

We specify the table *people* as the one we want to insert into and then the columns we will be supplying data for. Then we indicate the values to insert using `VALUES` where the order of the values matches the order of the columns we specified.

#### Updating a record

What if some data changed since being inserted? Well of course SQL provides an `UPDATE` command.

```sql
UPDATE people SET name='Bobby', email= 'bobby@acme.com' WHERE id = 1;
```

So we indicate an `UPDATE` on a specific table and then `SET` whichever columns we want to change. You almost always want to specify a condition of which record to update. If you left off the `WHERE` for this update it could set every `name` and `email` to "Bobby" and "bobby@acme.com", except that we are protected by our `UNIQUE` constraint on `email`, so our constrain saves us from a potentially devastating loss of data.

#### Fetching records

How would we query data from it? We use a SQL `SELECT` statement.

```sql
SELECT id,name,email FROM people;
```

When selecting we start with `SELECT` then specify the columns we want, then `FROM` which table.

When selecting data we can also use `WHERE` to specify specific records.

```sql
SELECT id,name,email FROM people WHERE id = 1;
SELECT id,name,email FROM people WHERE email LIKE '%@acme.com';
```

The first query will return a single record since `id` is always unique.  
The second query will return all records where `email` ends with *@acme.com*, skipping only record number 3 in our example data.

> In this tutorial we will only deal with data in a single table, we will not be going into relationships between tables. Relationships are a very powerful aspect of some databases and worth looking into further.

### SQLite

SQLite is a very popular database that has some unique characteristics that make it desireable for a tutorial like this. It requires no server so we interact directly with the file system from our process. This means it is very easy to get going with as it has zero setup.

We will be using the [System.Data.SQLite Nuget package](https://www.nuget.org/packages/System.Data.SQLite/) to interact with a local [SQLite](https://www.sqlite.org) database. The database is created in our code when we use it for the first time.

### Dapper

[Dapper](https://github.com/StackExchange/Dapper) is a very popular mini-ORM. An ORM (Object Relational Mapper) is typically a library used in your code that maps relational data from a database to objects in your programming language of choice. While a full ORM will typically generate all queries, joins, and mappings for you, a mini-ORM will usually require you to still write some SQL and then it will do some mapping by convention for you. We will be visiting some Dapper code soon.

## Now to the good part

Although Dapper is a great library for flexibly working with databases, it is written in and for C#. So the first thing we are going to do each time we use Dapper is wrap its functionality it in functions that surfaces Dapper in a more functional way.

### Executing SQL

Dapper exposes the following C# function that we will be using a lot. It executes a SQL statement against a database connection and allows you to optionally pass an `object` in for parameters for the SQL statement. Don't worry if this doesn't make complete sense now, it should make more sense when you see an example.

It is known as an **extension method** and is on an instance of `IDbConnection`.

```csharp
public static Task<int> Execute(this IDbConnection cnn, string sql, object param = null, SqlTransaction transaction = null)
```

So what is the problem here? Well for one remember in [part 8](/how-to-fsharp-pt-8) we looked at how to handle exceptions more functionally? The above method will throw an exception if something goes wrong. Lets fix that.

```fsharp
open Dapper
open System.Data.Common

// DbConnection -> string -> 'b -> Result<int,exn>
let execute (connection:#DbConnection) (sql:string) (parameters:_) =
    try
        let result = connection.Execute(sql, parameters)
        Ok result
    with
    | ex -> Error ex
```

> NOTE: I am catching ALL errors here, contrary to my advice in the previous [post on error handling](/how-to-fsharp-pt-8). This is to keep things simple and concentrate on executing SQL.

So we have a function called `execute` now with signature `DbConnection -> string -> 'b -> Result<int,exn>`. It makes use of the Dapper extension method `Execute` but we wrap it in a `try..with` expression and return a type `Result<int,exn>`.

To use `execute` we need an instance of a `DbConnection`. Lets write a small function that will return us a database connection and open that connection to the database, ready to use.

```fsharp
// string -> SQLiteConnection
let conn (db:string) =
    let c = new SQLiteConnection(sprintf "Data Source=%s.sqlite" db)
    c.Open()
    c
```

#### Creation

So we now have all the building blocks to execute a SQL statement. Lets create a *people* table in a database called *test*.

```fsharp
// DbConnection -> Result<int,exn>
let createPeopleTable (connection:DbConnection) =
    let sql = "CREATE TABLE IF NOT EXISTS people (
                 id INTEGER PRIMARY KEY,
                 name TEXT NOT NULL,
                 email text NOT NULL UNIQUE
                );"
    execute connection sql None

// create a connection and the table
let dbName = "test"
let connection = conn dbName
createPeopleTable connection
```

#### Insertion

So now we have a table called *people*. Lets insert a record.

```fsharp
// DbConnection -> string -> string -> Result<int,exn>
let insertPerson (connection:DbConnection) name email  =
    let data = [("@name",box name);("@email",box email)] |> dict |> fun d -> new Dapper.DynamicParameters(d)
    let sql = "INSERT INTO people (name,email) VALUES (@name,@email);"
    execute connection sql data

// insert a person from name and email
insertPerson connection "Sue" "sue@acme.com"
```

So in the above code we make use of a type called `DynamicParameters` from Dapper. This takes in a dictionary so we create a list of name value tuples, and convert that to a dictionary before passing it to `DynamicParameters`. Worth noting here is that the constructor of `DynamicParameters` takes `IDictionary<string,obj>`.

Which brings us to `box`. It has a signature of `'T -> obj`, so when applied to the values in the tuples we get type `IDictionary<string,obj>` as needed for the constructor of `DynamicParameters`.

> This fails with some pretty cryptic errors of *Insufficient parameters supplied to the command* if you do not call the `box` function on the value.

Another way of achieving the same, and usually a better option, is to use an actual type to represent the insert data.

```fsharp
type CreatePerson = { name:string; email:string }
let insertPerson (connection:DbConnection) (person:CreatePerson)  =
    let sql = "INSERT INTO people (name,email) VALUES (@name,@email);"
    execute connection sql person

insertPerson connection { name = "Ali"; email = "ali@acme.com" }
```

#### Update

We could of course have both variations with the update as well.

```fsharp
// Option 1: multiple arguments
let updatePerson (connection:DbConnection) id name email =
    let data = [("@id",box id);("@name",box name);("@email",box email)] 
                |> dict |> fun d -> new Dapper.DynamicParameters(d)
    let sql = "UPDATE people SET name=@name, email=@email WHERE id=@id"
    execute connection sql data

// Option 2: a record with all data
[<CLIMutable>]
type UpdatePerson = { id:int; name:string; email:string }
let updatePerson (connection:DbConnection) (person:UpdatePerson) =
    let sql = "UPDATE people SET name=@name, email=@email WHERE id=@id"
    execute connection sql person

// use option 2
let updatedPerson = { id=2; name="Kublai Khan"; email="kublai.k@acme.com"}
updatePerson connection updatedPerson
```

> NOTE: We put the `[<CliMutable>]` attribute on the type because later on we use this type to return rows from the database. If left off you will receive an error: *A parameterless default constructor or one matching signature (System.Int64 id, System.String name, System.String email) is required for UpdatePerson materialization*

As you can see, option 2 will handle change a lot better than option 1 if more fields need to be added it a person.

## Querying for data

So far we have looked at SQL that changes state but doesn't really return much, other than the number of changes. Lets now look at querying for data.

First we need to write our functional wrappers around Dapper. We will create a function for querying for multiple records (`query`) and another for querying a single record (`querySingle`). The make use of Dapper's `Query` and `QuerySingleOrDefault` methods respectively.

```fsharp
// DbConnection -> string -> IDictionary<string,obj> -> Result<seq<'T>,exn>
let query (connection:#DbConnection) (sql:string) (parameters:IDictionary<string, obj> option) : Result<seq<'T>,exn> =
    try
        let result =
            match parameters with
            | Some p -> connection.Query<'T>(sql, p)
            | None -> connection.Query<'T>(sql)
        Ok result
    with
    | ex -> Error ex

// DbConnection -> string -> IDictionary<string,obj> -> Result<'T,exn>
let querySingle (connection:#DbConnection) (sql:string) (parameters:IDictionary<string, obj> option) =
    try
        let result =
            match parameters with
            | Some p -> connection.QuerySingleOrDefault<'T>(sql, p)
            | None -> connection.QuerySingleOrDefault<'T>(sql)
        
        if isNull (box result) then Ok None
        else Ok (Some result)

    with
    | ex -> Error ex
```

Note that for `query` I specify the return type, this is purely so the return type uses `seq<'T>` instead of `IEnumerable<'T>`. Errors are returned as before and for `querySingle` any `null` is returned as an `option` type as we discussed in [part 6](/how-to-fsharp-pt-6).

So lets use `query` to create a search function for all ACME employees.

```fsharp
let findAcmeEmployees (connection:DbConnection) =
    let sql = "SELECT id,name,email FROM people WHERE email LIKE '%@acme.com'"
    query connection sql None

match (findAcmeEmployees connection) with 
| Ok people -> printfn "Found %i employees" (Seq.length people)
| Error ex -> printfn "%A" ex.Message
```

Lastly, we will demonstrate fetching a single record by **id**.

```fsharp
let personById (connection:DbConnection) id =
    let data = [("@id",box id)] |> dict |> Some
    let sql = "SELECT id,name,email FROM people WHERE id = @id"
    querySingle connection sql data

// use the function to fetch person with id 1 and print results out
match (personById connection 1) with
| Ok (Some(person)) -> printfn "Found %i : %s %s" person.id person.name person.email
| Ok None -> printfn "No person found"
| Error ex -> printfn "%A" ex.Message
```

See how we handle different possibilities when evaluating a query result. We have the happy case where we have no errors and find someone. We have no errors but do not find someone. And finally we handle errors.

## Cleaning up

Remeber the `conn` method we created at the beginning of the code walkthrough? It gave us back an open connection because it called `Open()` on the connection before returning it. If you have performed the operation on the connection, but may use it again, call `Close()` on the connection. If you are done with the operation, call `Dispose()`. Once disposed you cannot use the connection again and will need to create another if needed.

```fsharp
let cleanup (connection:DbConnection) =
    connection.Close()
    connection.Dispose()
```

Technically, you could just call `Dispose()` if you are not planning on reusing the connection.

## Conclusion

We covered quite a lot today but now you know the basics of working with a database in F#. We saw how we can use Dapper to ease passing in parameters and mapping to types. We wrote a functional wrapper around Dapper to handle errors and `null`s. And we saw how to persist to and query from a database that we created.

What we covered here is a pretty standard way to work with a database. F# actually has some very novel ways of working with databases using [Type Provider](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/)s like [SQLProvider](https://github.com/fsprojects/SQLProvider) and [Rezoom.SQL](https://github.com/rspeele/Rezoom.SQL).

In the final **How to F#** coming soon we will put everything we have learned together to create you first F# application.

## Resources

1. [Install SQLite binaries](http://www.sqlitetutorial.net/download-install-sqlite/)
1. [CREATE TABLE](http://www.sqlitetutorial.net/sqlite-create-table/)
1. [INSERT](http://www.sqlitetutorial.net/sqlite-insert/)
1. [UPDATE](http://www.sqlitetutorial.net/sqlite-update/)
1. [Boxing for fun and profit](https://fsharpforfunandprofit.com/posts/cli-types/#boxing-and-unboxing)


