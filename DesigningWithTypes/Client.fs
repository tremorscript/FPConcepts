// ======================================================
// This client attempts to use the types defined above
// ======================================================

module Client

open ContrainedTypes

// ---------------------------------------------
// Constrained String50 (FP-style)
// ---------------------------------------------

//let s50Bad = String50 "abc"
// The union cases or fields of the type 'String50' are not accessible from this code location

let s50opt = String50.create "abc"

s50opt
|> Option.map String50.value
|> Option.map (fun s -> s.ToUpper())
|> Option.iter (printfn "%s")

// ---------------------------------------------
// Constrained String50 (object-oriented style)
// ---------------------------------------------

// let ooS50Bad = OOString50("abc")
// This type has no accessible object constructors

let ooS50opt = OOString50.Create "abc"

ooS50opt
|> Option.map (fun s -> s.Value)
|> Option.map (fun s -> s.ToUpper())
|> Option.iter (printfn "%s")


// ---------------------------------------------
// Constrained AtLeastOne (FP style)
// ---------------------------------------------
// let atLeastOneBad = { A = None; B = None; C = None }
// The union cases or fields of the type 'AtLeastOne' are not accessible from this code location

let atLeastOne_BOnly = AtLeastOne.create None (Some 2) None

match atLeastOne_BOnly with
| Some x -> x |> AtLeastOne.value |> printfn "%A"
| None -> printfn "Not valid"

let atLeastOne_AOnly = AtLeastOne.createWhenAExists 1 None None
let atLeastOne_AB = AtLeastOne.createWhenAExists 1 (Some 2) None
atLeastOne_AB |> AtLeastOne.value |> printfn "%A"

// ---------------------------------------------
// Constrained AtLeastOne (OO style)
// ---------------------------------------------

// let ooAtLeastOneBad = OOAtLeastOne(None,None,None) //
// This type has no accessible object constructors

let atLeastOne_BOnly1 = OOAtLeastOne.create (None, Some 2, None)

match atLeastOne_BOnly1 with
| Some x -> printfn "%A" x.Value
| None -> printfn "Not valid"

let ooAtLeastOne_AOnly = OOAtLeastOne.createWhenAExists (1, None, None)
let ooAtLeastOne_AB = OOAtLeastOne.createWhenAExists (1, Some 2, None)
ooAtLeastOne_AB.Value |> printfn "A=%A"

// ---------------------------------------------
// Constrained DU (FP style)
// ---------------------------------------------

// let numberClassBad = IsPositive -1
// The union cases or fields of the type 'NumberClass' are not accessible from this code location


let numberClass = NumberClass.create -1
// this fails because the DU cases are not accessible

// The union cases or fields of the type 'NumberClass' are not accessible from this code location
// match numberClass with
// | IsPositive i -> printfn "%i is positive" i
// | IsNegative i -> printfn "%i is negative" i
// | Zero -> printfn "is zero"

open NumberClass // bring active pattern into scope
// this works because the active pattern is being used
match numberClass with
| IsPositive i -> printfn "%i is positive" i
| IsNegative i -> printfn "%i is negative" i
| Zero -> printfn "is zero"
