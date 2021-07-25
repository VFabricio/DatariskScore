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
    let app = Config.initializeForTest() |> Program.createApp
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

let postCpfNonStringCpf = task {
    use host = createHost().Start()
    let client = host.GetTestClient()

    let body = JsonContent.Create({| cpf = 42 |})
    let! response = client.PostAsync("/score", body)

    Expect.equal
        response.StatusCode
        HttpStatusCode.BadRequest
        "a POST where the CPF is not a string should return 400"
}

let postCpfSuccess = task {
    use host = createHost().Start()
    let client = host.GetTestClient()

    let body = JsonContent.Create({| cpf = "12345678910" |})
    let! response = client.PostAsync("/score", body)

    Expect.equal
        response.StatusCode
        HttpStatusCode.Created
        "a POST with a new, valid CPF should return 201"
}

let postCpfTwice = task {
    use host = createHost().Start()
    let client = host.GetTestClient()

    let body = JsonContent.Create({| cpf = "12345678910" |})
    let! _firstResponse = client.PostAsync("/score", body)
    let! secondResponse = client.PostAsync("/score", body)

    Expect.equal
        secondResponse.StatusCode
        HttpStatusCode.Conflict
        "a POST with an existing, valid CPF should return 409"
}

let getNonExistingCpf = task {
    use host = createHost().Start()
    let client = host.GetTestClient()

    let! response = client.GetAsync("/score/12345678910")

    Expect.equal
        response.StatusCode
        HttpStatusCode.NotFound
        "a GET with a non-existing CPF should return 404"
}

let postAndGetCpf = task {
    use host = createHost().Start()
    let client = host.GetTestClient()

    let body = JsonContent.Create({| cpf = "12345678910" |})
    let! _firstResponse = client.PostAsync("/score", body)

    let! secondResponse = client.GetAsync("/score/12345678910")

    Expect.equal
        secondResponse.StatusCode
        HttpStatusCode.OK
        "a GET after posting a new, valid CPF should return 200"
}

[<Tests>]
let tests =
    testList "HTTP integration tests" [
        testCaseAsync "postCpfNotJson" (Async.AwaitTask postCpfNotJson)
        testCaseAsync "postCpfNoCpf" (Async.AwaitTask postCpfNoCpf)
        testCaseAsync "postCpfNonStringCpf" (Async.AwaitTask postCpfNonStringCpf)
        testCaseAsync "postCpfSuccess" (Async.AwaitTask postCpfSuccess)
        testCaseAsync "postCpfTwice" (Async.AwaitTask postCpfTwice)
        testCaseAsync "getNonExistingCpf" (Async.AwaitTask getNonExistingCpf)
        testCaseAsync "postAndGetCpf" (Async.AwaitTask postAndGetCpf)
    ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
