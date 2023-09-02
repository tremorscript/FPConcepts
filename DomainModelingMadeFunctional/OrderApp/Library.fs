namespace OrderApp

module Say =
    let hello name =
        printfn "Hello %s" name

    let map1 = [("1", "One"); ("2", "Two")] |> Map.ofList
    let map2 = [("A", "Alice"); ("B", "Bob")] |> Map.ofList
    let map3 = [("CA", "California"); ("NY", "New York")] |> Map.ofList

    type OrElseBuilder() =
        member this.ReturnFrom(x) = x
        member this.Combine (a,b) =
            match a with
            | Some _ -> a   // a succeeds -- use it
            | None -> b     // a fails -- use b instead
        member this.Delay(f) = f()

    let orElse = new OrElseBuilder()

    let multiLookupM key = orElse {
        return! map1.TryFind key
        return! map2.TryFind key
        return! map3.TryFind key
    }
