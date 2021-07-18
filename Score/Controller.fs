module Score.Controller

open Config
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Saturn.ControllerHelpers
open Score.Commands

let getPersonalId ctx id =
    Controller.text ctx (sprintf "%d" id)

let submitPersonalId (ctx: HttpContext) =
    task {
        let config: Config = Controller.getConfig ctx
        let connectionString = config.ConnectionString

        let! scoreDto = Controller.getJson<CreateScoreDto> ctx

        let! result = createScore connectionString scoreDto
        match result with
        | Ok(_) -> return! Controller.text ctx "ok"
        | Error(_) -> return! Controller.text ctx "error"
    }

let controller = Saturn.Controller.controller {
    create submitPersonalId
    show getPersonalId
}
