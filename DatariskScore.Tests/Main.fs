module DatariskScore.Tests

open System
open Expecto
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.TestHost
open System.Net

let configureWebHostBuilder (builder: IWebHostBuilder) =
    builder.UseTestServer() |> ignore

let createHost () =
    Program.app.ConfigureWebHost(Action<IWebHostBuilder> configureWebHostBuilder)

let getScore = task {
    use host = createHost().Start()
    let client = host.GetTestClient()
    let! response = client.GetAsync("/score/12345678901")
    let code = response.StatusCode
    Expect.equal code HttpStatusCode.NotFound "the response must be a 404"
}

[<Tests>]
let tests =
    testList "HTTP integration tests" [
        testCaseAsync "foo" (Async.AwaitTask getScore)
    ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
