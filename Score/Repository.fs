module Score.Repository

open Database
open FSharp.Control.Tasks
open Npgsql
open Score.Domain
open System.Threading.Tasks

module Result = FSharp.Core.Result
module Option = FSharp.Core.Option

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

[<CLIMutable>]
type ScoreDao = {
    // fsharplint:disable-next-line RecordFieldNames
    id: System.Guid
    // fsharplint:disable-next-line RecordFieldNames
    cpf: string
    // fsharplint:disable-next-line RecordFieldNames
    score: int
    // fsharplint:disable-next-line RecordFieldNames
    created_at: System.DateTime
}

let daoToDomain (dao: ScoreDao) = {
    Id = dao.id
    CreatedAt = dao.created_at
    Cpf = dao.cpf
    Value = dao.score
}

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

let getByCpf (connectionString: string) (cpf: string): Task<Result<Option<Score>, RepositoryError>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let! (result: Result<Option<ScoreDao>, exn>) =
            querySingle
                connection
                "SELECT id, cpf, score, created_at FROM score WHERE cpf=@cpf"
                (dict ["cpf" => cpf] |> Some)

        let response =
            result
            |> Result.mapError (fun _ -> UnknownError)
            |> Result.map (Option.map daoToDomain)
        return response
    }

