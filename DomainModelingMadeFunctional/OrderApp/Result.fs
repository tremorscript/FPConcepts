﻿namespace global

open System

//==============================================
// Helpers for Result type and AsyncResult type
//==============================================

/// Functions for Result type (functor and monad).
/// For applicatives, see Validation.
[<RequireQualifiedAccess>]  // RequireQualifiedAccess forces the `Result.xxx` prefix to be used
module Result =

    /// Pass in a function to handle each case of `Result`
    let bimap onSuccess onError xR =
        match xR with
        | Ok x -> onSuccess x
        | Error err -> onError err

    // F# VERSION DIFFERENCE
    // The `map`, `mapError` and `bind` functions are in a different module in F# 4.1 and newer (from VS2017),
    // so these aliases make them available in this module.
    // In older versions of F#, where the functions are defined above, please comment them out
    let map = Result.map
    let mapError = Result.mapError
    let bind = Result.bind

    // Like `map` but with a unit-returning function
    let iter (f: _ -> unit) result =
        map f result |> ignore

    /// Apply a Result<fn> to a Result<x> monadically
    let apply fR xR =
        match fR, xR with
        | Ok f, Ok x -> Ok (f x)
        | Error err1, Ok _ -> Error err1
        | Ok _, Error err2 -> Error err2
        | Error err1, Error _ -> Error err1

    //combine a list of results, monadically
    let sequence aListOfResults =
        let (<*>) = apply //monadic
        let (<!>) = map
        let cons head tail = head :: tail
        let consR headR tailR = cons <!> headR <*> tailR
        let initialValue = Ok [] //empty list inside Result

        //loop through the list, prepending each element
        // to the initial value
        List.foldBack consR aListOfResults initialValue

    //-----------------------------------
    // Lifting

    /// Lift a two parameter function to use Result parameters
    let lift2 f x1 x2 =
        let (<!>) = map
        let (<*>) = apply
        f <!> x1 <*> x2

    /// Lift a three parameter function to use Result parameters
    let lift3 f x1 x2 x3 =
        let (<!>) = map
        let (<*>) = apply
        f <!> x1 <*> x2 <*> x3

    /// Lift a four parameter function to use Result parameters
    let lift4 f x1 x2 x3 x4 =
        let (<!>) = map
        let (<*>) = apply
        f <!> x1 <*> x2 <*> x3 <*> x4

    /// Apply a monadic function with two parameters
    let bind2 f x1 x2 = lift2 f x1 x2 |> bind id

    /// Apply a monadic function with three parameters
    let bind3 f x1 x2 x3 = lift3 f x1 x2 x3 |> bind id

    //-----------------------------------
    // Predicates

    /// Predicate that returns true on success
    let isOk =
        function
        | Ok _ -> true
        | Error _ -> false

    /// Predicate that returns true on failure
    let isError xR =
        xR |> isOk |> not

    /// Lift a given predicate into a predicate that works on Results
    let filter pred =
        function
        | Ok x -> pred x
        | Error _ -> true

    //-----------------------------------
    // Mixing simple values and results

    /// On success, return the value. On error, return a default value
    let ifError defaultVal =
        function
        | Ok x -> x
        | Error _ -> defaultVal

    //-----------------------------------
    // Mixing options and results

    /// Apply a monadic function to an Result<x option>
    let bindOption f xR =
       match xR with
       | Some x -> f x |> map Some
       | None -> Ok None

    /// Convert an Option into a Result. If none, use the passed-in errorValue
    let ofOption errorValue opt =
        match opt with
        | Some v -> Ok v
        | None -> Error errorValue

    // Convert a Result into an Option
    let toOption xR =
        match xR with
        | Ok v -> Some v
        | Error _ -> None

    /// Convert the Error case into an Option
    /// (useful with List.choose to find all errors in a list of Results)
    let toErrorOption =
        function
        | Ok _ -> None
        | Error er -> Some er