#r "packages/Dapper/lib/net451/Dapper.dll"

open System
open Dapper
open System.Data.Common
open System.Collections.Generic

let execute (connection:#DbConnection) (sql:string) (data:_) =
    async {
        try
            let! res = connection.ExecuteAsync(sql, data) |> Async.AwaitTask
            return Ok res
        with
        | ex -> return Error ex
    }

let query (connection:#DbConnection) (sql:string) (parameters:IDictionary<string, obj> option) =
    async {
        try
            let! res =
                match parameters with
                | Some p -> connection.QueryAsync<'T>(sql, p) |> Async.AwaitTask
                | None -> connection.QueryAsync<'T>(sql) |> Async.AwaitTask
            return Ok res
        with
        | ex -> return Error ex
    }

let querySingle (connection:#DbConnection) (sql:string) (parameters:IDictionary<string, obj> option) =
    async {
        try
            let! res =
                match parameters with
                | Some p -> connection.QuerySingleOrDefaultAsync<'T>(sql, p) |> Async.AwaitTask
                | None -> connection.QuerySingleOrDefaultAsync<'T>(sql) |> Async.AwaitTask
            return
                if isNull (box res) then Ok None
                else Ok (Some res)

        with
        | ex -> return Error ex
    }