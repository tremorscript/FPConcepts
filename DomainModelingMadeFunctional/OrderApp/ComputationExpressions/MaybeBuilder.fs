namespace OrderApp
(*
    Chains of if/else statements
*)

(*

Say we want to divide a series of numbers, one after another but one of them might be zero.
We can create a helper function that does the division and gives us back an int option.
If everything is OK, we get a Some and if the division fails, we get a None.

Then we chain the divisions together and keep going only if it was successful.

*)

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
    
    let good1 = divideByWorkflowM 12 3 2 1
    let good2 = maybe.Bind(divideBy 3 12, fun a ->
                                    maybe.Bind(divideBy 2 a, fun b ->
                                        maybe.Bind(divideBy 1 b, fun c ->
                                            Some c
                                            )))
    
    let bad1 = divideByWorkflowM 12 3 0 1
    let bad2 = maybe.Bind(divideBy 3 12, fun a ->
                                    maybe.Bind(divideBy 0 a, fun b ->
                                        maybe.Bind(divideBy 1 b, fun c ->
                                            Some c
                                            )))
