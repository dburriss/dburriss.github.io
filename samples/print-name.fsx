#load "Strings.fsx"
open Strings
let stringWithSpace x = x |> string |> (sprintf " %s")
let name first = first |> toUpper
let nameAndLastName first last = first |> StringBuilder.initWith |> StringBuilder.append last |> stringWithSpace |> toUpper
let nameAndLastNameWithOccupation first last occ = 
    first |> StringBuilder.initWith 
    |> StringBuilder.append " " 
    |> StringBuilder.append last 
    |> StringBuilder.append (sprintf " (%s)" occ)
    |> string |> toUpper

match fsi.CommandLineArgs with
| [|scriptName;|] -> failwith (sprintf "At least a name required for %s" scriptName)
| [|_;firstName|] -> name firstName |> printfn "Name: %s"
| [|_;firstName; lastName|] -> nameAndLastName firstName lastName |> printfn "Name: %s"
| [|_;firstName; lastName; occ|] -> nameAndLastNameWithOccupation firstName lastName occ |> printfn "Name: %s"
| _ -> failwith (sprintf "Too many arguments %A" (fsi.CommandLineArgs |> Array.tail))