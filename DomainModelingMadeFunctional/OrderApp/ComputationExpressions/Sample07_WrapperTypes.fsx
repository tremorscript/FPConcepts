(*
One of the major uses of computation expressions is to implicitly unwrap and rewrap values that are stored in some sort of wrapper type.
*)

(*
Say we are accessing a database, and we want to capture the result in a Success/Error union type:
*)

type DbResult<'a> =
    | Success of 'a
    | Error of string

(*
We then use this type in our database access methods.
*)

let getCustomerId name =
    if name = ""
    then Error "getCustomerId failed"
    else Success "Cust42"

let getLastOrderForCustomer custId =
    if custId = ""
    then Error "getLastOrderForCustomer failed"
    else Success "Order123"

let getLastProductForOrder orderId =
    if orderId = ""
    then Error "getLastProductForOrder failed"
    else Success "Product456"

(*
Now lets say we want to chain these calls together.

Here is the most explicit way of doing it.
As you can see, we have to have pattern matching at each step.
*)

let product =
    let r1 = getCustomerId "Alice"
    match r1 with
    | Error _ -> r1
    | Success custId ->
        let r2 = getLastOrderForCustomer custId
        match r2 with
        | Error _ -> r2
        | Success orderId ->
            let r3 = getLastProductForOrder orderId
            match r3 with
            | Error _ -> r3
            | Success productId ->
                printfn "Product is %s" productId
                r3

(*
Really ugly code. And the top-level flow has been submerged in the error handling logic.
*)

(*
Computation expressions to the rescue! We can write one that handles the branching of Success/Error behind the scenes:
*)

type DbResultBuilder() =

    member this.Bind(x,f) =
        match x with
        | Error _ -> x
        | Success y ->
            printfn "\tSuccessful: %s" y
            f y

    member this.Return(x) =
        Success x

let dbResult = new DbResultBuilder();

(*
And with this workflow, we can focus on the big picture and write much cleaner code:
*)

let product' =
    dbResult {
        let! custId = getCustomerId "Alice"
        let! orderId = getLastOrderForCustomer custId
        let! productId = getLastProductForOrder orderId
        printfn "Product is %s" productId
        return productId
    }

printfn "%A" product

(*
And if there are errors, the workflow traps them nicely and tells us where the error was:
*)

let product'' =
    dbResult {
        let! custId = getCustomerId "Alice"
        let! orderId = getLastOrderForCustomer "" //error
        let! productId = getLastProductForOrder orderId
        printfn "Product is %s" productId
        return productId
    }

printfn "%A" product

(*
Every computation expression must have an associated wrapper type.
And the wrapper type is often designed to go hand-in-hand with the workflow that we want to manage.
*)

