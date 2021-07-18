module Score.Controller

open Config
open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Saturn.ControllerHelpers
open Score.Commands
open Score.Domain
open Score.Repository
open System.Text.Json

let handleGetError (ctx: HttpContext) =
    ServerErrors.internalError (json {| error = "Internal server error" |}) earlyReturn ctx

let handleScoreFound (ctx: HttpContext) (score: Score) =
    Successful.ok (json score) earlyReturn ctx

let handleScoreNotFound (ctx: HttpContext) =
    RequestErrors.notFound
        (json {| error = "There is no score for this CPF" |})
        earlyReturn
        ctx

let getPersonalId ctx cpf =
    task {
        let config: Config = Controller.getConfig ctx
        let connectionString = config.ConnectionString
        let! result = getByCpf connectionString cpf
        let response =
            match result with
                | Ok (Some s) -> handleScoreFound ctx s
                | Ok None -> handleScoreNotFound ctx
                | Error _ -> handleGetError ctx

        return! response
    }


let handleCreateError (ctx: HttpContext) (error: RepositoryError) =
    match error with
    | RecordAlreadyExists ->
        RequestErrors.conflict
            (json {| error = "There is already a score for this CPF" |})
            earlyReturn
            ctx
    | UnknownError ->
        ServerErrors.internalError
            (json {| error = "Internal server error" |})
            earlyReturn
            ctx

let handleCreateOk (ctx: HttpContext) =
    Successful.created (text "") earlyReturn ctx

let handleJsonException (ctx: HttpContext) =
    RequestErrors.badRequest (json {| error = "Bad Input!" |}) earlyReturn ctx

let handleOtherExceptions (ctx: HttpContext) =
    ServerErrors.internalError (json {| error = "Internal server error" |}) earlyReturn ctx

let submitPersonalId (ctx: HttpContext) =
    task {
        try
            let config: Config = Controller.getConfig ctx
            let connectionString = config.ConnectionString

            let! scoreDto = Controller.getJson<CreateScoreDto> ctx

            let! result = createScore connectionString scoreDto
            match result with
            | Ok _ -> return! handleCreateOk ctx
            | Error e -> return! handleCreateError ctx e

        with
        | :? JsonException -> return! handleJsonException ctx
        | _ -> return! handleOtherExceptions ctx
    }

let controller = Saturn.Controller.controller {
    create submitPersonalId
    show getPersonalId
}
