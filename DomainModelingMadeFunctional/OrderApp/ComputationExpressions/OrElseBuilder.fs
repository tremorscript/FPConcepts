namespace OrderApp

module OrElseBuilder1 =
    
    (*
    Chains of "or else" tests
    
    Sometimes the flow of control depends on a series of "or else" tests.
    Try one thing, and if that succeeds, you're done.
    Otherwise try another thing, and if that fails, try a third thing, and so on.
    
    Say that we have three dictionaries and we want to find the value corresponding to a key.
    Each lookup might succeed or fail, so we need to chain the lookups in a series.
    *)
    
    let map1 = [("1", "One"); ("2", "Two")] |> Map.ofList
    let map2 = [("A", "Alice"); ("B", "Bob")] |> Map.ofList
    let map3 = [("CA", "California"); ("NY", "New York")] |> Map.ofList
    
    let multiLookup key =
        match map1.TryFind key with
        | Some result1 -> Some result1 //success
        | None -> //failure
            match map2.TryFind key with
            | Some result2 -> Some result2 //success
            | None -> //failure
                match map3.TryFind key with
                | Some result3 -> Some result3 //success
                | None -> None //failure
    
    (*
    Because everything is an expression in F# we can't do an early return, we have to cascade all the tests in a single expression.
    
    *)
    
    multiLookup "A" |> printfn "Result for A is %A"
    multiLookup "CA" |> printfn "Result for CA is %A"
    multiLookup "X" |> printfn "Result for X is %A"
    
    (*
    Here is an "or else" builder that allows us to simplify these kinds of lookups.
    *)
    
    type OrElseBuilder() =
        member this.ReturnFrom(x) = x
        member this.Combine (a,b) =
            match a with
            | Some _ -> 
                printfn "Success :%A" a
                a   // a succeeds -- use it
            | None -> 
                printfn "Failure: %A" b
                b     // a fails -- use b instead
        member this.Delay(f) = f()
    
    let orElse = new OrElseBuilder()
    
    let multiLookupM key = orElse {
        return! map1.TryFind key
        return! map2.TryFind key
        return! map3.TryFind key
    }
    
    multiLookupM "A" |> printfn "Result for A is %A"
    multiLookupM "CA" |> printfn "Result for CA is %A"
    multiLookupM "X" |> printfn "Result for X is %A"