open System.Net

let req1 = HttpWebRequest.Create("http://tryfsharp.org")
let req2 = HttpWebRequest.Create("http://google.com")
let req3 = HttpWebRequest.Create("http://bing.com")

req1.BeginGetResponse((fun r1 ->
    use resp1 = req1.EndGetResponse(r1)
    printfn "Downloaded %O" resp1.ResponseUri

    req2.BeginGetResponse((fun r2 ->
        use resp2 = req2.EndGetResponse(r2)
        printfn "Downloaded %O" resp2.ResponseUri

        req3.BeginGetResponse((fun r3 ->
            use resp3 = req3.EndGetResponse(r3)
            printfn "Downloaded %O" resp3.ResponseUri
            ),null) |> ignore
        ),null) |> ignore
    ),null) |> ignore

async {
    use! resp1 = req1.AsyncGetResponse()
    printfn "Downloaded %O" resp1.ResponseUri
    use! resp2 = req2.AsyncGetResponse()
    printfn "Downloaded %O" resp2.ResponseUri
    use! resp3 = req3.AsyncGetResponse()
    printfn "Downloaded %O" resp3.ResponseUri
} |> Async.RunSynchronously

(*
A computation expression is an expression that does something in the background between each let

*)