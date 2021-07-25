module DatariskScore.Tests

open System
open Expecto
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.TestHost
open System.Net
open System.Net.Http
open System.Net.Http.Json

let configureWebHostBuilder (builder: IWebHostBuilder) =
    builder.UseTestServer() |> ignore

let createHost () =
    let app = Program.createApp()
    app.ConfigureWebHost(Action<IWebHostBuilder> configureWebHostBuilder)

let postCpfNotJson = task {
    use host = createHost().Start()
    let client = host.GetTestClient()

    let body = new StringContent("")
    let! response = client.PostAsync("/score", body)

    Expect.equal
        response.StatusCode
        HttpStatusCode.BadRequest
        "a POST with a non-JSON body should return 400"
}

let postCpfNoCpf = task {
    use host = createHost().Start()
    let client = host.GetTestClient()

    let body = JsonContent.Create({| |})
    let! response = client.PostAsync("/score", body)

    Expect.equal
        response.StatusCode
        HttpStatusCode.BadRequest
        "a POST without the CPF property should return 400"
}

[<Tests>]
let tests =
    testList "HTTP integration tests" [
        testCaseAsync "postCpfNotJson" (Async.AwaitTask postCpfNotJson)
        testCaseAsync "postCpfNoCpf" (Async.AwaitTask postCpfNoCpf)
    ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
