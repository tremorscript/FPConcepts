// General hints on defining types with constraints or invariants
//
// Just as in C#, use a private constructor
// and expose "factory" methods that enforce the constraints.
//
// In F#, only classes can have private constructors with public members.
//
// If you want to use the record and DU types, the whole type becomes
// private, which means that you also need to provide:
// * a constructor function ("create").
// * a function to extract the internal data ("value").


module ContrainedTypes

open System

// ---------------------------------------------
// Constrained String50 (FP-style)
// ---------------------------------------------

// Type with constraint that value must be non-null
// and <= 50 chars
type String50 = private String50 of string

// Module containing functions related to String50 type
module String50 =
    // NOTE: these functions can access the internals of the
    // type because they are in the same scope (namespace/module)

    // constructor
    let create str =
        if String.IsNullOrEmpty(str) then None
        elif String.length str > 50 then None
        else Some(String50 str)

    // function used to extract data since type is private
    let value (String50 str) = str

// ---------------------------------------------
// Constrained String50 (object-oriented style)
// ---------------------------------------------

// Type with constraint that value must be non-null and <= 50 chars
type OOString50 private (str) =
    // constructor
    static member Create str =
        if String.IsNullOrEmpty(str) then None
        elif String.length str > 50 then None
        else Some(OOString50(str))

    // extractor
    member this.Value = str

// ---------------------------------------------
// Constrained AtLeastOne (FP style)
// ---------------------------------------------

// Type with constraint that at least one of the fields must be set.
type AtLeastOne =
    private
        { A: int option
          B: int option
          C: int option }

// Module containing functions related to AtLeastOne type
module AtLeastOne =

    // constructor
    let create aOpt bOpt cOpt =
        match aOpt, bOpt, cOpt with
        | (Some a, _, _) -> Some <| { A = aOpt; B = bOpt; C = cOpt }
        | (_, Some b, _) -> Some <| { A = aOpt; B = bOpt; C = cOpt }
        | (_, _, Some c) -> Some <| { A = aOpt; B = bOpt; C = cOpt }
        | _ -> None
    // This might fail, so return option -- caller must test for None

    // These three always succeed, no need to test for None
    let createWhenAExists a bOpt cOpt = { A = Some a; B = bOpt; C = cOpt }
    let createWhenBExists aOpt b cOpt = { A = aOpt; B = Some b; C = cOpt }
    let createWhenCExists aOpt bOpt c = { A = aOpt; B = bOpt; C = Some c }

    // function used to extract data since type is private
    let value atLeastOne =
        let a = atLeastOne.A
        let b = atLeastOne.B
        let c = atLeastOne.C
        (a, b, c)

// ---------------------------------------------
// Constrained AtLeastOne (object-oriented style)
// ---------------------------------------------

// Class with constraint that at least one of the fields must be set.
type OOAtLeastOne private (aOpt: int option, bOpt: int option, cOpt: int option) =

    // constructor
    static member create(aOpt, bOpt, cOpt) =
        match aOpt, bOpt, cOpt with
        | (Some a, _, _) -> Some <| OOAtLeastOne(aOpt, bOpt, cOpt)
        | (_, Some b, _) -> Some <| OOAtLeastOne(aOpt, bOpt, cOpt)
        | (_, _, Some c) -> Some <| OOAtLeastOne(aOpt, bOpt, cOpt)
        | _ -> None

    // These three always succeed, no need to test for None
    static member createWhenAExists(a, bOpt, cOpt) = OOAtLeastOne(Some a, bOpt, cOpt)
    static member createWhenBExists(aOpt, b, cOpt) = OOAtLeastOne(aOpt, Some b, cOpt)
    static member createWhenCExists(aOpt, bOpt, c) = OOAtLeastOne(aOpt, bOpt, Some c)

    member this.Value = (aOpt, bOpt, cOpt)

// ---------------------------------------------
// Constrained DU (FP style)
// ---------------------------------------------

// DU with constraint that classification must be done correctly
type NumberClass =
    private
    | IsPositive of int // int must be > 0
    | IsNegative of int // int must be < 0
    | Zero

// Module containing functions related to NumberClass type
module NumberClass =
    let create i =
        if i > 0 then IsPositive i
        elif i < 0 then IsNegative i
        else Zero


    // active pattern used to extract data since type is private
    let (|IsPositive|IsNegative|Zero|) numberClass =
        match numberClass with
        | IsPositive i -> IsPositive i
        | IsNegative i -> IsNegative i
        | Zero -> Zero

// ======================================================
// This client attempts to use the types defined above
// ======================================================

module Client =

    // ---------------------------------------------
    // Constrained String50 (FP-style)
    // ---------------------------------------------

    let s50Bad = String50 "abc"
