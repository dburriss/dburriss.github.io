open System.Web.UI.WebControls
#load "Strings.fsx"
open Strings
open System
open System.IO

// let add x y = x + y
// let add1 = add 1

// let trim (s:string) = s.Trim()
// let writeToFile path overwrite content =
//     if(File.Exists(path)) then
//         File.WriteAllText(path, content)
//     else ignore()

// let write sanitizer path content =
//     let sanitized = sanitizer content
//     File.WriteAllText(path, sanitized)

// write trim "/path/to/file.txt" "Some text to write to file"
// write (fun (s:string) -> s.Substring(0, 140)) "/path/to/file.txt" "Some text to write to file"

// let sanitzedWrite = write trim

// let x = id

// let justWrite = write id

// let trimmed1 = trim " some text "
// let trimmed2 = "  some text " |> trim

// let name1 = fst ("Devon",37)
// let name2 = ("Devon",37) |> fst


// Console.ReadLine()
// |> toUpper
// |> trim
// |> justWrite "/to/some/file.txt"


// let inc x = x + 1
// let intToString (x:int) = x |> string

// let incrementedString = inc >> intToString

// 1 |> incrementedString

// let prepareString = toUpper >> trim

// Console.ReadLine()
// |> prepareString
// |> justWrite "/to/some/file.txt"

// File.ReadAllText("/path/to/file.txt")

// let b = true
// if (b) then printfn "Is true" else printfn "Is false"

// if (b) then printfn "Is true"


// //let v = if(b) then 1 // <- Error: This 'if' expression is missing an 'else' branch.
// let v = if(b) then 1 else 0

// let divideBy d n = n/d
// let numerator = 10
// let denominator = 0

// if(denominator <> 0) then 
//     printfn "Dividing by %i, not 0" denominator
//     let x = numerator |> divideBy denominator
//     printfn "The answer is %i" x
//     x
// else
//     printfn "Dividing by 0"
//     let denominator = 1
//     printfn "Instead by %i, not 0" denominator
//     let x = numerator |> divideBy denominator
//     printfn "The answer is %i" x
//     x

// printfn "Denominator is %i" denominator

// let x = 1
// if(x = 1) then printfn "x is 1"
// elif (x = 2) then printfn "x is 2"
// else printfn "x is not 1 or 2"

// match x with
// | 1 -> printfn "x is 1"
// | 2 -> printfn "x is 2"
// | _ -> printfn "x is not 1 or 2"


// let numbers = [1..10]
// for x in numbers do
//     printf "%i " x

// numbers |> List.iter (printf "%i ")

// let ns = [|1..10..100|]
// for i=0 to ((Array.length ns)/2) do
//     printf "%i " (Array.get ns i)

// let random = new System.Random()
// let aNumber() = random.Next(1,10)
// let mutable n = 0
// while (n <> 7) do
//     printf "%i " n
//     n <- aNumber()

// let tripleThreat = (true,99,"str")
// let (b2,n2,s1) = tripleThreat

// let myTuple = (true,99)
// let (b1,n1) = myTuple


// let takeATup1 tup =
//     let x = fst tup
//     let y = snd tup
//     if(x) then printfn "%i" (y + 1) else printfn "%i" (y - 1)
// let takeATup2 (x,y) =
//     if(x) then printfn "%i" (y + 1) else printfn "%i" (y - 1)

// takeATup2 myTuple

// type Id = | RowId of int

// let getRow (RowId rid) =
//     printfn "%i" rid
//     (rid,true)

// let i = RowId 1
// let row = getRow i

// type Person = { Name:string; BirthYear:int }

// let p1 = { Name = "Devon"; BirthYear = 2120 }

// let sayHello { Name = name; BirthYear = _ } =
//     printfn "Hello %s" name

// sayHello p1

// let incDec t n  = 
//     match (t,n) with
//     | (_,1) -> 1
//     | (_,x) when x <= 0 -> 0
//     | (true,x) -> x + 1
//     | (false,x) -> x - 1

// printfn "%i" (incDec true 10)
// printfn "%i" (incDec false 10)
// printfn "%i" (incDec false 1)
// printfn "%i" (incDec false -5)

