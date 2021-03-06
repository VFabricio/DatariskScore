module Program

open Config
open Giraffe
open Microsoft.Extensions.DependencyInjection
open System.Text.Json.Serialization

let configureServices (services: IServiceCollection) =
    services.AddGiraffe() |> ignore

    let serializationOptions = SystemTextJson.Serializer.DefaultOptions
    serializationOptions.Converters.Add(JsonFSharpConverter())

    services.AddSingleton<Json.ISerializer>(SystemTextJson.Serializer(serializationOptions))
    |> ignore

    services

let createApp (config: Config) =
    Saturn.Application.application {
        use_router Router.router
        use_config (fun _ -> config)
        service_config configureServices
    }

[<EntryPoint>]
let main _ =
    initialize ()
    |> createApp
    |> Saturn.Application.run

    0
