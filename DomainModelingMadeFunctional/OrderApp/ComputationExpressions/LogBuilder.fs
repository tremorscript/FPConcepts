namespace OrderApp

module LogBuilder  =

    (*
        Let's say we have some code, and we want to log each step.
    *)

    let log p = printfn "expression is %A" p

    let loggedWorkflow =
        let x = 42
        log x
        let y = 43
        log y
        let z = x + y
        log z
        //return
        z

    (*
        But it is annoying to have to explicitly write all the log statements each time.
        Is there a way to hide them?
        A computation expression can do that.
    *)

    type LoggingBuilder() =
        let log p = printfn "expression is %A" p

        member this.Bind(x, f) =
            log x
            f x

        member this.Return(x) =
            x

    (*
        Next we create an instance of the type, logger in this case.
    *)

    let logger = new LoggingBuilder()

    (*
        So with this logger value, we can rewrite the original logging example like this:
    *)

    let loggedWorkflow1 =
        logger {
            let! x = 42
            let! y = 43
            let! z = x + y
            return z
        }

    let value = logger.Bind(42, fun x ->
                                    logger.Bind(43, fun y ->
                                        logger.Bind(x + y, fun z -> z)
                                        ))
module MaybeBuilder1 =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some (top/bottom)

    (*
    Here is a workflow that attempts to divide a starting number three times.
    *)

    let divideByWorkflow init x y z =
        let a = init |> divideBy x
        match a with
        | None -> None  //give up
        | Some a' ->    //keep going
            let b = a' |> divideBy y
            match b with
            | None -> None
            | Some b' ->
                let c = b' |> divideBy z
                match c with
                | None -> None
                | Some c' ->
                    Some c'

    let good = divideByWorkflow 12 3 2 1
    let bad = divideByWorkflow 12 3 0 1

    (*
    This continual testing and branching is really ugly!
    Does turning it into computation expression help?
    *)

    (*
    Once more we define a new type(MaybeBuilder) and make an instance of the type (maybe)
    *)

    type MaybeBuilder() =

        member this.Bind(x,f) =
            match x with
            | None -> None
            | Some a -> f a

        member this.Return(x) =
            Some x

    let maybe = new MaybeBuilder()

    let divideByWorkflowM init x y z =
        maybe
            {
                let! a = init |> divideBy x
                let! b = a |> divideBy y
                let! c = b |> divideBy z
                return c
            }

    let good1 = divideByWorkflow 12 3 2 1
    let good2 = maybe.Bind(divideBy 3 12, fun a ->
                                    maybe.Bind(divideBy 2 a, fun b ->
                                        maybe.Bind(divideBy 1 b, fun c ->
                                            Some c
                                            )))

    let bad1 = divideByWorkflow 12 3 0 1
    let bad2 = maybe.Bind(divideBy 3 12, fun a ->
                                    maybe.Bind(divideBy 0 a, fun b ->
                                        maybe.Bind(divideBy 1 b, fun c ->
                                            Some c
                                            )))
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
            | Some _ -> a   // a succeeds -- use it
            | None -> b     // a fails -- use b instead
        member this.Delay(f) = f()

    let orElse = new OrElseBuilder()

    let multiLookupM key = orElse {
        return! map1.TryFind key
        return! map2.TryFind key
        return! map3.TryFind key
    }

    let multiLookupExpanded key =  
        orElse.Delay(fun () ->
        orElse.Combine(orElse.ReturnFrom(map1.TryFind key),
        orElse.Delay(fun () ->
        orElse.Combine(orElse.ReturnFrom(map2.TryFind key),
        orElse.Delay(fun () -> orElse.ReturnFrom(map3.TryFind key))))))

    multiLookupM "A" |> printfn "Result for A is %A"
    multiLookupM "CA" |> printfn "Result for CA is %A"
    multiLookupM "X" |> printfn "Result for X is %A"

    multiLookupExpanded "A" |> printfn "Result for A is %A"
    multiLookupExpanded "CA" |> printfn "Result for CA is %A"
    multiLookupExpanded "X" |> printfn "Result for X is %A"