module Database

open Dapper
open FSharp.Control.Tasks
open Npgsql
open System.Collections.Generic
open System.Data.Common
open System.IO

module Sql = Npgsql.FSharp.Sql

let getConnectionString () =
    let username = "postgres"
    let password = "password"
    let host = "localhost"
    let port = 5432
    let database = "datarisk"

    Sql.host host
    |> Sql.username username
    |> Sql.password password
    |> Sql.port port
    |> Sql.database database
    |> Sql.formatConnectionString


let inline (=>) k v = k, box v

let execute (connection: #DbConnection) (sql: string) (data: _) =
    task {
        try
            let! res = connection.ExecuteAsync(sql, data)
            return Ok res
        with
        | ex -> return Error ex
    }

let query (connection: #DbConnection) (sql: string) (parameters: Option<IDictionary<string, obj>>) =
    task {
        try
            let! res =
                match parameters with
                | Some p -> connection.QueryAsync<'T>(sql, p)
                | None -> connection.QueryAsync<'T>(sql)
            return Ok res
        with
        | ex -> return Error ex
    }

let querySingle (connection: #DbConnection) (sql: string) (parameters: Option<IDictionary<string, obj>>) =
    task {
        try
            let! res =
                match parameters with
                | Some p -> connection.QuerySingleOrDefaultAsync<'T>(sql, p)
                | None -> connection.QuerySingleOrDefaultAsync<'T>(sql)
            return
                if isNull (box res) then Ok None
                else Ok (Some res)

        with
        | ex -> return Error ex
    }

let getTestConnectionString () =
    let id = System.Guid.NewGuid()

    let username = "postgres"
    let password = "password"
    let host = "localhost"
    let port = 5432
    let database = $"datarisk-{id}"

    let builderWithoutDb =
        Sql.host host
        |> Sql.username username
        |> Sql.password password
        |> Sql.port port

    let currentDirectory = __SOURCE_DIRECTORY__
    let sqlPath = Path.Combine [| currentDirectory; "Database"; "create-score-table.sql" |]
    let createScoreSql = File.ReadAllText(sqlPath)

    let connectionStringWithoutDb = Sql.formatConnectionString builderWithoutDb
    use connectionWithoutDb = new NpgsqlConnection(connectionStringWithoutDb)

    connectionWithoutDb.Execute($"CREATE DATABASE \"{database}\"") |> ignore

    let connectionStringWithDb =
        builderWithoutDb
            |> Sql.database database
            |> Sql.formatConnectionString

    let connectionWithDb = new NpgsqlConnection(connectionStringWithDb)
    connectionWithDb.Execute(createScoreSql) |> ignore

    connectionStringWithDb

