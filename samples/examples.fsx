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

let b = true
if (b) then printfn "Is true" else printfn "Is false"

if (b) then printfn "Is true"


//let v = if(b) then 1 // <- Error: This 'if' expression is missing an 'else' branch.
let v = if(b) then 1 else 0

let divideBy d n = n/d
let numerator = 10
let denominator = 0

if(denominator <> 0) then 
    printfn "Dividing by %i, not 0" denominator
    let x = numerator |> divideBy denominator
    printfn "The answer is %i" x
    x
else
    printfn "Dividing by 0"
    let denominator = 1
    printfn "Instead by %i, not 0" denominator
    let x = numerator |> divideBy denominator
    printfn "The answer is %i" x
    x

printfn "Denominator is %i" denominator

let x = 1
if(x = 1) then printfn "x is 1"
elif (x = 2) then printfn "x is 2"
else printfn "x is not 1 or 2"

match x with
| 1 -> printfn "x is 1"
| 2 -> printfn "x is 2"
| _ -> printfn "x is not 1 or 2"


let numbers = [1..10]
for x in numbers do
    printf "%i " x

numbers |> List.iter (printf "%i ")

let ns = [|1..10..100|]
for i=0 to ((Array.length ns)/2) do
    printf "%i " (Array.get ns i)

let random = new System.Random()
let aNumber() = random.Next(1,10)
let mutable n = 0
while (n <> 7) do
    printf "%i " n
    n <- aNumber()