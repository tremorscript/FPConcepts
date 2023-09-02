(*
The let! expression is syntactic sugar for a Bind method.
*)
(*
In other words, if we chain a number of let! expressions together like this:
let! x = 1
let! y = 2
let! z = x + y

Bind(1, fun x ->
Bind(2, fun y ->
Bind(x + y, fun z ->
etc

Computation expressions are just a way to create nice syntax for something that we could do ourselves.

*)

(*
We’ve used four different approaches for the “safe divide” example so far.
Let’s put them together side by side and compare them once more.
*)

//First the original version, using an explicit workflow:
module DivideByExplicit =
     let divideBy bottom top =
      if bottom = 0
      then None
      else Some(top/bottom)

     let divideByWorkflow x y w z =
      let a = x |> divideBy y
      match a with
      | None -> None // give up
      | Some a' -> // keep going
        let b = a' |> divideBy w
        match b with
        | None -> None // give up
        | Some b' -> // keep going
         let c = b' |> divideBy z
         match c with
         | None -> None // give up
         | Some c' -> // keep going
          //return
          Some c'

(*
Next, using our own version of “bind” (a.k.a. “pipeInto”)
*)

module DivideByWithBindFunction =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some(top/bottom)

    let bind (m,f) =
        Option.bind f m

    let return' x = Some x

    let divideByWorkflow x y w z =
        bind (x |> divideBy y, fun a ->
        bind (a |> divideBy w, fun b ->
        bind (b |> divideBy z, fun c ->
        return' c
        )))

(*
Next, using a computation expression:
*)
module DivideByWithCompExpr =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some(top/bottom)

    type MaybeBuilder() =
        member this.Bind(m, f) = Option.bind f m
        member this.Return(x) = Some x

    let maybe = new MaybeBuilder()

    let divideByWorkflow x y w z =
        maybe
            {
            let! a = x |> divideBy y
            let! b = a |> divideBy w
            let! c = b |> divideBy z
            return c
            }

(*
And finally, using bind as an infix operation:
*)
module DivideByWithBindOperator =

    let divideBy bottom top =
        if bottom = 0
        then None
        else Some(top/bottom)

    let (>>=) m f = Option.bind f m

    let divideByWorkflow x y w z =
        x |> divideBy y
        >>= divideBy w
        >>= divideBy z