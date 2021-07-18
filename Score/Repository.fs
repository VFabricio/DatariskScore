module Score.Repository

open Database
open FSharp.Control.Tasks
open Npgsql
open Score.Domain
open System.Threading.Tasks

let insert (connectionString: string) (record: Score): Task<Result<int, exn>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        return! execute
            connection
            "INSERT INTO score(id, cpf, score, created_at) VALUES (@Id, @Cpf, @Value, @CreatedAt)"
            record
    }

