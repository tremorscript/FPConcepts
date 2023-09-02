(*
    One of the major uses of computation expressions is to implicitly unwrap and rewrap values that are stored in some sort of wrapper type.
*)

(*
    The signature of Return as documented on MSDN is just this:
    member Return : 'T -> M<'T>

    In other words, for some type T, the Return method just wraps it in the wrapper type.
*)

(*
    The signature of Bind is this:
    member Bind : M<'T> * ('T -> M<'U>) -> M<'U>

    In other words, what Bind does is:
        Take a wrapped value.
        Unwrap it and do any special “behind the scenes” logic.
        Then, optionally apply the function to the unwrapped value to create a new wrapped value.
        Even if the function is not applied, Bind must still return a wrapped U.
    
*)

(*
    Composition of computation expressions

    Every computation expression must have an associated wrapper type. 
    This wrapper type is used in both Bind and Return, which leads to a key benefit:
        the output of a Return can be fed to the input of a Bind
    
    In other words, because a workflow returns a wrapper type, and because let! consumes a wrapper type, 
    you can put a “child” workflow on the right hand side of a let! expression.

    let subworkflow1 = myworkflow { return 42 }
    let subworkflow2 = myworkflow { return 43 }
    
    let aWrappedValue =
        myworkflow {
            let! unwrappedValue1 = subworkflow1
            let! unwrappedValue2 = subworkflow2
            return unwrappedValue1 + unwrappedValue2
            }

    let aWrappedValue =
    myworkflow {
        let! unwrappedValue1 = myworkflow {
            let! x = myworkflow { return 1 }
            return x
            }
        let! unwrappedValue2 = myworkflow {
            let! y = myworkflow { return 2 }
            return y
            }
        return unwrappedValue1 + unwrappedValue2
        }

*)


(*
    Sometimes we have a function that already returns a wrapped value, and we want to return it directly. 
    return is no good for this, because it requires an unwrapped type as input.

    The solution is a variant on return called return!, which takes a wrapped type as input and returns it.

    The corresponding method in the “builder” class is called ReturnFrom. Typically the implementation just returns the wrapped type “as is” 


*)


(*
Rules to follow while implementing a computation expression:-

Rule 1: If you start with an unwrapped value, and then you wrap it (using return), then unwrap it (using bind), 
you should always get back the original unwrapped value.

myworkflow {
let originalUnwrapped = something

// wrap it
let wrapped = myworkflow { return originalUnwrapped }

// unwrap it
let! newUnwrapped = wrapped

// assert they are the same
assertEqual newUnwrapped originalUnwrapped
}

*)


(*

Rule 2:
If you start with a wrapped value, and then you unwrap it (using bind), then wrap it (using return), 
you should always get back the original wrapped value.

myworkflow {
let originalWrapped = something

let newWrapped = myworkflow {

    // unwrap it
    let! unwrapped = originalWrapped

    // wrap it
    return unwrapped
    }

// assert they are the same
assertEqual newWrapped originalWrapped
}
*)

(*
Rule 3:
If you create a child workflow, it must produce the same result as if you had “inlined” the logic in the main workflow.
// inlined
let result1 = myworkflow {
    let! x = originalWrapped
    let! y = f x  // some function on x
    return! g y   // some function on y
    }

// using a child workflow ("extraction" refactoring)
let result2 = myworkflow {
    let! y = myworkflow {
        let! x = originalWrapped
        return! f x // some function on x
        }
    return! g y     // some function on y
    }

// rule
assertEqual result1 result2


*)