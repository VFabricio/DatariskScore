module Score.Controller

open Config
open FSharp.Control.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Saturn.ControllerHelpers
open Score.Commands
open System.Text.Json

let getPersonalId ctx id =
    Controller.text ctx (sprintf "%d" id)

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
            | Ok(_) -> return! handleCreateOk ctx
            | Error(_) -> return! Controller.text ctx "error"

        with
        | :? JsonException -> return! handleJsonException ctx
        | _ -> return! handleOtherExceptions ctx
    }

let controller = Saturn.Controller.controller {
    create submitPersonalId
    show getPersonalId
}
