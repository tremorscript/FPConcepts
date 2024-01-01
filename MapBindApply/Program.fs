// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

// Map is something that takes a function in the normal world and lifts it into an elevated world
// (a -> b) -> E<a> -> E<b>

// apply unpacks a function wrapped inside an elevated value into a lifted function
// E<(a->b)> -> E<a> -> E<b>

module Option =

    // the apply function for options
    let apply fOpt xOpt =
        match fOpt, xOpt with
        | Some f, Some x -> Some(f x)
        | _ -> None

    //infix versions of map and apply
    let (<!>) = Option.map
    let (<*>) = apply

    // The lift functions take a normal function with N parameters and transform it to a corresponding elevated function.
    let lift2 f x y = f <!> x <*> y

    let lift3 f x y z = f <!> x <*> y <*> z

    let lift4 f x y z w = f <!> x <*> y <*> z <*> w

module List =

    // the apply function for lists
    // [f;g;] apply [x;y] becomes [f x; f y; g x; g y]
    let apply (flist: ('a -> 'b) list) (xList: 'a list) =
        [ for f in flist do
              for x in xList do
                  yield f x ]


//infix version of apply
let add x y = x + y

let resultOption =
    let (<*>) = Option.apply
    (Some add) <*> (Some 2) <*> (Some 3)
// resultOption = Some 5

let resultList =
    let (<*>) = List.apply
    [ add ] <*> [ 1; 2 ] <*> [ 10; 20 ]


// If you have apply and return, you can construct map from them
let resultOption1 =
    let (<!>) = Option.map
    let (<*>) = Option.apply

    add <!> (Some 2) <*> (Some 3)

let resultList1 =
    let (<!>) = List.map
    let (<*>) = List.apply

    add <!> [ 1; 2 ] <*> [ 10; 20 ]
