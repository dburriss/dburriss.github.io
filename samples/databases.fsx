#load "Database.fsx"
#r "packages/System.Data.SQLite.Core/lib/net451/System.Data.SQLite.dll"

open Database
open System
open System.Data.SQLite
open System.Data.Common

let unwrapExn (ex:exn) =
    match ex with 
    | :? AggregateException ->
        let x = ex :?> AggregateException
        x.InnerExceptions |> Seq.toList
    | _ -> [ex]

let conn (db:string) = 
    let c = new SQLiteConnection(sprintf "Data Source=%s.sqlite" db)
    c.Open()
    c
    
let createPeopleTable (connection:DbConnection) =
    let sql = "CREATE TABLE IF NOT EXISTS people (
                 id INTEGER PRIMARY KEY,
                 name TEXT NOT NULL,
                 email text NOT NULL UNIQUE
                );"
    execute connection sql None

let insertPerson (connection:DbConnection) name email  =
    let data = [("@name",box name);("@email",box email)] |> dict |> fun d -> new Dapper.DynamicParameters(d)
    let sql = "INSERT INTO people (name,email) VALUES (@name,@email);"
    execute connection sql data

type CreatePerson = { name:string; email:string }
let insertPerson2 (connection:DbConnection) (person:CreatePerson)  =
    let sql = "INSERT INTO people (name,email) VALUES (@name,@email);"
    execute connection sql person

let updatePerson (connection:DbConnection) id name email =
    let data = [("@id",box id);("@name",box name);("@email",box email)] 
                |> dict |> fun d -> new Dapper.DynamicParameters(d)
    let sql = "UPDATE people SET name=@name, email=@email WHERE id=@id"
    execute connection sql data

[<CLIMutable>]
type UpdatePerson = { id:int; name:string; email:string }
let updatePerson2 (connection:DbConnection) (person:UpdatePerson) =
    let sql = "UPDATE people SET name=@name, email=@email WHERE id=@id"
    execute connection sql person

let printCommandResults (r:Result<int,exn>) =
    match r with 
    | Ok -1 -> printfn "Nothing happened "
    | Ok i -> printfn "%i changes" i
    | Error err -> printfn "ERROR: %A" (err |> unwrapExn |> List.map (fun e -> e.Message) )

let dbName = "test"
let connection = conn dbName

createPeopleTable connection |> printCommandResults
// insertPerson connection "Sue" "sue@acme.com" |> printCommandResults
// insertPerson2 connection {name = "Ali"; email = "ali@acme.com"} |> printCommandResults
let updatedPerson = { id=2; name="Devon Burriss"; email="devonb@acme.com"}
updatePerson2 connection updatedPerson |> printCommandResults


let findAcmeEmployees (connection:DbConnection) =
    let sql = "SELECT id,name,email FROM people WHERE email LIKE '%@acme.com'"
    query connection sql None

match (findAcmeEmployees connection) with 
| Ok people -> printfn "Found %i employees" (Seq.length people)
| Error ex -> printfn "%A" ex.Message

let personById (connection:DbConnection) id =
    let data = [("@id",box id)] |> dict |> Some
    let sql = "SELECT id,name,email FROM people WHERE id = @id"
    querySingle connection sql data

match (personById connection 1) with 
| Ok (Some(person)) -> printfn "Found %i : %s %s" person.id person.name person.email
| Ok None -> printfn "No person found"
| Error ex -> printfn "%A" ex.Message

connection.Close()
connection.Dispose()