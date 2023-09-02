(*
	Operations with and without ‘!’
	The difference is easy to remember when you realize that the operations without a “!” always have unwrapped types
	on the right hand side, while the ones with a “!” always have wrapped types.

	The “!” versions are particularly important for composition, because the wrapped type can be the result of another computation expression of the same type.
*)

module Sample1 =

    type TraceBuilder() =

        member this.Bind(x,f) =
            match x with
            | None ->
                printfn "Binding with None. Exiting"
            | Some a ->
                printfn "Binding with Some(%A). Continuing" a
            Option.bind f x

        member this.Return(x) =
            printfn "Returning an unwrapped %A as an option" x
            Some x

        member this.ReturnFrom(x) =
            printfn "Returning an option (%A) directly" x
            x

    // make an instance of the workflow
    let trace = new TraceBuilder()

    //Lets run some code sample through it
    trace {
        return 1
    } |> printfn "Result 1: %A"

    trace {
        return! 2
    } |> printfn "Result 2: %A"

    trace {
        let! x = Some 1
        let! y = Some 2
        return x + y
    } |> printfn "Result 3: %A"

    trace {
        let! x = None
        let! y = Some 1
        return x + y
    } |> printfn "Result 4: %A"

module Sample2_Introducingdo =
    (*
        In normal F#, do is just like let, except that the expression doesn’t return anything useful (namely, a unit value).
        Inside a computation expression, do! is very similar. Just as let! passes a wrapped result to the Bind method, so does do!,
        except that in the case of do! the “result” is the unit value, and so a wrapped version of unit is passed to the bind method.
    *)

    type TraceBuilder() =

        member this.Bind(x,f) =
            match x with
            | None ->
                printfn "Binding with None. Exiting"
            | Some a ->
                printfn "Binding with Some(%A). Continuing" a
            Option.bind f x

        member this.Return(x) =
            printfn "Returning an unwrapped %A as an option" x
            Some x

        member this.ReturnFrom(x) =
            printfn "Returning an option (%A) directly" x
            x

    // make an instance of the workflow
    let trace = new TraceBuilder()

    trace {
        do! Some (printfn "..expression that returns unit")
        do! Some (printfn "...another expression that returns unit")
        let! x = Some (1)
        return x
    }