module Score.Repository

open Database
open FSharp.Control.Tasks
open Npgsql
open Score.Domain
open System.Threading.Tasks

type RepositoryError = RecordAlreadyExists | UnknownError

let handlePgException (ex: PostgresException) =
    if ex.SqlState.Equals(PostgresErrorCodes.UniqueViolation)
    then
        RecordAlreadyExists
    else
        UnknownError

let handleInsertError (ex: exn) =
    match ex with
        | :? PostgresException as pgEx -> handlePgException pgEx
        | _ -> UnknownError

let insert (connectionString: string) (record: Score): Task<Result<unit, RepositoryError>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let! result =
            execute
                connection
                "INSERT INTO score(id, cpf, score, created_at) VALUES (@Id, @Cpf, @Value, @CreatedAt)"
                record

        let response =
            match result with
                | Ok _ ->  Ok ()
                | Error ex ->  handleInsertError ex |> Error

        return response
    }

