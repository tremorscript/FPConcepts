namespace OrderApp

module InlineFunctions =
    
    (*Inline functions are functions that are integrated directly into the calling code.*)

    (*
        The inline modifier can be applied to functions at the top level, at the module level, or at the method level in a class.
    *)

    let inline increment x = x + 1
    type WrapInt32() =
        member inline this.incrementByOne(x) = x + 1
        static member inline Increment(x) = x + 1

    (*
        The presence of inline affects type inference. This is because inline functions can have statically resolved type parameters, 
        whereas non-inline functions cannot. The following code example shows a case where inline is helpful 
        because you are using a function that has a statically resolved type parameter, the float conversion operator.
    *)
    
    let inline printAsFloatingPoint number =
        printfn "%f" (float number)


    (*
        Without the inline modifier, type inference forces the function to take a specific type, in this case int. 
        But with the inline modifier, the function is also inferred to have a statically resolved type parameter. 
        With the inline modifier, the type is inferred to be the following:
    *)

    (*
        The F# compiler includes an optimizer that performs inlining of code. 
        The InlineIfLambda attribute allows code to optionally indicate that, if an argument is determined to be a lambda function, 
        then that argument should itself always be inlined at call sites. 
    *)

    (*
        For example, consider the following iterateTwice function to traverse an array:
    *)

    let inline iterateTwice ([<InlineIfLambda>] action) (array: 'T[]) =
        for i = 0 to array.Length-1 do
            action array[i]
        for i = 0 to array.Length-1 do
            action array[i]

    (*
        If the call site is:
    *)
    let arr = [| 1.. 100 |]
    let mutable sum = 0
    arr  |> iterateTwice (fun x ->
        sum <- sum + x)


    (*
        let arr = [| 1..100 |]
        let mutable sum = 0
        for i = 0 to arr.Length - 1 do
            sum <- sum + arr[i] 
        for i = 0 to arr.Length - 1 do
            sum <- sum + arr[i] 
    *)
    (*
        This optimization is applied regardless of the size of the lambda expression involved. 
        This feature can also be used to implement loop unrolling and similar transformations more reliably.
    *)
