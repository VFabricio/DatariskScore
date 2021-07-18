module Program

let app = Saturn.Application.application {
    use_router Router.router
    use_config (fun _ -> Config.initialize())
}

[<EntryPoint>]
let main _ =
    Saturn.Application.run app
    0
