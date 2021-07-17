module Program

let app = Saturn.Application.application {
    use_router Router.router
}

[<EntryPoint>]
let main _ =
    Saturn.Application.run app
    0
