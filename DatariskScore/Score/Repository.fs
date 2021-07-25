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
    Cpf = Cpf.create dao.cpf
    Value = dao.score
}

let domainToDao (score: Score) = {
    id = score.Id
    created_at = score.CreatedAt
    cpf = score.Cpf.ToString()
    score = score.Value
}

let insert (connectionString: string) (score: Score): Task<Result<unit, RepositoryError>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let dao = domainToDao score
        let! result =
            execute
                connection
                "INSERT INTO score(id, cpf, score, created_at) VALUES (@id, @cpf, @score, @created_at)"
                dao

        let response =
            match result with
                | Ok _ ->  Ok ()
                | Error ex ->  handleInsertError ex |> Error

        return response
    }

let getByCpf (connectionString: string) (cpf: Cpf.T): Task<Result<Option<Score>, RepositoryError>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let! (result: Result<Option<ScoreDao>, exn>) =
            querySingle
                connection
                "SELECT id, cpf, score, created_at FROM score WHERE cpf=@cpf"
                (dict ["cpf" => cpf.ToString()] |> Some)

        let response =
            result
            |> Result.mapError (fun _ -> UnknownError)
            |> Result.map (Option.map daoToDomain)
        return response
    }