// // define partial active patterns
// let (|Fizz|_|) i = if ((i%3) = 0) then Some() else None
// let (|Buzz|_|) i = if ((i%5) = 0) then Some() else None
// // use partial active patterns
// let fizzbuzz i = 
//     match i with
//     | Fizz & Buzz -> printf "Fizz Buzz, "
//     | Fizz -> printf "Fizz, "
//     | Buzz -> printf "Buzz, "
//     | x -> printf "%i, " x
// // run fizz buzz for numbers 1 to 20
// [1..36] |> List.iter fizzbuzz
open System
let fname1 = Some "Brandon"
let fname2 = None
//string -> string
let makeEmail name = 
    let sanitizeString s = null |> (fun x -> if (box x = null) then None else Some(x))
    name
    |> Option.bind sanitizeString
    |> Option.orElse (Some "info")
    |> Option.map (fun n -> sprintf "%s@acme.com" n)
    
let lastname = Some "Lee"
let toEmail (s:string) = makeEmail 

let email1 = makeEmail fname1
let email2 = makeEmail fname2
let fullname = Option.map2

let z = Option.bind

// ===============================================
type Person = { Id:int; Name:string }

let people1 = [
    { Id=1; Name="Sue"}
    { Id=2; Name="Bob"}
    { Id=3; Name="Neo"}
    { Id=4; Name="Fen"}
    { Id=5; Name="An"}
    { Id=6; Name="Jan"}
]
let fNames = [ (1, "Sue"); (2, "Bob"); (3, "Neo"); (4, "Fen"); (5, "An Si" ); (6, "Jan")] |> Map.ofList
let lNames = [ (1, "Ali"); (2, "Khan"); (3, "Jacobs"); (4, "Jenson"); (5, "Wu" ); (6, "Lee")] |> Map.ofList
let names = List.zip fNames lNames
// Map<int,string> -> Map<int,string> -> int -> string
let generateName fnames lnames i =
    let random = new System.Random(i)
    let fo = random.Next(1,6) // get a random number between 1 - 6
    let lo = random.Next(1,6) // get a random number between 1 - 6
    sprintf "%s %s" (Map.find fo fnames) (Map.find lo lnames)

// int -> string
let nameGen = generateName fNames lNames
// (int -string) -> int -> Person
let generatePerson gen i = { Id = i; Name = gen(i) }          // return a person
// int -> Person
let personGen = generatePerson nameGen
let people = List.init 10 personGen


let bob = people |> List.find (fun p -> p.Name.StartsWith("Bob"))
let maybeBob = people |> List.tryFind (fun p -> p.Name = "Bob Khan")

let bobs = people |> List.filter (fun p -> p.Name.StartsWith("Bob"))

// char -> string -> string[]
let split (sep:char) (s:string) = s.Split([|sep|])

// string -> string
let leadingLastName (name:string) = 
    
    let lastNameToFront (names:string array) = 
        match names with
        | [||] -> ""
        | [|x|] -> x
        | [|x;y|] -> String.concat ", " ([|y;x|])
        | _ -> [|yield ([Array.last names;","] |> String.concat ""); for i=0 to ((Array.length names)-2) do yield names.[i] |] |> String.concat " "
    
    name |> split ' '
    |>lastNameToFront

let withLeadingLName =  people |> List.map (fun p -> {p with Name = (leadingLastName p.Name)})

[
    { Id = 0; Name = "Wu, Fen" }  
    { Id = 1; Name = "Ali, Bob" }  
    { Id = 2; Name = "Jacobs, Fen" }  
    { Id = 3; Name = "Jenson, Bob" }  
    { Id = 4; Name = "Wu, An Si" }
//...
]


let sorted = withLeadingLName |> List.sortBy (fun p -> p.Name)
[
    { Id = 8; Name = "Ali, An Si" }  
    { Id = 1; Name = "Ali, Bob" }  
    { Id = 6; Name = "Jacobs, An Si" }  
    { Id = 2; Name = "Jacobs, Fen" }  
    { Id = 9; Name = "Jacobs, Neo" }  
    //...
]

// Person -> string
let getLastName person = person.Name |> split ',' |> Array.head
let groupedByLName = withLeadingLName |> List.groupBy getLastName

[
    ("Wu", [{Id = 0; Name = "Wu, Fen";}; {Id = 4; Name = "Wu, An Si";}; {Id = 7; Name = "Wu, Bob";}]);
    ("Ali", [{Id = 1; Name = "Ali, Bob";}; {Id = 8; Name = "Ali, An Si";}]);
    //...
]

let fNames = [ "Sue"; "Bob"; "Neo"; "Fen"; "An Si" ; "Jan"]
let lNames = [ "Ali"; "Khan"; "Jacobs"; "Jenson"; "Wu"; "Lee"]
let names = List.zip fNames lNames |> List.map (fun (fname,lname) -> sprintf "%s, %s" lname fname)